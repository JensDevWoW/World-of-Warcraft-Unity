using UnityEngine;
using UnityEngine.EventSystems;

public class PanelHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Toggle the active state of the target panel
            targetPanel.SetActive(!targetPanel.activeSelf);
            Debug.Log("Target panel toggled.");
        }
    }
}
