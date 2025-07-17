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
    }

    private void SetDevMode(bool value)
    {
        DevSettings.IsDevMode = value;
    }
}