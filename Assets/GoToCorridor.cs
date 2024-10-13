using UnityEngine;
using UnityEngine.SceneManagement;
public class GoToCorridor : MonoBehaviour
{
    public PlayerMovement1 player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            Debug.Log("Entering");
            SceneManager.LoadScene("CorridorScene");
        }
    }
}
