using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Reel : BaseReel
{
    [Header("Player Reel Specific")]
    public Image lockVisualImage;

    [Header("Dependencies")]
    public ReelVisual reelVisual;
    public RuntimeReel runtimeReel;

    [Header("Animation Timing")]
    public float orbitDuration = 1f;

    public bool IsLocked { get; private set; } = false;

    private List<RuntimeSpell> spells => runtimeReel?.spells;

    public event Action<Reel> OnLock;
    public event Action<Reel> OnUnlock;
    public event Action<Reel> OnNudged;

    protected override void Awake()
    {
        base.Awake(); // Set up audio
    }

    public void Spin()
    {
        if (IsSpinning() || IsLocked || spells == null || spells.Count == 0 || Time.timeScale == 0f)
            return;

        Spin(spells.ToArray());
    }

    // Overrides spin entry point to use player's spell list
    public override void Spin(RuntimeSpell[] spells)
    {
        if (IsSpinning() || spells == null || spells.Length == 0 || Time.timeScale == 0f) return;
        StartCoroutine(SpinCoroutineWithCounters(spells));
    }

    protected override IEnumerator ScrollVisuals(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spellArray)
    {
        yield return StartCoroutine(reelVisual.ScrollSpells(duration, minSpeed, maxSpeed, spellArray));
    }

    public void RandomizeStart()
    {
        reelVisual?.InitializeVisuals(spells.ToArray());
    }

    public RuntimeSpell GetTopSpell()
    {
        return reelVisual?.GetSpellAtVisualIndex(0);
    }

    public RuntimeSpell GetCenterSpell()
    {
        return reelVisual?.GetCenterSpell();
    }

    public RuntimeSpell GetBottomSpell()
    {
        if (reelVisual == null) return null;
        int count = reelVisual.GetSlots().Count;
        if (count == 0) return null;

        return reelVisual.GetSpellAtVisualIndex(count - 1);
    }

    public void Lock()
    {
        IsLocked = true;
        if (lockVisualImage != null)
            lockVisualImage.enabled = true;

        OnLock?.Invoke(this);
    }

    public void Unlock()
    {
        IsLocked = false;
        if (lockVisualImage != null)
        {
            lockVisualImage.enabled = false;
            lockVisualImage.rectTransform.anchoredPosition = Vector2.zero;
        }

        OnUnlock?.Invoke(this);
    }

    public void NudgeUp()
    {
        if (IsLocked || IsSpinning()) return;

        AudioManager.Instance.PlaySFX("nudge");
        OnNudged?.Invoke(this);
        StartCoroutine(NudgeRoutine(true));
    }

    public void NudgeDown()
    {
        if (IsLocked || IsSpinning()) return;

        AudioManager.Instance.PlaySFX("nudge");
        OnNudged?.Invoke(this);
        StartCoroutine(NudgeRoutine(false));
    }

    private IEnumerator NudgeRoutine(bool isUp)
    {
        ShowSlotCounters(false);

        yield return StartCoroutine(reelVisual.Nudge(isUp, spells.ToArray()));

        ShowSlotCounters(true);
    }

    // for counters in ReelSlot
    public void ShowSlotCounters(bool show)
    {
        if (reelVisual != null)
        {
            foreach (var slot in reelVisual.GetSlots())
            {
                slot.SetCountersActive(show);
            }
        }
    }

    private IEnumerator SpinCoroutineWithCounters(RuntimeSpell[] spells)
    {
        isSpinning = true;
        ShowSlotCounters(false);

        if (Time.timeScale > 0f &&
            AudioManager.Instance != null &&
            AudioManager.Instance.gameLibrary.TryGetEntry("reel_spin", out var entry))
        {
            spinLoopSource.clip = entry.clip;
            spinLoopSource.volume = AudioSettings.GetVolume(entry.category) * entry.individualVolume;
            spinLoopSource.Play();
        }

        RaiseSpinStarted();

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);
        yield return StartCoroutine(ScrollVisuals(spinDuration, minSpinSpeed, maxSpinSpeed, spells));

        isSpinning = false;
        spinLoopSource.Stop();
        spinLoopSource.clip = null;

        RaiseSpinFinished();

        ShowSlotCounters(true);
    }

    // for getting grid data filled out
    public RuntimeSpell GetSpellAtSlot(int slotIndex)
    {
        if (reelVisual == null) return null;

        var visualSlots = reelVisual.GetSlots();
        if (slotIndex < 0 || slotIndex >= visualSlots.Count) return null;

        return reelVisual.GetSpellAtVisualIndex(slotIndex);
    }

    // animate a specific spell slot when processing reel
    public void PlayEffectAtSlot(int slotIndex)
    {
        if (reelVisual == null) return;

        var slots = reelVisual.GetSlots();
        if (slotIndex < 0 || slotIndex >= slots.Count) return;

        var slot = slots[slotIndex];
        if (slot == null) return;

        StartCoroutine(DoOrbitEffect(slot.transform, radius: 8f, orbitSpeed: 3f, duration: orbitDuration));
    }

    // Orbit animation effect around the original position
    private IEnumerator DoOrbitEffect(Transform target, float radius = 8f, float orbitSpeed = 3f, float duration = 1f)
    {
        Vector3 originalPosition = target.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float angle = elapsed * orbitSpeed * 2f * Mathf.PI;

            float offsetX = Mathf.Cos(angle) * radius;
            float offsetY = Mathf.Sin(angle) * radius;

            target.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localPosition = originalPosition;
    }
}