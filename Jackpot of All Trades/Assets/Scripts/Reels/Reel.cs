using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Reel : MonoBehaviour
{
    [Header("Visuals")]
    public Image reelBackground;
    public Image lockVisualImage;

    [Header("Spin Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 5f;
    public float minSpinSpeed = 100f;
    public float maxSpinSpeed = 800f;

    [Header("Dependencies")]
    public ReelVisual reelVisual;
    public RuntimeReel runtimeReel;

    private bool isSpinning = false;
    public bool IsLocked { get; private set; } = false;
    public bool IsSpinning() => isSpinning;

    // Shortcut for accessing associated spell array
    private List<RuntimeSpell> spells => runtimeReel?.spells;

    // events for various scenrios
    public event Action<Reel> OnSpinStarted;
    public event Action<Reel> OnSpinFinished;
    public event Action<Reel> OnLock;
    public event Action<Reel> OnUnlock;
    public event Action<Reel> OnNudged;

    // for spinning audio
    private AudioSource spinLoopSource;

    // primarily sets up audio
    private void Awake()
    {
        spinLoopSource = gameObject.AddComponent<AudioSource>();
        spinLoopSource.loop = true;
        spinLoopSource.playOnAwake = false;
        spinLoopSource.volume = 0.5f; // optional
    }

    // Begins spinning the reel, unless it's locked or already spinning.
    public void Spin()
    {
        if (IsLocked || isSpinning || spells == null || spells.Count == 0)
            return;

        StartCoroutine(SpinCoroutine());
    }

    // Coroutine that controls reel spinning with easing and timing.
    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        AudioClip spinClip = AudioManager.Instance.gameLibrary.GetClip("reel_spin");
        if (spinClip != null)
        {
            spinLoopSource.clip = spinClip;
            spinLoopSource.Play();
        }

        OnSpinStarted?.Invoke(this);

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);
        yield return StartCoroutine(reelVisual.ScrollSpells(spinDuration, minSpinSpeed, maxSpinSpeed, spells.ToArray()));

        isSpinning = false;
        spinLoopSource.Stop();
        spinLoopSource.clip = null;

        OnSpinFinished?.Invoke(this);
    }

    // Nudges the reel upward one position.
    public void NudgeUp()
    {
        if (IsLocked || isSpinning) return;

        AudioManager.Instance.PlaySFX("nudge");
        OnNudged?.Invoke(this);
        StartCoroutine(reelVisual.Nudge(true, spells.ToArray()));
    }

    // Nudges the reel downward one position.
    public void NudgeDown()
    {
        if (IsLocked || isSpinning) return;

        AudioManager.Instance.PlaySFX("nudge");
        OnNudged?.Invoke(this);
        StartCoroutine(reelVisual.Nudge(false, spells.ToArray()));
    }

    // Locks the reel and shows the visual indicator.
    public void Lock()
    {
        IsLocked = true;
        if (lockVisualImage != null)
        {
            lockVisualImage.enabled = true;
        }
    }

    // Unlocks the reel and hides the lock visual.
    public void Unlock()
    {
        IsLocked = false;

        if (lockVisualImage != null)
        {
            lockVisualImage.enabled = false;
            lockVisualImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    // Initializes the reel with a random spell start.
    public void RandomizeStart()
    {
        reelVisual?.InitializeVisuals(spells.ToArray());
    }

    // Gets the spell currently at the visual center of the reel.
    public RuntimeSpell GetTopSpell()
    {
        RuntimeSpell topSpell = reelVisual?.GetSpellAtVisualIndex(0);
        return topSpell;
    }

    // Gets the topmost visible spell on the reel.
    public RuntimeSpell GetCenterSpell()
    {
        RuntimeSpell centerSpell = reelVisual?.GetCenterSpell();
        return centerSpell;
    }

    // Gets the bottommost visible spell on the reel.
    public RuntimeSpell GetBottomSpell()
    {
        if (reelVisual == null) return null;
        int count = reelVisual.GetSlots().Count;
        if (count == 0) return null;

        RuntimeSpell bottomSpell = reelVisual.GetSpellAtVisualIndex(count - 1);
        return bottomSpell;
    }
}