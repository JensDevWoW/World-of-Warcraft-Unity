using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class BuffDebuff : MonoBehaviour
{
    public int spellId;
    public Image buffIcon;
    public float timer;
    public TMP_Text textBox;

    private float currentTime;

    private void Start()
    {
        currentTime = timer;
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
                Destroy(gameObject);
            UpdateTextBox();
        }
    }

    private void UpdateTextBox()
    {
        if (currentTime < 60)
            textBox.text = Mathf.Ceil(currentTime).ToString() + "s";
        else if (currentTime < 3600)
            textBox.text = Mathf.Floor(currentTime / 60).ToString() + "m";
        else
            textBox.text = Mathf.Floor(currentTime / 3600).ToString() + "h";
    }

    public void UpdateData(float duration, int stacks)
    {
        if (duration > 0)
        {
            this.timer = duration;
            currentTime = duration;
            UpdateTextBox();
        }
        else
            Destroy(gameObject);
    }

    public void InitializeBuff(int spellId, Sprite icon, float duration)
    {
        this.spellId = spellId;
        this.buffIcon.sprite = icon;
        this.timer = duration;
        currentTime = duration;
        UpdateTextBox();
    }
}
