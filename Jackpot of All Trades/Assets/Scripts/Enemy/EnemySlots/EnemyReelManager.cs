using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReelManager : MonoBehaviour
{
    [Header("References")]
    public List<EnemyReel> enemyReels;   // Assign all enemy reels in the inspector or at runtime
    public List<BaseEnemy> baseEnemies;  // Assign the BaseEnemy objects for this combat

    // Populates each EnemyReel with the spell pool from its associated BaseEnemy.
    // Should be called at the start of combat or when enemies are spawned.
    public void PopulateReelsFromEnemies(List<BaseEnemy> freshEnemies = null)
    {
        if (freshEnemies != null)          // optional hand-off from SpawnManager
            baseEnemies = freshEnemies;
    
        for (int i = 0; i < enemyReels.Count; i++)
        {
            if (enemyReels.Count != baseEnemies.Count)
            {
                Debug.LogWarning($"[EnemyReelManager] Mismatch: {enemyReels.Count} reels vs {baseEnemies.Count} enemies.");
            }

            if (i >= baseEnemies.Count)
            {
                Debug.LogWarning($"[EnemyReelManager] Reel {i} has no matching enemy. Disabling...");
                if (enemyReels[i] != null)
                    enemyReels[i].gameObject.SetActive(false);

                continue;
            }

            var reel  = enemyReels[i];
            var enemy = baseEnemies[i];
            var data  = enemy?.runtimeData;
    
            // null-safety checks
            if (reel == null || enemy == null || data == null)       continue;
            if (data.spellPool == null || data.spellPool.Count == 0) continue;

            // convert List → array so the types match
            reel.availableSpells = enemy.runtimeData.spellPool.ToArray();
            reel.RandomizeStart();
        }
    }
    

    // Spins all enemy reels and sets their intent spells after the spins finish.
    public void RollAllEnemyIntents()
    {
        StartCoroutine(RollIntentsCoroutine());
    }

    public IEnumerator RollIntentsCoroutine()
    {
        // Spin all reels (only if active and safe)
        foreach (var reel in enemyReels)
        {
            if (reel != null && reel.gameObject.activeInHierarchy)
            {
                reel.Spin();
            }
        }

        // Wait until all active reels finish spinning
        bool allDone;
        do
        {
            allDone = true;
            foreach (var reel in enemyReels)
            {
                if (reel != null && reel.gameObject.activeInHierarchy && reel.IsSpinning())
                {
                    allDone = false;
                    break;
                }
            }

            yield return null;
        }
        while (!allDone);

        // After reel spins finish, assign center spells as intent
        for (int i = 0; i < enemyReels.Count; i++)
        {
            if (i < baseEnemies.Count)
            {
                var reel = enemyReels[i];
                var enemy = baseEnemies[i];

                // Skip dead enemies or inactive reels
                if (enemy != null && !enemy.IsDead && reel != null && reel.gameObject.activeInHierarchy)
                {
                    RuntimeSpell result = reel.GetCenterSpell();
                    enemy.SetIntent(result);
                }
            }
        }
    }
}
