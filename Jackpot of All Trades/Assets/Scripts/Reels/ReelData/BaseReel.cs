using System;
using System.Collections;
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

    protected AudioSource spinLoopSource;
    protected bool isSpinning = false;

    public bool IsSpinning() => isSpinning;

    // Events triggered at the start and end of a spin
    public event Action<BaseReel> OnSpinStarted;
    public event Action<BaseReel> OnSpinFinished;

    // Sets up looping audio source for spin sound
    protected virtual void Awake()
    {
        spinLoopSource = gameObject.AddComponent<AudioSource>();
        spinLoopSource.loop = true;
        spinLoopSource.playOnAwake = false;
        spinLoopSource.volume = 0.5f;
    }

    // Begins the spinning sequence with the provided spell list
    public virtual void Spin(RuntimeSpell[] spells)
    {
        if (IsSpinning() || spells == null || spells.Length == 0) return;
        StartCoroutine(SpinCoroutine(spells));
    }

    // Handles the full spin sequence, including audio and timing
    private IEnumerator SpinCoroutine(RuntimeSpell[] spells)
    {
        isSpinning = true;

        AudioClip spinClip = AudioManager.Instance?.gameLibrary?.GetClip("reel_spin");
        if (spinClip != null)
        {
            spinLoopSource.clip = spinClip;
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

    // Must be overridden to define how the visual portion scrolls
    protected abstract IEnumerator ScrollVisuals(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells);
}
