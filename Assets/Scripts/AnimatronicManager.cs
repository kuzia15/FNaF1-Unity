using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AnimatronicManager : MonoBehaviour
{
    public AudioSource movementAudioSource;  // AudioSource to play movement sound
    public AudioClip movementSound;          // AudioClip for the movement sound

    public AudioSource ScareAudioSource;  // AudioSource to play movement sound
    public AudioClip ScareSound;          // AudioClip for the movement sound

    public AudioSource deepSoundAudioSource;  // AudioSource for playing the camera switch sound
    public AudioClip deepSoundClip;


    public GameObject[] animatronics; // Assign Bonnie, Chica, Freddie in the inspector
    public Transform[] positions; // Assign the 10 positions in the inspector
    private List<int> initialPositions; // List for initial three positions
    private List<int> remainingPositions; // List for remaining seven positions
    private float appearanceDelay = 70f; // Time before animatronics start appearing
    public GameObject bonnieScreamerVideo;  // Reference to Bonnie's screamer video
    public GameObject chicaScreamerVideo;   // Reference to Chica's screamer video
    public GameObject freddieScreamerVideo; // Reference to Freddie's screamer video
    private GameObject currentAnimatronic;  // Track which animatronic is active
    public float stayTime = 0f;             // Time animatronic stays in trigger
    public bool inTrigger = false;          // Whether the animatronic is in the trigger area

    private float timer = 2.5f;

    void Update()
    {
        // Check if the animatronic is in the trigger
        if (inTrigger)
        {
            if (Input.GetKeyDown(KeyCode.L))
                ScareAudioSource.PlayOneShot(ScareSound);
            if (Input.GetKeyDown(KeyCode.K))
                ScareAudioSource.PlayOneShot(ScareSound);
            stayTime += Time.deltaTime;  // Increment stay time
            if (stayTime > 5f && PowerManager.isDoorOpen) // Check conditions
            {
                TriggerGameOver();  // Call game over method
            }
        }
        else
        {
            stayTime = 0f;  // Reset stay time if not in trigger
        }
    }

    void Start()
    {
        // Initialize initial positions for the first appearance
        initialPositions = new List<int> { 0, 1, 2 }; // First three positions (0, 1, 2)

        // Initialize remaining positions for later appearances (3-9)
        remainingPositions = new List<int>();
        for (int i = 3; i < positions.Length; i++)
        {
            remainingPositions.Add(i);
        }

        // Start the coroutine to manage animatronic appearances
        StartCoroutine(HandleAnimatronicAppearances());
    }

    private IEnumerator HandleAnimatronicAppearances()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(appearanceDelay);

        // Randomly select one of the animatronics to appear in one of the first three positions
        int randomAnimatronicIndex = Random.Range(0, animatronics.Length);
        int randomInitialPositionIndex = Random.Range(0, initialPositions.Count);
        SpawnAnimatronic(randomAnimatronicIndex, initialPositions[randomInitialPositionIndex]);

        // Allow the other two animatronics to start appearing in the first three positions
        for (int i = 0; i < animatronics.Length; i++)
        {
            if (i != randomAnimatronicIndex) // For the other animatronics
            {
                StartCoroutine(AnimatronicAppearanceRoutine(i, 3)); // They can appear in the first three positions
            }
        }

        // After initial appearances, allow all animatronics to start appearing in remaining positions
        yield return new WaitForSeconds(50f); // Wait before enabling remaining positions

        while (true) // Continuously allow appearances
        {
            foreach (var animatronic in animatronics)
            {
                int animatronicIndex = System.Array.IndexOf(animatronics, animatronic); // Get the index directly
                int randomPositionIndex = Random.Range(0, remainingPositions.Count);
                SpawnAnimatronic(animatronicIndex, remainingPositions[randomPositionIndex]);
            }

            yield return new WaitForSeconds(40f); // Time between subsequent appearances
        }
    }

    private IEnumerator AnimatronicAppearanceRoutine(int animatronicIndex, int appearances)
    {
        for (int i = 0; i < appearances; i++)
        {
            yield return new WaitForSeconds(Random.Range(40f, 50f)); // Random delay between appearances
            int randomInitialPositionIndex = Random.Range(0, initialPositions.Count);
            SpawnAnimatronic(animatronicIndex, initialPositions[randomInitialPositionIndex]);
        }
    }

    private void SpawnAnimatronic(int animatronicIndex, int positionIndex)
    {
        // Ensure animatronic is active and set its position
        GameObject animatronic = animatronics[animatronicIndex];
        animatronic.transform.position = positions[positionIndex].position;

        if (movementAudioSource != null && movementSound != null)
        {
            movementAudioSource.PlayOneShot(movementSound);
        }


        // Track the current animatronic
        currentAnimatronic = animatronic;

        // Optionally activate the animatronic if it's disabled
        animatronic.SetActive(true); // Ensure it's active to be seen
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming the player has a "Player" tag
        {
            inTrigger = true;  // Player entered the trigger
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming the player has a "Player" tag
        {
            deepSoundAudioSource.PlayOneShot(deepSoundClip);
            inTrigger = false;  // Player exited the trigger
            stayTime = 0f;      // Reset stay time
        }
    }

    void TriggerGameOver()
    {
        if(timer>0)
        {
            timer -= Time.deltaTime;
        } 
        if(timer<0)
        {
            timer = 2.5f;
            SceneManager.LoadScene("GameOver");
        }

        // Check which animatronic triggered the game over and play the corresponding screamer video
        if (currentAnimatronic == animatronics[0]) // Bonnie
        {
            bonnieScreamerVideo.SetActive(true);
        }
        else if (currentAnimatronic == animatronics[1]) // Chica
        {
            chicaScreamerVideo.SetActive(true);
        }
        else if (currentAnimatronic == animatronics[2]) // Freddie
        {
            freddieScreamerVideo.SetActive(true);
        }

        // Add additional game over logic here
    }
}

