using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource uiSource;
    public AudioSource loopedSFXSource;
    public List<AudioSource> sfxSources;

    [Header("Audio Libraries")]
    public AudioLibrary gameLibrary;   // For general UI and world audio
    public AudioLibrary spellLibrary;  // For spell-specific audio

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(string clipName, AudioLibrary library = null)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        AudioSource source = GetAvailableSFXSource();
        source.volume = AudioSettings.GetVolume(entry.category);
        source.PlayOneShot(entry.clip);
    }

    public void PlayMusic(string clipName, AudioLibrary library = null, bool loop = true)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        musicSource.clip = entry.clip;
        musicSource.loop = loop;
        musicSource.volume = AudioSettings.GetVolume(entry.category);
        musicSource.Play();
    }

    public void PlayUI(string clipName, AudioLibrary library = null)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        uiSource.volume = AudioSettings.GetVolume(entry.category);
        uiSource.PlayOneShot(entry.clip);
    }

    public void PlayLoopedSFX(string clipName, AudioLibrary library = null)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        loopedSFXSource.clip = entry.clip;
        loopedSFXSource.loop = true;
        loopedSFXSource.volume = AudioSettings.GetVolume(entry.category);
        loopedSFXSource.Play();
    }

    public void StopLoopedSFX()
    {
        if (loopedSFXSource != null)
        {
            loopedSFXSource.Stop();
            loopedSFXSource.clip = null;
        }
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var source in sfxSources)
        {
            if (!source.isPlaying)
                return source;
        }

        return sfxSources[0]; // fallback
    }

    public void RefreshVolumes()
    {
        musicSource.volume = AudioSettings.GetVolume(AudioCategory.Music);
        uiSource.volume = AudioSettings.GetVolume(AudioCategory.UI);
        loopedSFXSource.volume = AudioSettings.GetVolume(AudioCategory.SFX);

        foreach (var source in sfxSources)
        {
            source.volume = AudioSettings.GetVolume(AudioCategory.SFX);
        }
    }

    //checks game scene to set music
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BattleScene")
        {
            PlayMusic("battle_theme", gameLibrary);
        }
        else if (scene.name == "MainMenu")
        {
            PlayMusic("main_theme", gameLibrary);
        }
    }
}