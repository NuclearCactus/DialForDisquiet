using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HospitalDialogueManager : MonoBehaviour
{
    public float moveSpeed = 2f; // Constant movement speed
    public Sprite walkSprite1; // First walking sprite
    public Sprite walkSprite2; // Second walking sprite
    public Sprite idleSprite;  // Idle sprite
    public float spriteSwitchTime = 0.2f; // Time between sprite switches

    public AudioClip dialogueAudio; // The audio clip to play during dialogue
    public Image dialogueUIImage; // Image to display during dialogue (in Canvas)
    private bool hasReachedTrigger = false; // Player reaches dialogue trigger
    public Telefone tele; // For phone interaction (receiverUp)

    private AudioSource audioSource; // Source for audio playback
    private SpriteRenderer spriteRenderer; // Handle player sprite switching
    private bool isMoving = true; // Track player movement status
    private float spriteTimer;
    private bool useFirstSprite = true; // Toggle between walking sprites

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Add and configure AudioSource if not already present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        spriteRenderer.sprite = walkSprite1; // Start with first walking sprite
        dialogueUIImage.gameObject.SetActive(false); // Initially hide the dialogue image
    }

    void Update()
    {
        // If the player is moving, handle walking animation
        if (isMoving && !hasReachedTrigger)
        {
            MoveForward();
            HandleWalkingAnimation();
        }
        else
        {
            spriteRenderer.sprite = idleSprite; // Set to idle sprite when stopped
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime); // Constant forward movement
    }

    private void HandleWalkingAnimation()
    {
        spriteTimer += Time.deltaTime;

        // Switch between walking sprites at regular intervals
        if (spriteTimer >= spriteSwitchTime)
        {
            spriteRenderer.sprite = useFirstSprite ? walkSprite1 : walkSprite2;
            useFirstSprite = !useFirstSprite; // Toggle between sprites
            spriteTimer = 0f; // Reset sprite timer
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DialogueTrigger"))
        {
            hasReachedTrigger = true;
            isMoving = false; // Stop the player
            StartCoroutine(HandleDialogue());
            // Phone starts ringing via Arduino controller
            
        }
    }

    private IEnumerator HandleDialogue()
    {
        FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");
        // Wait for the player to pick up the phone
        yield return StartCoroutine(WaitForPhonePickup());

        // Show dialogue UI and play the dialogue audio
        dialogueUIImage.gameObject.SetActive(true);
        audioSource.clip = dialogueAudio;
        audioSource.Play();

        // Wait until the dialogue audio finishes playing
        yield return new WaitUntil(() => !audioSource.isPlaying);

        // dialogueUIImage.gameObject.SetActive(false); // Hide the dialogue UI

        // Wait for the player to hang up the phone
        yield return new WaitUntil(() => !tele.receiverUp);

        Debug.Log("Phone hung up.");

        // isMoving = true; // Allow the player to move again
    }

    private IEnumerator WaitForPhonePickup()
    {
        yield return new WaitUntil(() => tele.receiverUp); // Wait for the player to pick up the phone
    }
}
