using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpecialPhoneTrigger : MonoBehaviour
{
    public Transform player;                  // Reference to the player
    public Telefone tele;                     // Reference to the Telefone script
    public AudioClip dialogueClip;            // Audio clip for the dialogue
    public Sprite newPlayerSprite;            // The new sprite for the player after picking up the phone
    public GameObject blackBackground;        // Black square that will fade in
    public Image fullScreenImage;             // Full-screen image that shows after the dialogue ends
    public TextMeshProUGUI subtitleText;      // TextMeshPro for displaying subtitles
    public string[] subtitles;                // Array of subtitles to display
    public float subtitleInterval;            // Interval time for subtitles, set from the inspector
    public float fadeDuration = 2f;           // Duration for the fade-in effect of the black background

    private AudioSource audioSource;
    private SpriteRenderer playerRenderer;    // Player's sprite renderer
    private bool playerCanMove = true;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerRenderer = player.GetComponent<SpriteRenderer>();

        // Initialize the black background and full-screen image
        blackBackground.SetActive(false);
        fullScreenImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == player)
        {
            // 1. Trigger the phone ring when the player enters the trigger
            FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC");

            // Start the sequence after the player picks up the phone
            StartCoroutine(PhoneSequence());
        }
    }

    IEnumerator PhoneSequence()
    {
        // Wait until the player picks up the phone
        yield return new WaitUntil(() => tele.receiverUp);

        // 2. Disable player movement and change their sprite
        playerCanMove = false;
        player.GetComponent<PlayerMovement1>().enabled = false;

        // Change player's sprite to the new one
        playerRenderer.sprite = newPlayerSprite;

        // 3. Activate and fade in the black background
        blackBackground.SetActive(true);
        yield return StartCoroutine(FadeInBackground(blackBackground, fadeDuration));

        // 4. Play audio and display subtitles
        audioSource.PlayOneShot(dialogueClip);
        yield return StartCoroutine(ShowSubtitles(subtitles));

        // 5. Once the audio is done, activate the full-screen image
        fullScreenImage.gameObject.SetActive(true);
    }

    IEnumerator FadeInBackground(GameObject background, float duration)
    {
        // Get the sprite renderer of the black background
        SpriteRenderer bgRenderer = background.GetComponent<SpriteRenderer>();
        Color color = bgRenderer.color;
        color.a = 0f;  // Start with full transparency
        bgRenderer.color = color;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Gradually increase opacity
            color.a = Mathf.Clamp01(elapsedTime / duration);
            bgRenderer.color = color;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the background is fully opaque at the end
        color.a = 1f;
        bgRenderer.color = color;
    }

    IEnumerator ShowSubtitles(string[] subtitles)
    {
        // Step through each subtitle and display it for the specified interval
        foreach (string subtitle in subtitles)
        {
            subtitleText.text = subtitle;
            yield return new WaitForSeconds(subtitleInterval);
        }

        // Clear the subtitle after the last one
        subtitleText.text = "";
    }
}
