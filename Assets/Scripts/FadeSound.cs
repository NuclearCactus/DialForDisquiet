using UnityEngine;

public class FadeSound : MonoBehaviour
{
    public GameObject Trigger;
    public GameObject player;
    private Vector3 playerPosition;
    // Called when the player enters the trigger area
    float TotalDistance, distanceToTarget;
    private void Start()
    {
        playerPosition = player.transform.position;
        TotalDistance = Vector3.Distance(playerPosition, Trigger.transform.position);
       
    }
    private void Update()
    {
        //distanceToTarget = Vector3.Distance(player.transform.position, Trigger.transform.position);
        //if (SoundManager.instance.BGaudioSource.volume>=0) SoundManager.instance.BGaudioSource.volume = 1 - distanceToTarget/TotalDistance;

    }
}
