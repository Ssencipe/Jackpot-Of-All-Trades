using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;     // Parent panel (always active when paused)
    public GameObject optionsMenuUI;   // Child panel (enabled/disabled as needed)

    private bool isPaused = false;

    private float originalSFXVolume; //store SFX volume before disabling

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
            {
                TogglePause(); // Open pause menu if not paused
            }
            else if (optionsMenuUI.activeSelf)
            {
                ToggleOptionsMenu(); // Close options if open
            }
            else
            {
                TogglePause(); // Otherwise unpause
            }
        }
    }

    public void TogglePause()
    {
        AudioManager.Instance.PlaySFX("select");
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);

        // Ensure options panel is hidden when pause menu is hidden
        if (!isPaused && optionsMenuUI.activeSelf)
            optionsMenuUI.SetActive(false);

        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
            MuteAudio();
        else
            RestoreAudio();

        AudioManager.Instance.RefreshVolumes();
    }

    public void ToggleOptionsMenu()
    {
        AudioManager.Instance.PlaySFX("select");
        optionsMenuUI.SetActive(!optionsMenuUI.activeSelf);
    }

    public void ResumeGame()
    {
        AudioManager.Instance.PlaySFX("select");
        isPaused = false;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1;
        RestoreAudio();
        AudioManager.Instance.RefreshVolumes();
    }

    public void ReturnToMainMenu()
    {
        AudioManager.Instance.PlaySFX("select");
        Time.timeScale = 1;
        RestoreAudio();
        AudioManager.Instance.RefreshVolumes();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlaySFX("select");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void MuteAudio()
    {
        originalSFXVolume = AudioSettings.GetRawCategoryVolume(AudioCategory.SFX);

        // Temporarily mute without saving
        foreach (var source in AudioManager.Instance.sfxSources)
        {
            if (source != null)
                source.volume = 0f;
        }

        if (AudioManager.Instance.loopedSFXSource != null)
            AudioManager.Instance.loopedSFXSource.volume = 0f;

        // Pause reel loop audio sources
        foreach (var source in BaseReel.AllSpinSources)
        {
            if (source != null && source.isPlaying)
                source.Pause();
        }

        // Also stop global looped SFX
        if (AudioManager.Instance.loopedSFXSource != null)
        {
            AudioManager.Instance.loopedSFXSource.Stop();
            AudioManager.Instance.loopedSFXSource.clip = null;
        }
    }

    private void RestoreAudio()
    {
        foreach (var source in AudioManager.Instance.sfxSources)
        {
            if (source != null)
                source.volume = originalSFXVolume;
        }

        if (AudioManager.Instance.loopedSFXSource != null)
            AudioManager.Instance.loopedSFXSource.volume = originalSFXVolume;

        // Resume reel loop audio sources
        foreach (var source in BaseReel.AllSpinSources)
        {
            if (source != null && source.clip != null)
                source.UnPause();
        }
    }


}