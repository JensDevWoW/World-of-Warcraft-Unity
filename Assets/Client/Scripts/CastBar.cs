using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class CastBar : MonoBehaviour
{
    public Image bar;              // Reference to the Image component
    public Text spellNameText;     // Reference to the Text component for the spell name
    public GameObject castbar;

    private float duration;        // Duration of the spell cast
    private float timeRemaining;   // Time remaining for the cast to complete
    private bool isCasting;        // Whether a spell is currently being cast
    private Color originalColor;   // Original color of the cast bar

    void Start()
    {
        // Hide the cast bar at the start
        castbar.gameObject.SetActive(false);
        originalColor = bar.color; // Store the original color of the bar
    }

    void Update()
    {
        if (isCasting)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                bar.fillAmount = Mathf.Clamp01(1 - (timeRemaining / duration));
            }
            else
            {
                // Casting complete
                FinishCast();
            }
        }
    }

    // Method to start the cast bar with the provided duration and spell name
    public void StartCast(float castDuration, string spellName)
    {
        duration = castDuration;
        timeRemaining = castDuration;
        isCasting = true;

        // Update the UI elements
        bar.fillAmount = 0;
        spellNameText.text = spellName;

        // Show the cast bar
        castbar.gameObject.SetActive(true);
    }

    // Method to finish the cast and hide the bar
    public void FinishCast()
    {
        isCasting = false;
        castbar.gameObject.SetActive(false);
    }

    // Method to cancel a cast
    public void CancelCast()
    {
        isCasting = false;
        FinishCast();
    }

    // Method to indicate a failed cast
    public void CastFailed()
    {
        isCasting = false;
        StartCoroutine(FadeOutCastBar());
    }

    // Coroutine to fade the cast bar out after a failed cast
    private IEnumerator FadeOutCastBar()
    {
        // Set the cast bar color to red
        bar.color = Color.red;

        // Set the duration of the fade out
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        // While loop to fade out the bar
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            bar.color = new Color(bar.color.r, bar.color.g, bar.color.b, alpha);
            yield return null;
        }

        // After fading out, reset the bar and hide it
        bar.color = originalColor; // Reset to original color
        castbar.gameObject.SetActive(false);
    }
}
