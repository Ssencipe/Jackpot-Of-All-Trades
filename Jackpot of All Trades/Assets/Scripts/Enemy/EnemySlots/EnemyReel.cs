using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyReel : BaseReel
{
    [Header("Spells")]
    public RuntimeSpell[] availableSpells;
    private int currentIndex = 0;

    [Header("Dependencies")]
    public EnemyReelVisual enemyReelVisual;
    public EnemyReelUI linkedUI;

    private void Start()
    {
        RandomizeStart();
    }

    // Randomizes the start position of the reel.
    public void RandomizeStart()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;

        currentIndex = UnityEngine.Random.Range(0, availableSpells.Length);

        // Initialize EnemyReelVisual with the random starting position
        if (enemyReelVisual != null)
        {
            enemyReelVisual.InitializeVisuals(availableSpells);
            enemyReelVisual.SetCurrentIndex(currentIndex); // Use public setter
        }

        linkedUI?.UpdateVisuals();
    }

    // Spins the reel for a random duration and updates visuals.
    // (Now handled via BaseReel, which calls ScrollVisuals)
    // We override ScrollVisuals to implement animation via EnemyReelVisual
    protected override IEnumerator ScrollVisuals(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells)
    {
        if (enemyReelVisual != null)
        {
            // Use EnemyReelVisual for smooth animation
            yield return StartCoroutine(enemyReelVisual.ScrollSpells(duration, minSpeed, maxSpeed, spells));

            // Get the final index directly from EnemyReelVisual
            currentIndex = enemyReelVisual.GetCurrentIndex();
        }
        else
        {
            // Fallback to old system if EnemyReelVisual not assigned
            yield return StartCoroutine(OldSpinAnimation(duration));
        }

        // Final sync
        if (linkedUI == null)
            linkedUI = GetComponentInChildren<EnemyReelUI>();

        linkedUI?.UpdateVisuals();
    }

    // Keep old animation as fallback
    private IEnumerator OldSpinAnimation(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easedT = 1 - Mathf.Pow(1 - t, 3); // EaseOutCubic
            float currentCooldown = Mathf.Lerp(minSpinSpeed, maxSpinSpeed, easedT);

            currentIndex = (currentIndex - 1 + availableSpells.Length) % availableSpells.Length;
            UpdateVisuals();

            yield return new WaitForSeconds(currentCooldown);
            elapsed += currentCooldown;
        }
    }

    public void Spin()
    {
        if (IsSpinning() || availableSpells == null || availableSpells.Length == 0)
            return;

        base.Spin(availableSpells);
    }

    // Updates the reel's visuals to reflect the current state.
    private void UpdateVisuals()
    {
        if (availableSpells == null || availableSpells.Length == 0) return;

        // Only update background if it exists
        if (reelBackground != null)
            reelBackground.sprite = availableSpells[currentIndex].icon;

        // EnemyReelVisual handles all the sprite positioning and updates
        // No need to manually update upperSprite/lowerSprite anymore
    }

    // Returns the spell in the center of the reel (for intent).
    public RuntimeSpell GetCenterSpell()
    {
        if (availableSpells == null || availableSpells.Length == 0) return null;
        return availableSpells[currentIndex];
    }

    // Returns the index of the center spell (for UI).
    public int GetCurrentIndex() => currentIndex;
}