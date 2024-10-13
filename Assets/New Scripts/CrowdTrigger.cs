using UnityEngine;

public class CrowdTrigger : MonoBehaviour
{
    public StalkerChaseTrigger stalkerChaseTrigger;  // Reference to the StalkerChaseTrigger script
    public GameObject phoneBookUI;  // Reference to the phonebook UI

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Stop the stalker from chasing
            stalkerChaseTrigger.StopChase();

            // Hide the phoneBook UI when the player reaches the crowd trigger
            phoneBookUI.SetActive(false);
            Debug.Log("CrowdTrigger activated: Stalker stopped, phonebook hidden.");
        }
    }
}
