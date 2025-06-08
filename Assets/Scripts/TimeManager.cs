using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public AudioSource scarySoundAudioSource;  // AudioSource for playing the camera switch sound
    public AudioClip scarySoundClip;           // The sound clip to play when switching cameras

    public TMP_Text timeText;                   // Reference to the TMP text element to display time
    public float secondsPerGameHour = 60f;      // Duration of a game hour in seconds

    private float timeElapsed;                    // Total time elapsed in seconds
    private int currentHour;                      // Current hour in the game (12-6)

    void Start()
    {
        // Initialize time to 12 AM
        currentHour = 12;

        // Update time display at the start
        UpdateTimeText();
    }

    void Update()
    {
        // Update the game time
        UpdateGameTime();
    }

    void UpdateGameTime()
    {
        timeElapsed += Time.deltaTime;  // Increase the elapsed time

        // Check if a game hour has passed
        if (timeElapsed >= secondsPerGameHour)
        {
            timeElapsed -= secondsPerGameHour; // Reset elapsed time for the next hour
            currentHour++;  // Increase the hour

            // Check for win condition at 6 AM
            if (currentHour == 6)
            {
                TriggerWin(); // Call win condition
            }
            if(currentHour == 3)
                scarySoundAudioSource.PlayOneShot(scarySoundClip);

            // Reset hour to 12 if it exceeds 6
            if (currentHour > 12)
            {
                currentHour = 1; // Loop back to 1 after 12
            }

            UpdateTimeText(); // Update time display
        }
    }

    void UpdateTimeText()
    {
        // Format time display as "12 AM", "1 AM", ..., "6 AM"
        timeText.text = $"{currentHour} AM";  // Update TMP text without leading zeros
    }

    void TriggerWin()
    {
        SceneManager.LoadScene("Win");
        // Add logic for winning the game here, such as transitioning to a win screen
        // For example, you might want to load a new scene or display a win message
    }
}