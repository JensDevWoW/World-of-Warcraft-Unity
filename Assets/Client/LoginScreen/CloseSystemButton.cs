using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CloseSystemButton : MonoBehaviour
{
    public GameObject panel;
    public Button button;
    void Start()
    {
        button.onClick.AddListener(Close);
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}
