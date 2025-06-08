using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Newspaper : MonoBehaviour
{
    public float newspaperDuration = 6f; // Duration to display the newspaper scene

    void Start()
    {
        // Start a coroutine to load the game scene after a delay
        StartCoroutine(ShowNewspaperAndLoadGame());
    }

    private IEnumerator ShowNewspaperAndLoadGame()
    {
        // Wait for the specified duration (6 seconds)
        yield return new WaitForSeconds(newspaperDuration);

        // Load the Game Scene after the delay (replace "GameScene" with your scene name)
        SceneManager.LoadScene("GameScene");
    }

}
