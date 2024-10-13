using System.Collections;
using UnityEngine;

public class ProtestTrigger : MonoBehaviour
{
    public GameObject currentSprite; // The sprite that will fade away
    public GameObject newSprite;      // The new sprite that will appear
    public float fadeDuration = 1f;   // Duration of the fade effect
    public PlayerMovement1 playerMovement; // Reference to the player's movement script

    private void Start()
    {
        // Ensure the new sprite is not visible initially
        newSprite.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Assuming the player has the tag "Player"
        {
            StartCoroutine(FadeAndReplace());
        }
    }

    private IEnumerator FadeAndReplace()
    {
        // Disable player movement during the fade
        playerMovement.enabled = false;

        // Fade out the current sprite
        SpriteRenderer currentSpriteRenderer = currentSprite.GetComponent<SpriteRenderer>();
        Color currentColor = currentSpriteRenderer.color;

        // Gradually reduce the alpha value to create a fade-out effect
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            currentColor.a = Mathf.Lerp(1, 0, normalizedTime);
            currentSpriteRenderer.color = currentColor;
            yield return null;
        }

        // Ensure the current sprite is completely transparent
        currentColor.a = 0;
        currentSpriteRenderer.color = currentColor;

        // Deactivate the current sprite and activate the new sprite
        currentSprite.SetActive(false);
        newSprite.SetActive(true);

        // Optionally, fade in the new sprite (if desired)
        SpriteRenderer newSpriteRenderer = newSprite.GetComponent<SpriteRenderer>();
        Color newColor = newSpriteRenderer.color;
        newColor.a = 0; // Start transparent
        newSpriteRenderer.color = newColor;

        // Fade in the new sprite
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            newColor.a = Mathf.Lerp(0, 1, normalizedTime);
            newSpriteRenderer.color = newColor;
            yield return null;
        }

        // Ensure the new sprite is completely opaque
        newColor.a = 1;
        newSpriteRenderer.color = newColor;

        // Re-enable player movement after fading is complete
        playerMovement.enabled = true;
    }
}
