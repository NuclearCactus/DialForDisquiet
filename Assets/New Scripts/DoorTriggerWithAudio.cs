using System.Collections;
using UnityEngine;
using TMPro; // Make sure you are using the TextMeshPro namespace

public class DoorTriggerWithAudio : MonoBehaviour
{
    public Telefone tele;
    public GameObject player;                 // Reference to the player
    public PlayerMovement1 playerMovement;     // Reference to the player movement script
    public AudioSource audioSource;           // AudioSource component to play dialogue
    public AudioClip[] dialogueClips;         // Array of audio clips for the dialogue
    public string[] subtitles;                // Subtitles to display with dialogue
    public TextMeshProUGUI subtitleText;      // TMP Text element to display subtitles

    public SpriteRenderer doorRenderer;       // SpriteRenderer for the door
    public Sprite closedDoor;                 // Sprite for the closed door
    public Sprite doorOpening1;               // First step of the door opening
    public Sprite doorOpening2;               // Second step of the door opening
    public Sprite openDoor;                   // Final open door sprite

    public float doorAnimationDelay = 0.5f;   // Time between door sprite changes
    public float subtitleDisplayTime = 2.0f;  // Time each subtitle is shown (adjustable in Inspector)
    private bool playerInTrigger = false;     // Track if player is in trigger
    private bool dialogueTriggered = false;   // To ensure dialogue triggers only once

    IEnumerator TriggerDialogueAndOpenDoor()
    {
        // Disable player movement while dialogue is playing
        playerMovement.enabled = false;
        FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");
        yield return new WaitUntil(() => tele.receiverUp);
        // Play the Audio clip
        audioSource.clip = dialogueClips[0];
        audioSource.Play();

        subtitleText.text = subtitles[0];
        yield return new WaitForSeconds(subtitleDisplayTime);
        subtitleText.text = subtitles[1];
        yield return new WaitForSeconds(subtitleDisplayTime);
        subtitleText.text = subtitles[2];
        yield return new WaitForSeconds(subtitleDisplayTime);
        subtitleText.text = subtitles[3];
        yield return new WaitForSeconds(subtitleDisplayTime);
        subtitleText.text = subtitles[4];
        yield return new WaitForSeconds(subtitleDisplayTime);

        // Clear the subtitle text after all subtitles are shown
        subtitleText.text = "";

        // Animate the door opening after the dialogue finishes
        yield return new WaitForSeconds(doorAnimationDelay);
        doorRenderer.sprite = doorOpening1;  // First part of the animation

        yield return new WaitForSeconds(doorAnimationDelay);
        doorRenderer.sprite = doorOpening2;  // Second part of the animation

        yield return new WaitForSeconds(doorAnimationDelay);
        doorRenderer.sprite = openDoor;      // Door fully open

        // Re-enable player movement once dialogue and door animation are finished
        playerMovement.enabled = true;
    }

    // Trigger detection for the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            playerInTrigger = true;
            // Check if player is in trigger zone and dialogue hasn't started yet
            if (!dialogueTriggered)
            {
                dialogueTriggered = true;
                StartCoroutine(TriggerDialogueAndOpenDoor());
            }
        }
    }
}
