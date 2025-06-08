using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CameraManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Audio Settings")]
    public AudioSource closeSoundAudioSource;
    public AudioClip closeSoundClip;
    public AudioSource switchSoundAudioSource;
    public AudioClip switchSoundClip;
    public AudioSource musicAudioSource;
    public AudioClip kitchenMusicClip;
    public AudioSource dvdAudioSource;
    public AudioClip dvdMusicClip;
    public AudioSource cameraAudioSource;
    public AudioClip cameraOpenSound;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public Camera[] securityCameras;
    public TMP_Text cameraLabelText;
    public GameObject cameraUI;
    public GameObject openCameraButton;
    public GameObject closeCameraButton;
    public Button[] cameraButtons;
    public Color normalButtonColor = Color.white;
    public Color highlightedButtonColor = Color.green;
    public GameObject openCameraEffect;
    public GameObject closeCameraEffect;
    public GameObject cameraSwitchEffect;

    [Header("Auto Movement Settings")]
    public float autoMoveSpeed = 10f;
    public float autoMoveRange = 30f;
    public float autoMoveDelay = 3f;

    private bool isCameraSystemActive = false;
    private int currentCameraIndex = -1;
    private int lastActiveCameraIndex = 0;
    private bool isHovering = false;
    public float hoverTimeToOpen = 1.0f;
    private Coroutine openCoroutine;
    private float autoMoveTimer;
    private bool isMovingRight = true;
    private FNaFCameraController fnafCameraController;
    private bool returnToLastCamera = false;

    private readonly string[] cameraNames = {
        "Show Stage", "Dining Area", "Pirate Cove", "Backstage", "Restrooms",
        "Supply Closet", "Kitchen", "West Hall", "East Hall", "W. Hall Corner", "E. Hall Corner"
    };

    void Start()
    {
        fnafCameraController = FindObjectOfType<FNaFCameraController>();
        
        foreach (Camera cam in securityCameras)
        {
            if (cam != null) cam.gameObject.SetActive(false);
        }

        ActivateMainCamera();

        if (cameraUI != null) cameraUI.SetActive(false);
        if (openCameraButton != null) openCameraButton.SetActive(true);
        if (closeCameraButton != null) closeCameraButton.SetActive(false);
        if (openCameraEffect != null) openCameraEffect.SetActive(false);
        if (closeCameraEffect != null) closeCameraEffect.SetActive(false);
        if (cameraSwitchEffect != null) cameraSwitchEffect.SetActive(false);

        ResetCameraButtons();
    }

    void Update()
    {
        if (isCameraSystemActive)
        {
            if (openCameraButton != null) openCameraButton.SetActive(false);
            if (closeCameraButton != null) closeCameraButton.SetActive(true);

            if (currentCameraIndex == 0 && securityCameras.Length > 0 && securityCameras[0] != null)
            {
                UpdateAutoCameraMovement();
            }

            // Добавляем проверку нажатия клавиши для возврата к последней камере
            if (Input.GetKeyDown(KeyCode.R) && !returnToLastCamera)
            {
                ReturnToLastCamera();
            }
        }
        else
        {
            if (openCameraButton != null) openCameraButton.SetActive(true);
            if (closeCameraButton != null) closeCameraButton.SetActive(false);
            if (cameraUI != null) cameraUI.SetActive(false);
        }
    }

    // Новый метод для возврата к последней камере
    public void ReturnToLastCamera()
    {
        if (!isCameraSystemActive || currentCameraIndex == lastActiveCameraIndex) return;

        returnToLastCamera = true;
        ActivateSecurityCamera(lastActiveCameraIndex);
        returnToLastCamera = false;
    }

    public void HandleCameraButtonClick(int cameraIndex)
    {
        if (!isCameraSystemActive) return;

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        ActivateSecurityCamera(cameraIndex);
    }

    private void UpdateAutoCameraMovement()
    {
        if (securityCameras.Length == 0 || securityCameras[0] == null) return;

        autoMoveTimer += Time.deltaTime;

        if (autoMoveTimer >= autoMoveDelay)
        {
            float targetRotation = isMovingRight ? autoMoveRange : -autoMoveRange;
            float currentRotation = securityCameras[0].transform.localEulerAngles.y;
            currentRotation = currentRotation > 180 ? currentRotation - 360 : currentRotation;

            securityCameras[0].transform.localEulerAngles = new Vector3(0,
                Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * autoMoveSpeed), 0);

            if (Mathf.Abs(currentRotation - targetRotation) < 1f)
            {
                isMovingRight = !isMovingRight;
                autoMoveTimer = 0f;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCameraSystemActive)
        {
            isHovering = true;
            openCoroutine = StartCoroutine(OpenAfterDelay());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (openCoroutine != null)
        {
            StopCoroutine(openCoroutine);
            openCoroutine = null;
        }
    }

    private IEnumerator OpenAfterDelay()
    {
        yield return new WaitForSeconds(hoverTimeToOpen);
        if (isHovering && !isCameraSystemActive)
        {
            OpenCameraSystem();
        }
    }

    private void OpenCameraSystem()
    {
        StartCoroutine(PlayOpenCameraEffectAndShowSystem());
    }

    private IEnumerator PlayOpenCameraEffectAndShowSystem()
    {
        if (fnafCameraController != null)
            fnafCameraController.enabled = false;

        if (openCameraEffect != null) openCameraEffect.SetActive(true);
        if (cameraAudioSource != null && cameraOpenSound != null)
            cameraAudioSource.PlayOneShot(cameraOpenSound);

        yield return new WaitForSeconds(0.5f);

        if (openCameraEffect != null) openCameraEffect.SetActive(false);
        isCameraSystemActive = true;
        if (cameraUI != null) cameraUI.SetActive(true);
        ActivateSecurityCamera(lastActiveCameraIndex);
    }

    public void CloseCameraSystem()
    {
        StartCoroutine(PlayCloseCameraEffectAndHideSystem());
    }

    private IEnumerator PlayCloseCameraEffectAndHideSystem()
    {
        if (cameraUI != null) cameraUI.SetActive(false);
        isCameraSystemActive = false;

        if (closeSoundAudioSource != null && closeSoundClip != null)
            closeSoundAudioSource.PlayOneShot(closeSoundClip);

        if (currentCameraIndex >= 0)
        {
            lastActiveCameraIndex = currentCameraIndex;
        }

        ActivateMainCamera();

        if (closeCameraEffect != null) closeCameraEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        if (closeCameraEffect != null) closeCameraEffect.SetActive(false);

        if (fnafCameraController != null)
            fnafCameraController.enabled = true;
    }

    public void ActivateSecurityCamera(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= securityCameras.Length)
        {
            Debug.LogWarning($"Invalid camera index: {cameraIndex}");
            return;
        }

        if (cameraIndex != currentCameraIndex)
        {
            if (cameraIndex != 0 && !returnToLastCamera)
            {
                StartCoroutine(PlaySwitchEffectAndActivateCamera(cameraIndex));
            }
            else
            {
                ActivateCameraImmediately(cameraIndex);
            }

            if (dvdAudioSource != null)
            {
                if (cameraIndex == 0 && dvdMusicClip != null)
                {
                    dvdAudioSource.clip = dvdMusicClip;
                    dvdAudioSource.Play();
                }
                else
                {
                    dvdAudioSource.Stop();
                }
            }

            if (musicAudioSource != null)
            {
                if (cameraIndex == 6 && kitchenMusicClip != null)
                {
                    musicAudioSource.clip = kitchenMusicClip;
                    musicAudioSource.Play();
                }
                else
                {
                    musicAudioSource.Stop();
                }
            }
        }
    }

    private IEnumerator PlaySwitchEffectAndActivateCamera(int cameraIndex)
    {
        if (cameraSwitchEffect != null) cameraSwitchEffect.SetActive(true);
        PlayCameraSwitchSound();

        yield return new WaitForSeconds(0.5f);

        if (cameraSwitchEffect != null) cameraSwitchEffect.SetActive(false);
        ActivateCameraImmediately(cameraIndex);
    }

    private void ActivateCameraImmediately(int cameraIndex)
    {
        if (cameraIndex < 0 || cameraIndex >= securityCameras.Length) return;

        foreach (Camera cam in securityCameras)
        {
            if (cam != null) cam.gameObject.SetActive(false);
        }

        if (securityCameras[cameraIndex] != null)
        {
            securityCameras[cameraIndex].gameObject.SetActive(true);
            if (mainCamera != null) mainCamera.gameObject.SetActive(false);

            currentCameraIndex = cameraIndex;
            UpdateCameraLabel(cameraIndex);
            HighlightSelectedCamera(cameraIndex);

            autoMoveTimer = 0f;
            isMovingRight = true;

            foreach (Camera cam in securityCameras)
            {
                if (cam != null) cam.transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    public void ActivateMainCamera()
    {
        foreach (Camera cam in securityCameras)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
                cam.transform.localEulerAngles = Vector3.zero;
            }
        }

        if (mainCamera != null) mainCamera.gameObject.SetActive(true);

        currentCameraIndex = -1;
        if (cameraLabelText != null) cameraLabelText.text = "Security Office";

        ResetCameraButtons();

        if (musicAudioSource != null) musicAudioSource.Stop();
        if (dvdAudioSource != null) dvdAudioSource.Stop();
    }

    void UpdateCameraLabel(int cameraIndex)
    {
        if (cameraLabelText == null) return;

        if (cameraIndex >= 0 && cameraIndex < cameraNames.Length)
        {
            cameraLabelText.text = cameraNames[cameraIndex];
        }
        else
        {
            cameraLabelText.text = "Unknown Camera";
        }
    }

    void ResetCameraButtons()
    {
        if (cameraButtons == null) return;

        foreach (Button button in cameraButtons)
        {
            if (button != null)
            {
                Image img = button.GetComponent<Image>();
                if (img != null) img.color = normalButtonColor;
            }
        }
    }

    void HighlightSelectedCamera(int cameraIndex)
    {
        if (cameraButtons == null || cameraIndex < 0 || cameraIndex >= cameraButtons.Length) return;

        ResetCameraButtons();

        if (cameraButtons[cameraIndex] != null)
        {
            Image img = cameraButtons[cameraIndex].GetComponent<Image>();
            if (img != null) img.color = highlightedButtonColor;
        }
    }

    private void PlayCameraSwitchSound()
    {
        if (switchSoundAudioSource != null && switchSoundClip != null)
        {
            switchSoundAudioSource.PlayOneShot(switchSoundClip);
        }
    }
}