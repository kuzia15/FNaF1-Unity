using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PowerManager : MonoBehaviour
{
    public AudioClip doorSound;       // Sound for both door open and close
    public AudioSource audioSource;  // Audio source component to play sounds

    public AudioClip lightSound;       // Sound for both door open and close
    public AudioSource audioSource2;  // Audio source component to play sounds

    public float totalPower = 100f;           // Starting power at 100%
    private float passiveDrainRate = 0.2f;    // Passive power drain rate per second
    private float lightDrainRate = 0.5f;       // Drain rate when lights are active
    private float doorDrainRate = 0.5f;        // Drain rate when doors are closed

    public GameObject freddieScreamerVideo;
    private float timer = 2.5f;

    public Image batteryImage;                // Reference to the battery image
    public TMP_Text powerText;                    // Reference to the battery percentage text
    public Sprite[] batteryLevels;            // Array of battery icons (full, 75%, 50%, 25%, empty)

    public GameObject[] lights;               // Array of light GameObjects
    public GameObject[] doors;                // Array of door GameObjects (to animate)

    private bool[] lightsOn;                  // Track whether each light is on
    private bool[] doorsClosed;               // Track whether each door is closed

    // Animation parameters
    public float doorOpenSpeed = 2.0f;        // Speed at which the door opens/closes
    private Vector3[] doorClosedPositions;    // Closed positions for doors
    private Vector3[] doorOpenPositions;      // Open positions for doors

    public static bool isDoorOpen = true;

    void Start()
    {

        audioSource2 = GetComponent<AudioSource>();

        // Assign the door sound to the AudioSource
        audioSource.clip = doorSound;
        audioSource2.clip = lightSound;

        // Initialize lights and doors states
        lightsOn = new bool[lights.Length];
        doorsClosed = new bool[doors.Length];

        // Set the open/closed positions for the doors
        doorClosedPositions = new Vector3[doors.Length];
        doorOpenPositions = new Vector3[doors.Length];

        // Define the Y positions for the doors
        doorClosedPositions[0] = new Vector3(doors[0].transform.position.x, 27.6f, doors[0].transform.position.z);
        doorOpenPositions[0] = new Vector3(doors[0].transform.position.x, 31.1f, doors[0].transform.position.z);

        // Set the positions for the second door (adjust the Y values as needed)
        doorClosedPositions[1] = new Vector3(doors[1].transform.position.x, 27.6f, doors[1].transform.position.z);
        doorOpenPositions[1] = new Vector3(doors[1].transform.position.x, 31.1f, doors[1].transform.position.z);
    }

    void Update()
    {
        // Drain power over time
        DrainPower();

        // Update the battery UI elements
        UpdateBatteryUI();

        // Check for game over
        if (totalPower <= 0)
        {
            TriggerGameOver();
        }

        // Example Input for toggling lights and doors (replace with UI buttons if needed)
        if (Input.GetKeyDown(KeyCode.L)) ToggleLight(0);  // Toggle first light
        if (Input.GetKeyDown(KeyCode.K)) ToggleLight(1); // Toggle second light
        if (Input.GetKeyDown(KeyCode.D)) ToggleDoor(0);   // Toggle first door
        if (Input.GetKeyDown(KeyCode.A)) ToggleDoor(1);  // Toggle second door
    }

    void DrainPower()
    {
        // Reduce power passively over time
        totalPower -= passiveDrainRate * Time.deltaTime;

        // Check if any lights are on and drain power
        for (int i = 0; i < lightsOn.Length; i++)
        {
            if (lightsOn[i])
            {
                totalPower -= lightDrainRate * Time.deltaTime;
            }
        }

        // Check if any doors are closed and drain power
        for (int i = 0; i < doorsClosed.Length; i++)
        {
            if (doorsClosed[i])
            {
                totalPower -= doorDrainRate * Time.deltaTime;
            }
        }

        // Make sure power doesn't go below 0
        totalPower = Mathf.Clamp(totalPower, 0, 100);
    }

    void UpdateBatteryUI()
    {
        // Update the power text
        powerText.text = Mathf.RoundToInt(totalPower).ToString() + "%";

        // Update the battery icon based on power level
        if (totalPower > 75)
        {
            batteryImage.sprite = batteryLevels[0];  // Full battery
        }
        else if (totalPower > 50)
        {
            batteryImage.sprite = batteryLevels[1];  // 75% battery
        }
        else if (totalPower > 25)
        {
            batteryImage.sprite = batteryLevels[2];  // 50% battery
        }
        else if (totalPower > 0)
        {
            batteryImage.sprite = batteryLevels[3];  // 25% battery
        }
        else
        {
            batteryImage.sprite = batteryLevels[4];  // Empty battery
        }
    }

    void TriggerGameOver()
    {
        freddieScreamerVideo.SetActive(true);
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0)
        {
            timer = 2.5f;
            SceneManager.LoadScene("GameOver");
        }
    }

    // Toggle light on/off
    public void ToggleLight(int lightIndex)
    {
        if (lightIndex >= 0 && lightIndex < lights.Length)
        {
            // Toggle the light state
            lightsOn[lightIndex] = !lightsOn[lightIndex];  // Toggle state
            lights[lightIndex].SetActive(lightsOn[lightIndex]);  // Enable/disable light GameObject

            // Play or stop the light sound based on the light state
            if (lightsOn[lightIndex])
            {
                // Play the light sound when the light is turned on
                audioSource2.clip = lightSound;
                audioSource2.Play();
            }
            else
            {
                // Stop the light sound when the light is turned off
                audioSource2.Stop();
            }
        }
    }

    public void ToggleDoor(int doorIndex)
{
    Debug.Log($"Дверь {doorIndex} нажата!");
    
    doorsClosed[doorIndex] = !doorsClosed[doorIndex];
    isDoorOpen = !doorsClosed[doorIndex];

    audioSource.Play();  // Проверь, есть ли звук

    if (doorsClosed[doorIndex])
    {
        StartCoroutine(MoveDoor(doors[doorIndex].transform, doorOpenPositions[doorIndex], doorClosedPositions[doorIndex]));
    }
    else
    {
        StartCoroutine(MoveDoor(doors[doorIndex].transform, doorClosedPositions[doorIndex], doorOpenPositions[doorIndex]));
    }
}

    // Coroutine to smoothly move the door
    IEnumerator MoveDoor(Transform door, Vector3 fromPosition, Vector3 toPosition)
    {
        float elapsedTime = 0;
        float duration = Vector3.Distance(fromPosition, toPosition) / doorOpenSpeed;

        while (elapsedTime < duration)
        {
            // Lerp between the positions
            door.position = Vector3.Lerp(fromPosition, toPosition, elapsedTime / duration);

            // Increase the elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure the door ends exactly at the target position
        door.position = toPosition;
    }
}