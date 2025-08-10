using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class ResolutionManager
{
    private static Resolution[] availableResolutions;
    private static int currentResolutionIndex;

    public static void Initialize(TMP_Dropdown resolutionDropdown, Toggle fullscreenToggle)
    {
        availableResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        currentResolutionIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution res = availableResolutions[i];
            string option = res.width + " x " + res.height;
            options.Add(option);

            if (res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("RES_INDEX", currentResolutionIndex);
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = PlayerPrefs.GetInt("FULLSCREEN", 1) == 1;

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        // Apply saved settings
        ApplySavedSettings();
    }

    private static void ApplySavedSettings()
    {
        int resIndex = PlayerPrefs.GetInt("RES_INDEX", currentResolutionIndex);
        bool fullscreen = PlayerPrefs.GetInt("FULLSCREEN", 1) == 1;

        Resolution res = availableResolutions[resIndex];
        Screen.SetResolution(res.width, res.height, fullscreen);
    }

    private static void SetResolution(int index)
    {
        Resolution res = availableResolutions[index];
        bool fullscreen = Screen.fullScreen;

        Screen.SetResolution(res.width, res.height, fullscreen);
        PlayerPrefs.SetInt("RES_INDEX", index);
    }

    private static void SetFullscreen(bool isFullscreen)
    {
        Resolution res = availableResolutions[PlayerPrefs.GetInt("RES_INDEX", currentResolutionIndex)];
        Screen.SetResolution(res.width, res.height, isFullscreen);
        PlayerPrefs.SetInt("FULLSCREEN", isFullscreen ? 1 : 0);
    }
}