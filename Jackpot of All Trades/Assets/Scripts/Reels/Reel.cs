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

    // Called when spinning completes
    public event Action<Reel> OnSpinFinished;

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

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);
        yield return StartCoroutine(reelVisual.ScrollSpells(spinDuration, minSpinSpeed, maxSpinSpeed, spells.ToArray()));


        isSpinning = false;
        OnSpinFinished?.Invoke(this);
    }

    // Nudges the reel upward one position.
    public void NudgeUp()
    {
        if (IsLocked || isSpinning) return;
        StartCoroutine(reelVisual.Nudge(true, spells.ToArray()));
    }

    // Nudges the reel downward one position.
    public void NudgeDown()
    {
        if (IsLocked || isSpinning) return;
        StartCoroutine(reelVisual.Nudge(false, spells.ToArray()));
    }

    // Locks the reel and shows the visual indicator.
    public void Lock()
    {
        IsLocked = true;
        if (lockVisualImage != null)
        {
            lockVisualImage.enabled = true;
            AdjustLockPosition();
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

    // Skews the lock icon based on reel screen position (for pseudo-3D perspective).
    public void AdjustLockPosition()
    {
        if (lockVisualImage == null || Camera.main == null) return;

        RectTransform lockRect = lockVisualImage.rectTransform;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        //Get reel center in screen space
        float screenCenterX = Screen.width / 2f;
        float screenCenterY = Screen.height / 2f;

        //Get relative screen center
        float offsetX = screenPos.x - screenCenterX;
        float offsetY = screenPos.y - screenCenterY;

        //Offset skew around edges of screen from perspective camera
        float xCorrection = -offsetX * 0.09f;
        float yCorrection = -offsetY * 0.04f;

        lockRect.anchoredPosition = new Vector2(xCorrection, yCorrection);
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