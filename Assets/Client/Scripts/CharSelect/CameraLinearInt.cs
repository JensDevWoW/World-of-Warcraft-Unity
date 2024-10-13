using UnityEngine;

public class LinIntCam : MonoBehaviour
{
    public static LinIntCam Instance;  // Static instance

    private bool isMoving = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 endPosition;
    private Quaternion endRotation;
    private float duration;
    private float timeElapsed;

    // Set up the static instance in Awake
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one instance of this script
        }
    }

    // Start the movement with given start and end positions, rotations, and speed (duration)
    public void StartLinearInterpolation(Vector3 startPos, Vector3 endPos, Vector3 startRot, Vector3 endRot, float speed)
    {
        // Initialize values
        startPosition = startPos;
        startRotation = Quaternion.Euler(startRot);
        endPosition = endPos;
        endRotation = Quaternion.Euler(endRot);
        duration = speed;
        timeElapsed = 0.0f;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            // Apply ease-out cubic to slow down at the end
            t = EaseOutCubic(t);

            // Ensure the interpolation finishes cleanly
            if (t >= 1.0f)
            {
                t = 1.0f;
                isMoving = false;
            }

            // Lerp for position
            transform.position = Vector3.Lerp(startPosition, endPosition, t);

            // Slerp for rotation (using spherical interpolation for smooth rotation)
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
        }
    }

    // Ease-out cubic function to slow down the movement at the end
    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
