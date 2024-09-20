using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClientDebug : MonoBehaviour
{
    public static ClientDebug Instance { get; private set; }

    public GameObject textPrefab;
    private Transform parentCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded event
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Called every time a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCanvas(); // Find the canvas for the new scene
    }

    // Call this whenever needed to refresh the canvas
    public void FindCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            parentCanvas = canvas.transform;
        }
        else
        {
            Debug.LogWarning("No Canvas found in the scene.");
        }
    }

    public void Log(string message, float duration = 3f)
    {
        if (textPrefab != null && parentCanvas != null)
        {
            GameObject textObj = Instantiate(textPrefab, parentCanvas);
            Text debugText = textObj.GetComponent<Text>();
            if (debugText != null)
            {
                debugText.text = message;
            }

            Destroy(textObj, duration); // Auto-destroy after duration
        }
        else
        {
            Debug.LogWarning("Text prefab or parentCanvas not assigned.");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe from event when destroyed
    }
}
