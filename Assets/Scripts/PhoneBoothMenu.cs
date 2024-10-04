using UnityEngine;

public class PhoneBoothMenu : MonoBehaviour
{
    public GameObject phoneBoothMenuUI; // The Canvas or UI Panel that should be shown when the game is paused
    public Telefone tele; // Reference to your existing Telefone script to check if '#' is pressed

    private bool isPaused = false; // Tracks if the game is currently paused

    void Start()
    {
        // Ensure the UI is hidden at the start of the game
        phoneBoothMenuUI.SetActive(false);
    }

    void Update()
    {
        // If the game is paused, check if '#' is pressed to exit the pause menu
        if (isPaused && tele.hashKeyPressed) // Now using the property to check if '#' is pressed
        {
            ResumeGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When the player enters the phone booth trigger, pause the game
        if (other.CompareTag("PhoneBooth")) // Make sure the trigger is tagged "PhoneBooth"
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        // Set time scale to 0 to pause the game
        Time.timeScale = 0f;
        // Show the phone booth UI
        phoneBoothMenuUI.SetActive(true);
        // Set the paused state to true
        isPaused = true;
    }

    private void ResumeGame()
    {
        // Set time scale to 1 to resume the game
        Time.timeScale = 1f;
        // Hide the phone booth UI
        phoneBoothMenuUI.SetActive(false);
        // Set the paused state to false
        isPaused = false;
    }
}
