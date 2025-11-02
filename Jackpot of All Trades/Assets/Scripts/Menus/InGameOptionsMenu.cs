using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Used for the options menu in game when pause menu is activated. It's kept distinct from the main menu options.

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

    [Header("Slider Labels")]
    public TextMeshProUGUI masterLabel;
    public TextMeshProUGUI musicLabel;
    public TextMeshProUGUI sfxLabel;
    public TextMeshProUGUI uiLabel;
    public TextMeshProUGUI gameSpeedLabel;

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

        masterLabel.text = $"{masterSlider.value:F2}";
        musicLabel.text = $"{musicSlider.value:F2}";
        sfxLabel.text = $"{sfxSlider.value:F2}";
        uiLabel.text = $"{uiSlider.value:F2}";

        masterSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetMasterVolume(val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
            masterLabel.text = $"{masterSlider.value:F2}";
        });

        musicSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.Music, val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
            musicLabel.text = $"{musicSlider.value:F2}";
        });

        sfxSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.SFX, val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
            sfxLabel.text = $"{sfxSlider.value:F2}";
        });

        uiSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.UI, val);
            PlayerPrefs.Save();
            AudioManager.Instance.RefreshVolumes();
            uiLabel.text = $"{uiSlider.value:F2}";
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

        gameSpeedLabel.text = $"{gameSpeedSlider.value:F2}";

        gameSpeedSlider.onValueChanged.AddListener(val =>
        {
            GameSpeedManager.CurrentSpeed = val;

            if (Time.timeScale > 0f)
                Time.timeScale = val;

            PlayerPrefs.SetFloat("GAME_SPEED", val);
            PlayerPrefs.Save();

            gameSpeedLabel.text = $"{gameSpeedSlider.value:F2}";
        });
    }
}