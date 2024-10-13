using UnityEngine;

public class DoorMoveTrigger : MonoBehaviour
{
    // Reference to the door's animator
    public Animator doorAnimator;
    
    // The specific trigger for this door movement
    public string doorAnimationTrigger = "Open"; // Default to "Open" or customize for each trigger

    private bool isTriggered = false; // To prevent repeated triggering
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            Debug.Log("Triggered");
            isTriggered = true;  // Ensure the door only opens once

            if (doorAnimator != null)
            {
                // Trigger the animation based on the provided trigger name
                doorAnimator.SetTrigger(doorAnimationTrigger);
                SoundManager.instance.PlaySound(SoundManager.instance.DoorOpen);
            }
        }
    }
}
