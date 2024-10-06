using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateCharButton : MonoBehaviour
{
    // Start is called before the first frame update
    public Button button;
    public CanvasGroup select;
    public CanvasGroup create;
    void Start()
    {
        button.GetComponent<Button>().onClick.AddListener(onCreateCharButtonClicked);
    }

    // Function to show a canvas
    private void ShowCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1; // Make it visible
        canvasGroup.interactable = true; // Allow interaction
        canvasGroup.blocksRaycasts = true; // Block UI raycasts for clicks
    }

    // Function to hide a canvas
    private void HideCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0; // Make it invisible
        canvasGroup.interactable = false; // Disable interaction
        canvasGroup.blocksRaycasts = false; // Allow other UI elements to be clicked
    }

    private void onCreateCharButtonClicked()
    {
        if (select != null && create != null)
        {
            ShowCanvas(create);
            HideCanvas(select);
            return;
        }
    }
}
