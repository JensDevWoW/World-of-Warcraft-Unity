using UnityEngine;

public class CameraBezierMovement : MonoBehaviour
{
    public static CameraBezierMovement Instance;  // Static instance

    public Vector3 startPosition = new Vector3(0.139f, 6.401f, -9f);
    public Vector3 startRotation = new Vector3(0f, 0f, 0f);

    public Vector3 endPosition = new Vector3(0.139f, 6.085f, -8.02f);
    public Vector3 endRotation = new Vector3(2.855f, 0f, 0f);

    public Vector3 controlPoint1 = new Vector3(0.139f, 6.5f, -9f);  // First control point
    public Vector3 controlPoint2 = new Vector3(0.139f, 6.2f, -8.5f); // Second control point

    public float duration = 2.0f;  // Duration of the movement

    private float timeElapsed;
    private bool isMoving = false;  // Flag to start/stop movement
    private Quaternion startQuat;
    private Quaternion endQuat;

    void Awake()
    {
        // Setup static instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialize camera's initial position and rotation
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(startRotation);

        // Convert start and end rotations to quaternions
        startQuat = Quaternion.Euler(startRotation);
        endQuat = Quaternion.Euler(endRotation);
    }

    void Update()
    {
        if (isMoving)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            // Apply easing out (ease-out cubic: t = 1 - (1 - t)^3)
            t = EaseOutCubic(t);

            // Ensure movement stays within the duration range
            if (t >= 1.0f)
            {
                t = 1.0f;
                isMoving = false;
            }

            // Update position and rotation using Bezier and SLERP
            Vector3 bezierPosition = GetBezierPoint(t, startPosition, controlPoint1, controlPoint2, endPosition);
            transform.position = bezierPosition;

            Quaternion bezierRotation = Quaternion.Slerp(startQuat, endQuat, t);
            transform.rotation = bezierRotation;
        }
    }

    // Public method to start the camera movement
    public void StartCameraMovement()
    {
        isMoving = true;
        timeElapsed = 0.0f;
    }

    // Function to calculate a point on a cubic Bézier curve
    Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 point = uuu * p0; // First term
        point += 3 * uu * t * p1; // Second term
        point += 3 * u * tt * p2; // Third term
        point += ttt * p3;        // Fourth term

        return point;
    }

    // Ease-out cubic function to slow down the movement at the end
    float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
}
