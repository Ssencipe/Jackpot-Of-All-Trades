using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class EnemyReel : MonoBehaviour
{
    [Header("Spells")]
    public SpellSO[] availableSpells;
    private int currentIndex = 0;

    [Header("Visuals")]
    public Image reelBackground;
    public Image upperSprite;
    public Image lowerSprite;

    [Header("Spin Settings")]
    public float minSpinDuration = 3f;
    public float maxSpinDuration = 5f;
    public float minSpinSpeed = 0.05f;
    public float maxSpinSpeed = 0.5f;

    private bool isSpinning = false;

    private void Start()
    {
        RandomizeStart();
    }

    /// <summary>
    /// Spins the reel for a random duration and updates visuals.
    /// </summary>
    public void Spin()
    {
        if (isSpinning || availableSpells == null || availableSpells.Length == 0) return;
        StartCoroutine(SpinCoroutine());
    }

    private IEnumerator SpinCoroutine()
    {
        isSpinning = true;

        float spinDuration = UnityEngine.Random.Range(minSpinDuration, maxSpinDuration);
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            float t = elapsed / spinDuration;
            float easedT = 1 - Mathf.Pow(1 - t, 3); // EaseOutCubic
            float currentCooldown = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, easedT);

            currentIndex = (currentIndex - 1 + availableSpells.Length) % availableSpells.Length;
            UpdateVisuals();

            yield return new WaitForSeconds(currentCooldown);
            elapsed += currentCooldown;
        }

        isSpinning = false;
    }

    /// <summary>
    /// Randomizes the start position of the reel.
    /// </summary>
    public void RandomizeStart()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;
        currentIndex = UnityEngine.Random.Range(0, availableSpells.Length);
        UpdateVisuals();
    }

    /// <summary>
    /// Updates the reel's visuals to reflect the current state.
    /// </summary>
    private void UpdateVisuals()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;

        if (reelBackground != null)
            reelBackground.sprite = availableSpells[currentIndex].icon;

        if (upperSprite != null)
            upperSprite.sprite = availableSpells[(currentIndex - 1 + availableSpells.Length) % availableSpells.Length].icon;

        if (lowerSprite != null)
            lowerSprite.sprite = availableSpells[(currentIndex + 1) % availableSpells.Length].icon;
    }

    /// <summary>
    /// Returns the spell in the center of the reel (for intent).
    /// </summary>
    public SpellSO GetCenterSpell()
    {
        if (availableSpells == null || availableSpells.Length == 0)
            return null;
        return availableSpells[currentIndex];
    }

    /// <summary>
    /// Returns the index of the center spell (for UI).
    /// </summary>
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    /// <summary>
    /// Returns true if the reel is currently spinning.
    /// </summary>
    public bool IsSpinning()
    {
        return isSpinning;
    }
}
