using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenSettingsManager : MonoBehaviour
{
    public TMP_Dropdown aspectRatioDropdown;
    public TMP_Dropdown screenModeDropdown;
    public Button system;
    public GameObject panel;

    private const string ScreenResolutionKey = "ScreenResolution";
    private const string ScreenModeKey = "ScreenMode";

    private void Start()
    {
        aspectRatioDropdown.onValueChanged.AddListener(OnAspectRatioChanged);
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        system.onClick.AddListener(OnClick);

        // Load saved screen settings
        LoadScreenSettings();
    }

    public void OnClick()
    {
        panel.gameObject.SetActive(true);
    }

    public void OnAspectRatioChanged(int index)
    {
        switch (index)
        {
            case 0: SetResolution(1920, 1080); break; // 16:9
            case 1: SetResolution(1024, 768); break;  // 4:3
            case 2: SetResolution(2560, 1080); break; // 21:9
            case 3: SetResolution(1080, 1080); break; // 1:1
        }

        // Save aspect ratio setting
        PlayerPrefs.SetInt(ScreenResolutionKey, index);
        PlayerPrefs.Save();
    }

    public void OnScreenModeChanged(int index)
    {
        switch (index)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }

        // Save screen mode setting
        PlayerPrefs.SetInt(ScreenModeKey, index);
        PlayerPrefs.Save();
    }

    private void SetResolution(int width, int height)
    {
        bool isFullScreen = Screen.fullScreen;
        Screen.SetResolution(width, height, isFullScreen);
    }

    private void LoadScreenSettings()
    {
        if (PlayerPrefs.HasKey(ScreenResolutionKey))
        {
            int resolutionIndex = PlayerPrefs.GetInt(ScreenResolutionKey);
            aspectRatioDropdown.value = resolutionIndex;
            OnAspectRatioChanged(resolutionIndex); // Apply saved resolution
        }

        if (PlayerPrefs.HasKey(ScreenModeKey))
        {
            int screenModeIndex = PlayerPrefs.GetInt(ScreenModeKey);
            screenModeDropdown.value = screenModeIndex;
            OnScreenModeChanged(screenModeIndex); // Apply saved screen mode
        }
    }
}
