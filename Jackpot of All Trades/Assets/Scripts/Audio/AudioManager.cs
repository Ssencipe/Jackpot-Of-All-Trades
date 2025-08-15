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
    public AudioLibrary musicLibrary;  // For music related audio

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
        source.volume = AudioSettings.GetVolume(entry.category) * entry.individualVolume;
        source.PlayOneShot(entry.clip);
    }

    public void PlayMusic(string clipName, AudioLibrary library = null, bool loop = true)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        musicSource.clip = entry.clip;
        musicSource.loop = loop;
        musicSource.volume = AudioSettings.GetVolume(entry.category) * entry.individualVolume;
        musicSource.Play();
    }

    public void PlayUI(string clipName, AudioLibrary library = null)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        uiSource.volume = AudioSettings.GetVolume(entry.category) * entry.individualVolume;
        uiSource.PlayOneShot(entry.clip);
    }

    public void PlayLoopedSFX(string clipName, AudioLibrary library = null)
    {
        library ??= gameLibrary;
        if (!library.TryGetEntry(clipName, out var entry)) return;

        loopedSFXSource.clip = entry.clip;
        loopedSFXSource.loop = true;
        loopedSFXSource.volume = AudioSettings.GetVolume(entry.category) * entry.individualVolume;
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
        // Update music volume if clip exists in a library
        if (musicSource.clip != null)
        {
            if (musicLibrary.TryGetEntry(musicSource.clip.name, out var musicEntry))
            {
                musicSource.volume = AudioSettings.GetVolume(musicEntry.category) * musicEntry.individualVolume;
            }
        }

        // Update UI volume if clip exists in a library
        if (uiSource.clip != null)
        {
            if (gameLibrary.TryGetEntry(uiSource.clip.name, out var uiEntry))
            {
                uiSource.volume = AudioSettings.GetVolume(uiEntry.category) * uiEntry.individualVolume;
            }
        }

        // Update looped SFX volume if clip exists in a library
        if (loopedSFXSource.clip != null)
        {
            if (gameLibrary.TryGetEntry(loopedSFXSource.clip.name, out var sfxEntry))
            {
                loopedSFXSource.volume = AudioSettings.GetVolume(sfxEntry.category) * sfxEntry.individualVolume;
            }
        }

        // Update all general SFX sources
        foreach (var source in sfxSources)
        {
            if (source.clip != null)
            {
                if (gameLibrary.TryGetEntry(source.clip.name, out var sfxClipEntry))
                {
                    source.volume = AudioSettings.GetVolume(sfxClipEntry.category) * sfxClipEntry.individualVolume;
                }
                else
                {
                    // Fallback in case clip isn't registered
                    source.volume = AudioSettings.GetVolume(AudioCategory.SFX);
                }
            }
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
            PlayMusic("battle_theme", musicLibrary);
        }
        else if (scene.name == "MainMenu")
        {
            PlayMusic("main_theme", musicLibrary);
        }
    }
}