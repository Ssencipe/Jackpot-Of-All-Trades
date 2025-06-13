using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class Reel : MonoBehaviour
{
    [Header("Spells")]
    public SpellSO[] availableSpells;

    [Header("Visuals")]
    public Image reelBackground;
    public Image lockVisualImage;

    [Header("Spin Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 5f;
    public float minSpinSpeed = 300f; // Slowest spin speed
    public float maxSpinSpeed = 600f;  // Fastest spin speed
    public bool IsSpinning() => isSpinning;


    [Header("Dependencies")]
    public ReelVisual reelVisual;

    public bool IsLocked { get; private set; } = false;
    private bool isSpinning = false;

    public event Action<Reel> OnSpinFinished;

    public void Spin()
    {
        if (IsLocked || isSpinning || availableSpells.Length == 0) return;
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);
        yield return StartCoroutine(reelVisual.ScrollSpells(spinDuration, minSpinSpeed, maxSpinSpeed, availableSpells));

        isSpinning = false;
        OnSpinFinished?.Invoke(this);
    }

    public void NudgeUp()
    {
        if (IsLocked || isSpinning) return;
        StartCoroutine(reelVisual.Nudge(true, availableSpells));
    }

    public void NudgeDown()
    {
        if (IsLocked || isSpinning) return;
        StartCoroutine(reelVisual.Nudge(false, availableSpells));
    }

    public void Lock()
    {
        IsLocked = true;
        if (lockVisualImage != null)
            lockVisualImage.enabled = true;

        AdjustLockPosition();
    }

    public void Unlock()
    {
        IsLocked = false;

        if (lockVisualImage != null)
        {
            lockVisualImage.enabled = false;

            // Optional safety reset
            lockVisualImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    public void RandomizeStart()
    {
        reelVisual?.InitializeVisuals(availableSpells);
    }

    //offsets perspective skew of lock sprite
    public void AdjustLockPosition()
    {
        if (lockVisualImage == null || Camera.main == null) return;

        RectTransform lockRect = lockVisualImage.rectTransform;

        // Get this reel's center in screen space
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        float screenCenterX = Screen.width / 2f;
        float screenCenterY = Screen.height / 2f;

        // Distance from screen center
        float offsetX = screenPos.x - screenCenterX;
        float offsetY = screenPos.y - screenCenterY;

        // Apply a correction factor to skew based on distance from screen center
        float xCorrection = -offsetX * 0.09f; // tweak factor as needed
        float yCorrection = -offsetY * 0.04f; // tweak factor as needed

        lockRect.anchoredPosition = new Vector2(xCorrection, yCorrection);
    }

    public SpellSO GetCenterSpell()
    {
        return reelVisual?.GetCenterSpell();
    }

    public SpellSO GetTopSpell()
    {
        return reelVisual?.GetSpellAtVisualIndex(0); // assuming top = slot[0]
    }

    public SpellSO GetBottomSpell()
    {
        return reelVisual?.GetSpellAtVisualIndex(reelVisual.GetSlots().Count - 1);
    }
}