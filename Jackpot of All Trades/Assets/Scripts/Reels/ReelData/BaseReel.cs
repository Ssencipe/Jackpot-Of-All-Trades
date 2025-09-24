using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Base class that handles the spinning behavior and audio for any reel type.
public abstract class BaseReel : MonoBehaviour
{
    [Header("Visuals")]
    public Image reelBackground;

    [Header("Spin Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 5f;
    public float minSpinSpeed = 100f;
    public float maxSpinSpeed = 800f;

    public static List<AudioSource> AllSpinSources = new();
    protected AudioSource spinLoopSource;

    protected bool isSpinning = false;

    public bool IsSpinning() => isSpinning;

    // Events triggered at the start and end of a spin
    protected event Action<BaseReel> OnSpinStarted;
    protected event Action<BaseReel> OnSpinFinished;

    // Sets up looping audio source for spin sound
    protected virtual void Awake()
    {
        spinLoopSource = gameObject.AddComponent<AudioSource>();
        spinLoopSource.loop = true;
        spinLoopSource.playOnAwake = false;
        spinLoopSource.volume = 0.25f;

        AllSpinSources.Add(spinLoopSource);
    }

    // Begins the spinning sequence with the provided spell list
    public virtual void Spin(RuntimeSpell[] spells)
    {
        if (IsSpinning() || spells == null || spells.Length == 0 || Time.timeScale == 0f) return;
        StartCoroutine(SpinCoroutine(spells));
    }

    // Handles the full spin sequence, including audio and timing
    private IEnumerator SpinCoroutine(RuntimeSpell[] spells)
    {
        isSpinning = true;

        // only play if not paused
        if (Time.timeScale > 0f &&
            AudioManager.Instance != null &&
            AudioManager.Instance.gameLibrary.TryGetEntry("reel_spin", out var entry))
        {
            spinLoopSource.clip = entry.clip;
            spinLoopSource.volume = AudioSettings.GetVolume(entry.category) * entry.individualVolume;
            spinLoopSource.Play();
        }

        OnSpinStarted?.Invoke(this);

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);

        yield return StartCoroutine(ScrollVisuals(spinDuration, minSpinSpeed, maxSpinSpeed, spells));

        isSpinning = false;
        spinLoopSource.Stop();
        spinLoopSource.clip = null;

        OnSpinFinished?.Invoke(this);
    }

    // Event triggers
    protected void RaiseSpinStarted()
    {
        OnSpinStarted?.Invoke(this);
    }

    protected void RaiseSpinFinished()
    {
        OnSpinFinished?.Invoke(this);
    }

    protected virtual void OnDestroy()
    {
        AllSpinSources.Remove(spinLoopSource);
    }


    // Must be overridden to define how the visual portion scrolls
    protected abstract IEnumerator ScrollVisuals(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells);
}
