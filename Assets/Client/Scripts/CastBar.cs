using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CastBar : MonoBehaviour
{
    public Image bar;
    public Text spellNameText;
    public GameObject castbar;

    private float duration;
    private float timeRemaining;
    private bool isCasting;
    private bool isChanneling;
    private Color originalColor;

    void Start()
    {
        castbar.SetActive(false);
        originalColor = bar.color;
    }

    void Update()
    {
        if (isCasting || isChanneling)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                bar.fillAmount = isChanneling
                    ? Mathf.Clamp01(timeRemaining / duration) // Decrease for channeling
                    : Mathf.Clamp01(1 - (timeRemaining / duration)); // Increase for normal casting
            }
            else
            {
                FinishCast();
            }
        }
    }

    public void StartCast(float castDuration, string spellName)
    {
        duration = castDuration;
        timeRemaining = castDuration;
        isCasting = true;
        bar.fillAmount = 0;
        spellNameText.text = spellName;
        castbar.SetActive(true);
    }

    public void StartChannel(int spellId, float channelDuration)
    {
        SpellInfo spell = SpellContainer.Instance.GetSpellById(spellId);
        if (spell == null) return;

        duration = channelDuration;
        timeRemaining = channelDuration;
        isChanneling = true;
        bar.fillAmount = 1; // Start full for channeling
        spellNameText.text = spell.Name;
        castbar.SetActive(true);
    }

    public void FinishCast()
    {
        isCasting = false;
        isChanneling = false;
        castbar.SetActive(false);
    }

    public void CancelCast()
    {
        isCasting = false;
        isChanneling = false;
        FinishCast();
    }

    public void CastFailed()
    {
        isCasting = false;
        isChanneling = false;
        StartCoroutine(FadeOutCastBar());
    }

    private IEnumerator FadeOutCastBar()
    {
        bar.color = Color.red;
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, alpha);
            yield return null;
        }

        bar.color = originalColor;
        castbar.SetActive(false);
    }
}
