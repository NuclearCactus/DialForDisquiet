using UnityEngine;

public class StalkerAnimationTrigger : MonoBehaviour
{
    public Animator stalkerAnimator;    // Reference to the stalker's animator

    private bool animationPlayed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !animationPlayed)
        {
            stalkerAnimator.SetTrigger("TurnAndWalk");  // Trigger the stalker animation
            animationPlayed = true;                    // Prevent retriggering
            SoundManager.instance.PlaySound(SoundManager.instance.Glitch);
        }
    }
}
