using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This is the options menu specifically in the main menu and not in game after pausing. Kept separate in case major settings that should not be changed in game need to be kept elsewhere. Tied to MainMenu script.

public class OptionsMenu : MonoBehaviour
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
        // Initialize volume sliders with saved values
        masterSlider.value = AudioSettings.GetMasterVolume();
        musicSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.Music);
        sfxSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.SFX);
        uiSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.UI);

        masterLabel.text = $"{masterSlider.value:F2}";
        musicLabel.text = $"{musicSlider.value:F2}";
        sfxLabel.text = $"{sfxSlider.value:F2}";
        uiLabel.text = $"{uiSlider.value:F2}";

        // Dev Mode setup
        devModeToggle.isOn = DevSettings.IsDevMode;
        devModeToggle.onValueChanged.AddListener(SetDevMode);

        // CRT Toggle Setup
        if (crtManager != null && crtToggle != null)
        {
            crtToggle.isOn = PlayerPrefs.GetInt("CRT_ENABLED", 1) == 1;
            crtToggle.onValueChanged.AddListener(SetCRTEffect);
        }

        // DISPLAY SETTINGS
        ResolutionManager.Initialize(resolutionDropdown, fullscreenToggle);

        // Add listeners to update settings
        masterSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetMasterVolume(val);
            AudioManager.Instance.RefreshVolumes();
            if (masterLabel != null) masterLabel.text = $"{val:F2}";
        });
        musicSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.Music, val);
            AudioManager.Instance.RefreshVolumes();
            if (musicLabel != null) musicLabel.text = $"{val:F2}";
        });
        sfxSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.SFX, val);
            AudioManager.Instance.RefreshVolumes();
            if (sfxLabel != null) sfxLabel.text = $"{val:F2}";
        });
        uiSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.UI, val);
            AudioManager.Instance.RefreshVolumes();
            if (uiLabel != null) uiLabel.text = $"{val:F2}";
        });

        InitializeGameSpeed();
        gameSpeedLabel.text = $"{gameSpeedSlider.value:F2}";
    }

    private void SetDevMode(bool value)
    {
        DevSettings.IsDevMode = value;
    }

    private void SetCRTEffect(bool isEnabled)
    {
        if (crtManager != null)
        {
            crtManager.SetCRTEnabled(isEnabled);
        }
    }

    private void InitializeGameSpeed()
    {
        float savedSpeed = PlayerPrefs.GetFloat("GAME_SPEED", 1f);
        gameSpeedSlider.value = savedSpeed;
        GameSpeedManager.CurrentSpeed = savedSpeed;
        gameSpeedSlider.value = savedSpeed;

        gameSpeedSlider.onValueChanged.AddListener(val =>
        {
            GameSpeedManager.CurrentSpeed = val;

            if (Time.timeScale > 0f)
                Time.timeScale = val;

            PlayerPrefs.SetFloat("GAME_SPEED", val);
            PlayerPrefs.Save();

            if (gameSpeedLabel != null) gameSpeedLabel.text = $"{val:F2}";
        });
    }
}