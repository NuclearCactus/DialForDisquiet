using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HospitalController : MonoBehaviour
{
    public Sprite[] hospitalSprites;  // Array of hospital sprites
    public float spriteChangeInterval = 2f;  // Time interval to change sprites
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    public Collider2D doorCollider;   // Reference to the door's BoxCollider2D

    public Transform player;          // Reference to the player (for centering the transition)
    public SpriteMask transitionMask; // The circular Sprite Mask
    public float transitionDuration = 1.5f; // Duration of the transition effect
    public GameObject blackBackground; // The full-screen black background

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (hospitalSprites.Length > 0)
        {
            InvokeRepeating("ChangeHospitalSprite", 0f, spriteChangeInterval);
        }
        else
        {
            Debug.LogWarning("No hospital sprites assigned!");
        }

        // Ensure the transition mask and black background are initially hidden
        if (transitionMask != null)
        {
            transitionMask.gameObject.SetActive(false);
        }
        if (blackBackground != null)
        {
            blackBackground.SetActive(false);
        }
    }

    void ChangeHospitalSprite()
    {
        currentSpriteIndex = (currentSpriteIndex + 1) % hospitalSprites.Length;
        spriteRenderer.sprite = hospitalSprites[currentSpriteIndex];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other is CapsuleCollider2D)
        {
            StartCoroutine(TransitionToNextScene());
        }
    }

    IEnumerator TransitionToNextScene()
    {
        if (transitionMask != null && player != null)
        {
            // Activate the black background and transition mask
            blackBackground.SetActive(true);
            transitionMask.gameObject.SetActive(true);

            // Center the mask on the player's position
            Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(player.position);
            transitionMask.transform.position = playerScreenPos;

            // Start the transition effect by scaling the mask down to zero
            float elapsedTime = 0f;
            float startScale = 5f; // Start with a large mask size
            float endScale = 0.01f; // End with a tiny mask

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(startScale, endScale, elapsedTime / transitionDuration);
                transitionMask.transform.localScale = new Vector3(scale, scale, scale);
                yield return null;
            }

            // After the transition, load the next scene
            SceneManager.LoadScene("HospitalScene");
        }
    }
}
