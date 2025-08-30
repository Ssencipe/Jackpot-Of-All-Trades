using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameOptionsMenu : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    [Header("Dev Mode")]
    public Toggle devModeToggle;

    [Header("CRT Effect")]
    public Toggle crtToggle;
    public CRTManager crtManager;

    [Header("Display Settings")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Game Speed")]
    public Slider gameSpeedSlider;

    private void Start()
    {
        InitializeAudio();
        InitializeDevMode();
        InitializeCRT();
        InitializeDisplaySettings();
        InitializeGameSpeed();
    }

    private void InitializeAudio()
    {
        masterSlider.value = AudioSettings.GetMasterVolume();
        musicSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.Music);
        sfxSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.SFX);
        uiSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.UI);

        masterSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetMasterVolume(val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
        });

        musicSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.Music, val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
        });

        sfxSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.SFX, val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
        });

        uiSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.UI, val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
        });
    }

    private void InitializeDevMode()
    {
        devModeToggle.isOn = DevSettings.IsDevMode;
        devModeToggle.onValueChanged.AddListener(SetDevMode);
    }

    private void InitializeCRT()
    {
        if (crtManager != null && crtToggle != null)
        {
            crtToggle.isOn = PlayerPrefs.GetInt("CRT_ENABLED", 1) == 1;
            crtToggle.onValueChanged.AddListener(SetCRTEffect);
        }
    }

    private void InitializeDisplaySettings()
    {
        ResolutionManager.Initialize(resolutionDropdown, fullscreenToggle);

        resolutionDropdown.onValueChanged.AddListener(index =>
        {
            ResolutionManager.ApplyResolution(index);
            PlayerPrefs.Save();
        });

        fullscreenToggle.onValueChanged.AddListener(isFullscreen =>
        {
            ResolutionManager.SetFullscreenPublic(isFullscreen);
            PlayerPrefs.Save();
        });
    }

    private void SetDevMode(bool value)
    {
        AudioManager.Instance.PlaySFX("select");
        DevSettings.IsDevMode = value;
    }

    private void SetCRTEffect(bool isEnabled)
    {
        AudioManager.Instance.PlaySFX("select");
        if (crtManager != null)
        {
            crtManager.SetCRTEnabled(isEnabled);
            PlayerPrefs.Save();
        }
    }

    private void InitializeGameSpeed()
    {
        float savedSpeed = PlayerPrefs.GetFloat("GAME_SPEED", 1f);
        gameSpeedSlider.value = savedSpeed;
        Time.timeScale = savedSpeed;

        gameSpeedSlider.onValueChanged.AddListener(val =>
        {
            Time.timeScale = val;
            PlayerPrefs.SetFloat("GAME_SPEED", val);
            PlayerPrefs.Save();
        });
    }
}