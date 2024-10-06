using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharCreationUI : MonoBehaviour
{
    public Button customize;
    public Button mage;
    public Button warlock;
    public Button rogue;
    public Button selected;
    public int selectedClassId = 0;

    public void Start()
    {

        mage.onClick.AddListener(() => AssignClass(mage, 1));
        warlock.onClick.AddListener(() => AssignClass(warlock, 2));
        rogue.onClick.AddListener(() => AssignClass(rogue, 3));

        customize.onClick.AddListener(() => Customize());
    }

    void Highlight(Button button)
    {
        button.GetComponent<Image>().color = new Color32(255, 255, 0, 255);
        selected = button;
    }

    void Unhighlight(Button button)
    {
        button.GetComponent<Image>().color = new Color32(255, 255, 255, 125);
    }

    void AssignClass(Button button, int classId)
    {
        selectedClassId = classId;
        Debug.Log("Selected Class ID: " + selectedClassId);
        if (selected != null)
            Unhighlight(selected);

        Highlight(button);
    }

    void Customize()
    {
        GameObject customizeCanvas = GameObject.FindWithTag("CharacterCustomizeCanvas");
        if (customizeCanvas != null)
        {
            gameObject.SetActive(false);
            customizeCanvas.SetActive(true);
            return;
        }
    }
}
