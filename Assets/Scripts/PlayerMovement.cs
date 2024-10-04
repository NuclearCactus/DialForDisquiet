using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Constant movement speed
    public Sprite walkSprite1; // First walking sprite
    public Sprite walkSprite2; // Second walking sprite
    public Sprite idleSprite;  // Idle sprite
    public float spriteSwitchTime = 0.2f; // Time between sprite switches

    public AudioClip[] npcAudioClips; // Array of NPC audio clips for sequential playback
    public AudioClip[] stalkerAudioClips; // Array of Stalker audio clips for sequential playback

    private int npcDialogueIndex = 0; // Track which NPC dialogue is playing
    private int stalkerDialogueIndex = 0; // Track which Stalker dialogue is playing

    private AudioSource audioSource;
    private bool isMoving = true; // Player's movement status
    private SpriteRenderer spriteRenderer;
    private float spriteTimer;
    private bool useFirstSprite = true; // Toggle between walking sprites
    public Telefone tele;

    public List<GameObject> stalkerObjects; // List of Stalker objects in the Inspector
    private int currentStalkerIndex = 0; // Track the current Stalker object to activate

    public List<GameObject> npcObjects; // List of NPC objects
    private int currentNPCIndex = 0; // Track the current NPC

    public float npcMoveSpeed = 2f; // Speed of NPC moving towards player

    // Reference to the pause menu UI GameObject
    public GameObject pauseMenuUI; 


    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Check if the AudioSource exists, if not, add one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        spriteRenderer.sprite = walkSprite1; // Start with the first walking sprite

        // Disable all stalker objects at the start of the game
        foreach (var stalker in stalkerObjects)
        {
            stalker.SetActive(false);
        }

        // Disable all NPCs at the start of the game
        foreach (var npc in npcObjects)
        {
            npc.SetActive(false);
        }

        // Ensure the pause menu UI is inactive at the start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
    }

    void Update()
    {
        // If the player is moving, switch between the walking sprites
        if (isMoving)
        {
            MoveForward();
            HandleWalkingAnimation();
        }
        else
        {
            spriteRenderer.sprite = idleSprite; // Set to idle sprite when not moving
        }
    }

    private void MoveForward()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime); // Move forward at constant speed
    }

    private void HandleWalkingAnimation()
    {
        spriteTimer += Time.deltaTime;

        // Switch walking sprites at regular intervals
        if (spriteTimer >= spriteSwitchTime)
        {
            spriteRenderer.sprite = useFirstSprite ? walkSprite1 : walkSprite2;
            useFirstSprite = !useFirstSprite; // Toggle the sprite
            spriteTimer = 0f; // Reset timer
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPCPoint"))
        {
            isMoving = false; // Stop moving when hitting a trigger point
            StartCoroutine(HandleNPCInteraction());
        }
        else if (other.CompareTag("StalkerPoint"))
        {
            isMoving = false; // Stop moving when hitting a trigger point
            StartCoroutine(HandleStalkerInteraction());
        }
        else if (other.CompareTag("PhoneBooth"))
        {
            isMoving = false; // Stop moving when entering the PhoneBooth
            EnterPauseMenu();
        }
    }

    private void EnterPauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true); // Activate the pause menu UI
            StartCoroutine(WaitForHash());
        }
    }

    private IEnumerator HandleNPCInteraction()
    {
        FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");

        // Activate and move the next NPC towards the player
        if (currentNPCIndex < npcObjects.Count)
        {
            GameObject npc = npcObjects[currentNPCIndex];
            npc.SetActive(true); // Enable the NPC
            yield return StartCoroutine(MoveNPCToPlayer(npc)); // Move the NPC towards the player
            currentNPCIndex++; // Move to the next NPC for the next trigger
        }

        // Wait for the player to pick up the phone
        yield return StartCoroutine(WaitForPhonePickup());

        // Play the next NPC dialogue
        if (npcDialogueIndex < npcAudioClips.Length)
        {
            PlayNPCAudio(npcDialogueIndex);
            npcDialogueIndex++; // Move to the next dialogue for the next trigger
        }

        // Wait for the player to hang up the phone
        yield return StartCoroutine(WaitForPhoneHangup());

        isMoving = true; // Allow the player to move again
    }

    private IEnumerator MoveNPCToPlayer(GameObject npc)
    {
        Vector3 startPosition = npc.transform.position;

        // Get the player's capsule collider and NPC's collider bounds
        CapsuleCollider2D playerCollider = GetComponent<CapsuleCollider2D>();
        CapsuleCollider2D npcCollider = npc.GetComponent<CapsuleCollider2D>();

        // Calculate the extents (half-width) of both colliders
        float playerColliderWidth = playerCollider.size.x * Mathf.Abs(transform.localScale.x) / 2f; // Consider scale in case it's not 1
        float npcColliderWidth = npcCollider.size.x * Mathf.Abs(npc.transform.localScale.x) / 2f;

        // Calculate the target position for the NPC to stop beside the player, avoiding overlap
        float gap = 0.1f; // A small gap to ensure no overlap
        Vector3 targetPosition;

        if (npc.transform.position.x < transform.position.x) // NPC is to the left of the player
        {
            targetPosition = new Vector3(transform.position.x - (playerColliderWidth + npcColliderWidth + gap), npc.transform.position.y, npc.transform.position.z);
        }
        else // NPC is to the right of the player
        {
            targetPosition = new Vector3(transform.position.x + (playerColliderWidth + npcColliderWidth + gap), npc.transform.position.y, npc.transform.position.z);
        }

        SpriteRenderer npcSpriteRenderer = npc.GetComponent<SpriteRenderer>();
        NPCSpriteController npcSprites = npc.GetComponent<NPCSpriteController>(); // Fetch the walking/idle sprites

        float npcTimer = 0f;
        bool useFirstNpcSprite = true;

        // Move towards the target position and animate
        while (Vector3.Distance(npc.transform.position, targetPosition) > 0.05f)
        {
            npc.transform.position = Vector3.MoveTowards(npc.transform.position, targetPosition, npcMoveSpeed * Time.deltaTime);

            // Handle NPC walking animation
            npcTimer += Time.deltaTime;
            if (npcTimer >= spriteSwitchTime)
            {
                npcSpriteRenderer.sprite = useFirstNpcSprite ? npcSprites.walkSprite1 : npcSprites.walkSprite2;
                useFirstNpcSprite = !useFirstNpcSprite;
                npcTimer = 0f;
            }

            yield return null;
        }

        // Stop the NPC and switch to idle sprite
        npcSpriteRenderer.sprite = npcSprites.idleSprite;
    }

    private IEnumerator HandleStalkerInteraction()
    {
        FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");

        // Activate the next stalker object
        if (currentStalkerIndex < stalkerObjects.Count)
        {
            GameObject stalker = stalkerObjects[currentStalkerIndex];
            stalker.SetActive(true); // Activate the stalker object

            // Wait for the player to pick up the phone
            yield return StartCoroutine(WaitForPhonePickup());

            // Play the next Stalker dialogue
            if (stalkerDialogueIndex < stalkerAudioClips.Length)
            {
                PlayStalkerAudio(stalkerDialogueIndex);
                stalkerDialogueIndex++; // Move to the next dialogue for the next trigger
            }

            // Wait for the player to hang up the phone
            yield return StartCoroutine(WaitForPhoneHangup());

            // Deactivate the stalker object
            stalker.SetActive(false);

            currentStalkerIndex++; // Move to the next stalker object for the next trigger
        }

        isMoving = true; // Allow the player to move again
    }

    private IEnumerator WaitForPhonePickup()
    {
        yield return new WaitUntil(() => tele.receiverUp);
        Debug.Log("Phone picked up.");
    }

    private IEnumerator WaitForHash()
    {
        yield return new WaitUntil(() => tele.hashKeyPressed);
        pauseMenuUI.SetActive(false);
        isMoving = true;
    }

    private IEnumerator WaitForPhoneHangup()
    {
        yield return new WaitUntil(() => !tele.receiverUp);
        Debug.Log("Phone hung up.");
    }

    private void PlayNPCAudio(int index)
    {
        if (index < npcAudioClips.Length && npcAudioClips[index] != null)
        {
            audioSource.clip = npcAudioClips[index];
            audioSource.Play(); // Play the NPC audio
        }
        else
        {
            Debug.LogWarning("NPC audio clip not found at index " + index);
        }
    }

    private void PlayStalkerAudio(int index)
    {
        if (index < stalkerAudioClips.Length && stalkerAudioClips[index] != null)
        {
            audioSource.clip = stalkerAudioClips[index];
            audioSource.Play(); // Play the Stalker audio
        }
        else
        {
            Debug.LogWarning("Stalker audio clip not found at index " + index);
        }
    }
}
