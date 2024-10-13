using UnityEngine;
using UnityEngine.SceneManagement;

public class HospitalEntry : MonoBehaviour
{
    public PlayerMovement1 player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            SceneManager.LoadScene("FinalScene");
        }
    }
}
