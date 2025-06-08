using UnityEngine;

public class FNaFCameraController : MonoBehaviour
{
    public float maxRotationAngle = 45f;
    public float acceleration = 2f;
    public float deceleration = 4f;
    public float maxRotationSpeed = 100f;
    public float edgeThreshold = 0.15f;
    [Range(0, 1)] public float smoothing = 0.2f;
    public float deadZone = 0.05f;

    private float currentYRotation;
    private float initialYRotation;
    private float currentVelocity;
    private float targetDirection;
    private float smoothedDirection;

    private void Start()
    {
        initialYRotation = transform.eulerAngles.y;
        currentYRotation = initialYRotation;
    }

    private void Update()
    {
        float mouseX = Input.mousePosition.x / Screen.width;

        targetDirection = 0f;

        if (mouseX < edgeThreshold)
        {
            float edgePercent = 1 - (mouseX / edgeThreshold);
            targetDirection = -1 * edgePercent;
        }
        else if (mouseX > 1f - edgeThreshold)
        {
            float edgePercent = (mouseX - (1f - edgeThreshold)) / edgeThreshold;
            targetDirection = 1 * edgePercent;
        }

        smoothedDirection = Mathf.Lerp(smoothedDirection, targetDirection, 1f - smoothing);

        float targetSpeed = smoothedDirection * maxRotationSpeed;

        if (Mathf.Abs(targetSpeed) > Mathf.Abs(currentVelocity))
        {
            currentVelocity = Mathf.MoveTowards(
                currentVelocity,
                targetSpeed,
                acceleration * Time.deltaTime * maxRotationSpeed
            );
        }
        else
        {
            currentVelocity = Mathf.MoveTowards(
                currentVelocity,
                targetSpeed,
                deceleration * Time.deltaTime * maxRotationSpeed
            );
        }

        currentYRotation += currentVelocity * Time.deltaTime;

        currentYRotation = Mathf.Clamp(
            currentYRotation,
            initialYRotation - maxRotationAngle,
            initialYRotation + maxRotationAngle
        );

        if (Mathf.Abs(targetDirection) < deadZone && Mathf.Abs(currentVelocity) < 5f)
        {
            currentYRotation = Mathf.Lerp(
                currentYRotation,
                initialYRotation,
                deceleration * Time.deltaTime
            );
        }

        transform.eulerAngles = new Vector3(0, currentYRotation, 0);
    }
}