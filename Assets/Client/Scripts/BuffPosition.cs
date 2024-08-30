using System.Collections.Generic;
using UnityEngine;

public class BuffPosition : MonoBehaviour
{
    public GameObject template;
    public int columns = 5;
    public float spacing = 5f;
    private List<GameObject> activeBuffs = new List<GameObject>();

    void Start()
    {
        if (template == null)
        {
            template = transform.Find("Template").gameObject;
        }

        template.SetActive(false);
    }

    void Update()
    {
        PositionBuffs();
    }

    public void AddBuff(GameObject newBuff)
    {
        activeBuffs.Add(newBuff);
        PositionBuffs();
    }

    public void RemoveBuff(GameObject buff)
    {
        if (activeBuffs.Contains(buff))
        {
            activeBuffs.Remove(buff);
            PositionBuffs();
        }
    }

    private void PositionBuffs()
    {
        int index = 0;
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            if (activeBuffs[i] == null)
            {
                activeBuffs.RemoveAt(i);
                i--;
                continue;
            }

            int row = index / columns;
            int col = index % columns;

            Vector3 newPosition = new Vector3(col * (template.GetComponent<RectTransform>().rect.width + spacing),
                                              -row * (template.GetComponent<RectTransform>().rect.height + spacing),
                                              0);

            activeBuffs[i].GetComponent<RectTransform>().anchoredPosition = newPosition;
            index++;
        }
    }
}
