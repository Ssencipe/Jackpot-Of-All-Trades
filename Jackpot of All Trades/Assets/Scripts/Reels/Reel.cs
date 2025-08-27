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

    // Overrides spin entry point to use player's spell list
    public void Spin()
    {
        if (IsLocked || IsSpinning() || spells == null || spells.Count == 0)
            return;

        base.Spin(spells.ToArray());
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
        StartCoroutine(reelVisual.Nudge(true, spells.ToArray()));
    }

    public void NudgeDown()
    {
        if (IsLocked || IsSpinning()) return;

        AudioManager.Instance.PlaySFX("nudge");
        OnNudged?.Invoke(this);
        StartCoroutine(reelVisual.Nudge(false, spells.ToArray()));
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