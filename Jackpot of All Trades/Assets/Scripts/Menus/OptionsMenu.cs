using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    //audio
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider uiSlider;

    //dev mode
    public Toggle devModeToggle;

    //CRT effects
    public Toggle crtToggle;
    public CRTManager crtManager;

    //Display settings
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Game Speed")]
    public Slider gameSpeedSlider;

    private void Start()
    {
        // Initialize volume sliders with saved values
        masterSlider.value = AudioSettings.GetMasterVolume();
        musicSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.Music);
        sfxSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.SFX);
        uiSlider.value = AudioSettings.GetRawCategoryVolume(AudioCategory.UI);

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
        });
        musicSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.Music, val);
            AudioManager.Instance.RefreshVolumes();
        });
        sfxSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.SFX, val);
            AudioManager.Instance.RefreshVolumes();
        });
        uiSlider.onValueChanged.AddListener(val =>
        {
            AudioSettings.SetVolume(AudioCategory.UI, val);
            AudioManager.Instance.RefreshVolumes();
        });

        InitializeGameSpeed();
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
        });

    }
}