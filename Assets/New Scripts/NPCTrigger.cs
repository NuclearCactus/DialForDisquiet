using UnityEngine;
using TMPro;
using System.Collections;

public class NPCTrigger : MonoBehaviour
{
    public GameObject npcObject;              // Reference to the NPC object (already active)
    public GameObject player;                 // Reference to the player object
    public Telefone tele;                     // Reference to the Telefone script for phone interaction
    public AudioSource audioSource;           // AudioSource for playing dialogue
    public AudioClip dialogueClip;            // Audio clip for NPC dialogue
    public string[] subtitles;                // Array of subtitles to display
    public TextMeshProUGUI subtitleText;      // TextMeshPro for displaying subtitles
    public float subtitleDuration = 2f;       // Duration for each subtitle (settable in Inspector)

    public Sprite idleSprite;                 // Final idle sprite after NPC stops moving
    public Sprite blackAndWhiteSprite;        // Black-and-white sprite to change to after phone pickup
    public Sprite[] walkingSprites;           // Two walking sprites for NPC animation
    public float moveSpeed = 2f;              // Speed at which the NPC moves towards the player

    private SpriteRenderer npcRenderer;       // NPC's sprite renderer for changing sprites
    private bool isMoving = false;            // Track if NPC is moving
    private int currentWalkingSpriteIndex = 0;// Index to alternate walking sprites

    private void Start()
    {
        // Ensure the audio source is set up properly
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the dialogue clip to the audio source
        audioSource.clip = dialogueClip;
        audioSource.playOnAwake = false;

        // Get the NPC's sprite renderer for handling sprite changes
        npcRenderer = npcObject.GetComponent<SpriteRenderer>();
        npcRenderer.sprite = idleSprite; // Set the initial idle sprite
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Player triggered NPC event.");
            StartCoroutine(ActivateNPCSequence());
        }
    }

    private IEnumerator ActivateNPCSequence()
    {
        // Step 1: Player movement stops
        player.GetComponent<PlayerMovement1>().enabled = false;

        // Step 2: NPC starts moving towards the player
        isMoving = true;
        StartCoroutine(MoveNPC());

        // Step 3: Ring the phone using Arduino
        Debug.Log("Sending ring command to Arduino.");
        FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");

        // Step 4: Wait for the player to pick up the phone
        Debug.Log("Waiting for phone pickup...");
        yield return new WaitUntil(() => tele.receiverUp);
        Debug.Log("Phone picked up.");

        // Change the NPC sprite to black-and-white after phone pickup
        npcRenderer.sprite = blackAndWhiteSprite;

        // Step 5: Play NPC dialogue and show subtitles
        StartCoroutine(PlayDialogueAndSubtitles());

        // Step 6: Wait for the player to hang up the phone
        Debug.Log("Waiting for phone hangup...");
        yield return new WaitUntil(() => !tele.receiverUp);
        Debug.Log("Phone hung up.");

        // Step 7: NPC remains in its final idle position and player can move again
        player.GetComponent<PlayerMovement1>().enabled = true;
        Debug.Log("Player can move again.");
    }

    private IEnumerator MoveNPC()
    {
        // The NPC moves towards the player, alternating walking sprites
        while (isMoving)
        {
            // Calculate the direction towards the player
            Vector3 direction = (player.transform.position - npcObject.transform.position).normalized;

            // Move the NPC towards the player
            npcObject.transform.position += direction * moveSpeed * Time.deltaTime;

            // Check if NPC is close enough to the player
            if (Vector3.Distance(npcObject.transform.position, player.transform.position) < 0.5f)
            {
                // NPC stops moving when it's close to the player
                isMoving = false;
                npcRenderer.sprite = idleSprite; // Set final idle sprite
            }
            else
            {
                // Alternate walking sprites during movement
                npcRenderer.sprite = walkingSprites[currentWalkingSpriteIndex];
                currentWalkingSpriteIndex = (currentWalkingSpriteIndex + 1) % walkingSprites.Length;

                // Delay for smooth sprite animation
                yield return new WaitForSeconds(0.2f); // Adjust time for sprite switching
            }
        }
    }

    private IEnumerator PlayDialogueAndSubtitles()
    {
        // Play dialogue audio
        audioSource.Play();
        Debug.Log("Playing NPC dialogue audio.");

        // Step through each subtitle and display it for a set duration
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitleText.text = subtitles[i];  // Display current subtitle
            Debug.Log("Showing subtitle: " + subtitles[i]);

            // Wait for the duration of the subtitle or until the audio ends
            yield return new WaitForSeconds(subtitleDuration);
        }

        // Clear the subtitle after the last one
        subtitleText.text = "";
        Debug.Log("Subtitles finished.");
    }
}
