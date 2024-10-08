using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    public Image loadingBar;

    private float currentFill = 0f;
    private bool isLoading = true;

    void Start()
    {
        StartCoroutine(FillLoadingBar());
    }

    void Update()
    {
        if (isLoading)
        {
            loadingBar.fillAmount = currentFill;
        }
        else if (currentFill >= 1f)
        {
            LoadGame();
        }
    }

    IEnumerator FillLoadingBar()
    {
        while (currentFill < 1f)
        {
            currentFill += Random.Range(0.05f, 0.2f); // Random jump between 0.05 and 0.2
            currentFill = Mathf.Clamp01(currentFill);
            yield return new WaitForSeconds(Random.Range(0.5f, 1f)); // Random delay
        }
        isLoading = false; // Stop loading once we reach 1
    }

    void LoadGame()
    {
        CharacterSelectionManager.Instance.LoadingFinished();
    }
}
