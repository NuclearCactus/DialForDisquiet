using UnityEngine;

public class StalkerAnimation : MonoBehaviour
{
    public Animator stalkerAnimator;   // Reference to the stalker's animator

    public void TriggerStalkerAnimation()
    {
        // Play the "StalkerTurnAndWalk" animation
        stalkerAnimator.SetTrigger("TurnAndWalk");
    }
}
