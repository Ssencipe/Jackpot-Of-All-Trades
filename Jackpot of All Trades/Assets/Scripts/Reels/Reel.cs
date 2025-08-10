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
}