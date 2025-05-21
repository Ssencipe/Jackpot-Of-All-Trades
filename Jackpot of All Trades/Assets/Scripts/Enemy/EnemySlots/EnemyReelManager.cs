using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages one or more enemy reels, spins them, and sets enemy intent accordingly.
/// </summary>
public class EnemyReelManager : MonoBehaviour
{
    [Header("References")]
    public List<EnemyReel> enemyReels;   // Assign all enemy reels in the inspector or at runtime
    public List<BaseEnemy> baseEnemies;  // Assign the BaseEnemy objects for this combat

    /// <summary>
    /// Populates each EnemyReel with the spell pool from its associated BaseEnemy.
    /// Should be called at the start of combat or when enemies are spawned.
    /// </summary>
    public void PopulateReelsFromEnemies()
    {
        for (int i = 0; i < enemyReels.Count; i++)
        {
            if (i < baseEnemies.Count)
            {
                enemyReels[i].availableSpells = baseEnemies[i].baseData.spellPool.ToArray();
                enemyReels[i].RandomizeStart();
            }
        }
    }

    /// <summary>
    /// Spins all enemy reels and sets their intent spells after the spins finish.
    /// </summary>
    public void RollAllEnemyIntents()
    {
        StartCoroutine(RollIntentsCoroutine());
    }

    public IEnumerator RollIntentsCoroutine()
    {
        // Spin all reels
        foreach (var reel in enemyReels)
            reel.Spin();

        // Wait until all reels finish spinning
        bool allDone;
        do
        {
            allDone = true;
            foreach (var reel in enemyReels)
            {
                if (reel.IsSpinning())
                {
                    allDone = false;
                    break;
                }
            }
            yield return null;
        }
        while (!allDone);

        // Set intent for each enemy from reel result
        for (int i = 0; i < enemyReels.Count; i++)
        {
            if (i < baseEnemies.Count)
            {
                SpellSO result = enemyReels[i].GetCenterSpell();
                baseEnemies[i].SetIntent(result); // Or: baseEnemies[i].nextIntentSpell = result;
                Debug.Log($"{baseEnemies[i].baseData.enemyName} intent set via Reel: {result.spellName}");
            }
        }

        // Optionally, notify CombatManager or trigger next combat phase here!
    }
}
