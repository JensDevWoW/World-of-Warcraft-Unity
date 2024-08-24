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
        currentTime = timer; // Initialize the timer
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                // Timer has finished, remove the buff/debuff
                Destroy(gameObject);
            }
            textBox.text = Mathf.Ceil(currentTime).ToString();
        }
    }

    public void UpdateData(float duration, int stacks)
    {
        if (duration > 0)
        {
            this.timer = duration;
            currentTime = duration;
            textBox.text = Mathf.Ceil(duration).ToString();
        }
        else
            Destroy(gameObject);
    }

    // This function allows setting up the buff/debuff with the icon and duration
    public void InitializeBuff(int spellId, Sprite icon, float duration)
    {
        this.spellId = spellId;
        this.buffIcon.sprite = icon;
        this.timer = duration;
        currentTime = duration;
        textBox.text = Mathf.Ceil(duration).ToString();
    }
}
