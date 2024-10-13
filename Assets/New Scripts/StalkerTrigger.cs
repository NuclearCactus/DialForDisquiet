using UnityEngine;
using TMPro;
using System.Collections;

public class StalkerTrigger : MonoBehaviour
{
    public GameObject stalkerObject;          // Reference to the stalker object
    public GameObject player;                 // Reference to the player object
    // public ArduinoController arduinoController; // Reference to Arduino controller for ringing
    public Telefone tele;                     // Reference to the Telefone script
    public AudioSource audioSource;           // AudioSource for playing dialogue
    public AudioClip dialogueClip;            // Audio clip for dialogue
    public string[] subtitles;                // Array of subtitles to display
    public TextMeshProUGUI subtitleText;      // TextMeshPro for displaying subtitles
    public float subtitleDuration = 2f;       // Duration for each subtitle (can adjust in Inspector)

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

        // Initially, hide the stalker object
        stalkerObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            Debug.Log("Player triggered Stalker event.");
            StartCoroutine(ActivateStalkerSequence());
        }
    }

    private IEnumerator ActivateStalkerSequence()
    {
        // Step 1: Stalker appears and player movement stops
        stalkerObject.SetActive(true);
        player.GetComponent<PlayerMovement1>().enabled = false;  // Disable player movement

        // Step 2: Ring the phone using Arduino
        Debug.Log("Sending ring command to Arduino.");
        FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");

        // Step 3: Wait for the player to pick up the phone
        Debug.Log("Waiting for phone pickup...");
        yield return new WaitUntil(() => tele.receiverUp);  // Wait until receiver is picked up
        Debug.Log("Phone picked up.");

        // Step 4: Play dialogue and show subtitles
        StartCoroutine(PlayDialogueAndSubtitles());

        // Step 5: Wait for the player to hang up the phone
        Debug.Log("Waiting for phone hangup...");
        yield return new WaitUntil(() => !tele.receiverUp);  // Wait until receiver is hung up
        Debug.Log("Phone hung up.");

        // Step 6: Stalker disappears and player can move again
        stalkerObject.SetActive(false);
        player.GetComponent<PlayerMovement1>().enabled = true;  // Enable player movement
    }

    private IEnumerator PlayDialogueAndSubtitles()
    {
        // Play dialogue audio
        audioSource.Play();
        Debug.Log("Playing dialogue audio.");

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
