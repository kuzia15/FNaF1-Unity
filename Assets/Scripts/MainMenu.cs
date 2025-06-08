using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    // This function will be called when the Play button is pressed
    public void PlayGame()
    {
        // Load the game scene (replace "GameScene" with the name of your game scene)
        SceneManager.LoadScene("Newspaper");
    }
}
