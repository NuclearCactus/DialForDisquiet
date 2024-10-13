using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorOpenTrigger : MonoBehaviour
{
    public SpriteRenderer doorRenderer;    // Reference to the door's SpriteRenderer
    public Sprite openDoorSprite;          // The sprite for the open door
    public string nextSceneName;           // The name of the next scene to load

    private bool doorOpened = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !doorOpened)
        {
            doorRenderer.sprite = openDoorSprite;  // Change the door sprite to the open door
            doorOpened = true;                     // Prevent further triggering

            // Optionally load the next scene (if required after entering the door)
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
