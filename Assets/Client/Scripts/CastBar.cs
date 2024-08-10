using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    public Image bar;              // Reference to the Slider component
    public Text spellNameText;      // Reference to the Text component for the spell name
    public GameObject castbar;

    private float duration;         // Duration of the spell cast
    private float timeRemaining;    // Time remaining for the cast to complete
    private bool isCasting;         // Whether a spell is currently being cast

    void Start()
    {
        // Hide the cast bar at the start
        castbar.gameObject.SetActive(false);
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

    // Optionally, call this method to cancel a cast
    public void CancelCast()
    {
        isCasting = false;
        FinishCast();
    }
}
