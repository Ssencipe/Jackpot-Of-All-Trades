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

    [Header("Visuals")]
    public GameObject castingBorder; // The border around the casting spell

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
            currentIndex = enemyReelVisual.GetCurrentIndex(); // Sync to visual's logical center
        }

        linkedUI?.UpdateVisuals();
    }

    // Spins the reel for a random duration and updates visuals.
    // Now handled via BaseReel, which calls ScrollVisuals
    // We override ScrollVisuals to implement animation via EnemyReelVisual
    protected override IEnumerator ScrollVisuals(float duration, float minSpeed, float maxSpeed, RuntimeSpell[] spells)
    {
        if (enemyReelVisual != null)
        {
            enemyReelVisual.ShowSlotCounters(false); // Hide counters before scroll
            yield return StartCoroutine(enemyReelVisual.ScrollSpells(duration, minSpeed, maxSpeed, spells));
            enemyReelVisual.ShowSlotCounters(true);  // Show after scroll
            currentIndex = enemyReelVisual.GetCurrentIndex();
        }

        linkedUI?.UpdateVisuals();
    }

    public void Spin()
    {
        if (IsSpinning() || availableSpells == null || availableSpells.Length == 0)
            return;

        base.Spin(availableSpells);
    }

    // Returns the spell in the center of the reel (for intent).
    public RuntimeSpell GetCenterSpell()
    {
        return enemyReelVisual?.GetCenterSpell();
    }

    public RuntimeSpell GetTopSpell()
    {
        return enemyReelVisual?.GetTopSpell();
    }

    public RuntimeSpell GetBottomSpell()
    {
        return enemyReelVisual?.GetBottomSpell();
    }


    public void ShowCastingBorder(bool show)
    {
        if (castingBorder != null)
            castingBorder.SetActive(show);
    }

    // Returns the index of the center spell (for UI).
    public int GetCurrentIndex() => currentIndex;
}