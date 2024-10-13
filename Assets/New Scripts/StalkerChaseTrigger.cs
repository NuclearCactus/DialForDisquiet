using System.Collections;
using UnityEngine;
using TMPro;

public class StalkerChaseTrigger : MonoBehaviour
{
    public GameObject stalker;                // Reference to the stalker GameObject
    public Transform player;                  // Reference to the player
    public AudioClip stalkerDialogueClip;     // Audio clip for stalker dialogue
    public AudioClip policeAudioClip;         // Audio for police
    public AudioClip friend1AudioClip;        // Audio for Friend1
    public AudioClip friend2AudioClip;        // Audio for Friend2
    public GameObject phoneLogo;              // Phone logo on HUD
    public GameObject phonebookUI;            // Phonebook UI (contains TextMeshPro numbers)
    public TMP_Text[] phonebookNumbers;       // Array of TextMeshPro objects for phone numbers
    public TextMeshProUGUI subtitleText;      // TextMeshPro for displaying subtitles

    // Subtitle duration for each type (exposed in inspector)
    public float stalkerSubtitleDuration = 2f;
    public float policeSubtitleDuration = 2f;
    public float friend1SubtitleDuration = 2f;
    public float friend2SubtitleDuration = 2f;

    public float stalkerSpeed = 2f;           // Speed at which the stalker moves towards the player
    public Telefone tele;                     // Reference to the Telefone script
    public PhoneInput phone;

    public Sprite[] stalkerMovingSprites;     // Array to hold the moving sprites for the stalker
    public float spriteSwitchTime = 0.3f;     // Time interval for switching sprites during movement

    private AudioSource audioSource;
    private bool playerCanMove = true;
    private bool isStalkerChasing = false;
    private bool phonebookOpen = false;
    private int dialedNumbersCount = 0;
    private string dialedNumber = "";
    private int currentStalkerSpriteIndex = 0; // Index to alternate stalker moving sprites
    private float spriteTimer = 0f;           // Timer for sprite switching
    private SpriteRenderer stalkerRenderer;   // Reference to stalker's SpriteRenderer

    public string[] stalkerSubtitles; // Define stalker subtitles
    public string[] policeSubtitles;  // Define police subtitles
    public string[] friend1Subtitles; // Define Friend1 subtitles
    public string[] friend2Subtitles; // Define Friend2 subtitles
    bool fastenHeartBeat = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        stalker.SetActive(false); // Initially deactivate the stalker
        phoneLogo.SetActive(false); // Hide phone logo initially
        phonebookUI.SetActive(false); // Hide phonebook UI initially

        // Get the stalker's SpriteRenderer
        stalkerRenderer = stalker.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == player && !isStalkerChasing)
        {
            // 1. Stalker appears, player can't move, and phone rings
            stalker.SetActive(true);
            player.GetComponent<PlayerMovement1>().enabled = false;

            FindAnyObjectByType<ArduinoController>().SendSerialMessage("ringC"); // Ring the phone
            StartCoroutine(WaitForPlayerPickUp());
            SoundManager.instance.PlaySound(SoundManager.instance.Glitch);
        }
    }

    private void Update()
    {
        if (isStalkerChasing)
        {
            if (!fastenHeartBeat)
            {
                SoundManager.instance.PlaySound(SoundManager.instance.WomanBreating);
                SoundManager.instance.audioSource.loop = true;
                SoundManager.instance.audioSource.volume = 0.5f;

                fastenHeartBeat=true;
            }
            // 3. Make the stalker chase the player
            Vector3 direction = player.position - stalker.transform.position;
            stalker.transform.position += direction.normalized * stalkerSpeed * Time.deltaTime;

            // Update sprite animation for the stalker
            spriteTimer += Time.deltaTime;
            if (spriteTimer >= spriteSwitchTime)
            {
                // Switch to the next sprite in the array
                currentStalkerSpriteIndex = (currentStalkerSpriteIndex + 1) % stalkerMovingSprites.Length;
                stalkerRenderer.sprite = stalkerMovingSprites[currentStalkerSpriteIndex];
                spriteTimer = 0f;
            }
        }

        if (phonebookOpen && tele.receiverUp)
        {
            ProcessDialedNumber(); // Handle phonebook dialing when it's open
        }
    }

    IEnumerator WaitForPlayerPickUp()
    {
        // Wait until the player picks up the phone
        yield return new WaitUntil(() => tele.receiverUp);

        // 2. Play stalker audio and show subtitles
        audioSource.PlayOneShot(stalkerDialogueClip);
        StartCoroutine(ShowSubtitles(stalkerSubtitles, stalkerSubtitleDuration));

        // Wait until player puts down the phone
        yield return new WaitUntil(() => !tele.receiverUp);

        // Player can move again, and stalker starts chasing the player
        player.GetComponent<PlayerMovement1>().enabled = true;
        isStalkerChasing = true;

        // 4. Show the phone logo on the HUD
        phoneLogo.SetActive(true);

        // 5. Wait until the player picks up the phone again
        yield return new WaitUntil(() => tele.receiverUp);

        // PhonebookUI
        OpenPhonebook();
    }

    IEnumerator ShowSubtitles(string[] subtitles, float duration)
    {
        // Step through each subtitle and display it for a set duration
        foreach (string subtitle in subtitles)
        {
            subtitleText.text = subtitle;
            yield return new WaitForSeconds(duration);
        }

        // Clear the subtitle after the last one
        subtitleText.text = "";
    }

    public void OpenPhonebook()
    {
        // 5. Open the phonebook UI
        phoneLogo.SetActive(false); // Hide phone logo
        phonebookUI.SetActive(true); // Show phonebook
        phonebookOpen = true;
    }

    private void ProcessDialedNumber()
    {
        // Match the dialed number with contacts and play respective audio + subtitles
        if (phone.number.EndsWith("100")) // Police
        {
            PlayPhonebookAudio(policeAudioClip);
            StrikeOutPhoneNumber(0);
            StartCoroutine(ShowSubtitlesWithDelay(policeSubtitles, policeSubtitleDuration));
            phone.number = "";
        }
        else if (phone.number.EndsWith("101")) // Friend1
        {
            PlayPhonebookAudio(friend1AudioClip);
            StrikeOutPhoneNumber(1);
            StartCoroutine(ShowSubtitlesWithDelay(friend1Subtitles, friend1SubtitleDuration));
            phone.number = "";
        }
        else if (phone.number.EndsWith("102")) // Friend2
        {
            PlayPhonebookAudio(friend2AudioClip);
            StrikeOutPhoneNumber(2);
            StartCoroutine(ShowSubtitlesWithDelay(friend2Subtitles, friend2SubtitleDuration));
            phone.number = "";
        }
    }

    private void PlayPhonebookAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
        dialedNumbersCount++;

        // 7. Once all numbers have been dialed, hide the phonebook
        if (dialedNumbersCount == 3)
        {
            phonebookUI.SetActive(false);
            phonebookOpen = false;
        }
    }

    private IEnumerator ShowSubtitlesWithDelay(string[] subtitles, float duration)
    {
        // Wait for 7 seconds before showing subtitles
        yield return new WaitForSeconds(8f);
        StartCoroutine(ShowSubtitles(subtitles, duration));
    }

    private void StrikeOutPhoneNumber(int index)
    {
        // Strike through the phone number in the phonebook UI
        phonebookNumbers[index].fontStyle = FontStyles.Strikethrough;
    }

    public void StopChase()
    {
        isStalkerChasing = false;
    }
}
