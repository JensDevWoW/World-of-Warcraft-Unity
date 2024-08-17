using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class ActionButton : MonoBehaviour
{
    public KeyCode keybind = KeyCode.Space; // Set your default keybind here
    private RectTransform rectTransform;

    private Vector3 originalScale;
    private Unit localPlayerUnit;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    void Update()
    {
        if (Input.GetKeyDown(keybind))
        {
            CompressButton();
        }
        else if (Input.GetKeyUp(keybind))
        {
            ReleaseButton();
        }
    }

    // Compress the button down visually
    private void CompressButton()
    {
        rectTransform.localScale = originalScale * 0.9f;
    }

    // Release the button to its original size
    private void ReleaseButton()
    {
        rectTransform.localScale = originalScale;
    }
}
