using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReelManager : MonoBehaviour
{
    [Header("Reel Spawning")]
    public GameObject enemyReelPrefab;         // Assign EnemyReel_Visual prefab here
    public Transform reelParentTransform;      // Parent layout container for positioning

    [Header("References")]
    public List<EnemyReel> enemyReels = new List<EnemyReel>();        // Populated at runtime
    public List<BaseEnemy> baseEnemies = new List<BaseEnemy>();      // Assigned by spawn system

    // Call this when combat begins, with fresh enemy data
    public void PopulateReelsFromEnemies(List<BaseEnemy> freshEnemies = null)
    {
        if (freshEnemies != null)
            baseEnemies = freshEnemies;

        // Destroy existing reel objects (if any)
        foreach (var oldReel in enemyReels)
        {
            if (oldReel != null)
                Destroy(oldReel.gameObject);
        }

        enemyReels.Clear();

        for (int i = 0; i < baseEnemies.Count; i++)
        {
            if (enemyReelPrefab == null)
            {
                Debug.LogError("[EnemyReelManager] No enemyReelPrefab assigned!");
                return;
            }

            GameObject reelGO = Instantiate(enemyReelPrefab, reelParentTransform);
            reelGO.name = $"EnemyReel_{i}";

            EnemyReel reel = reelGO.GetComponent<EnemyReel>();
            if (reel == null)
            {
                Debug.LogError($"[EnemyReelManager] EnemyReel component missing on prefab.");
                continue;
            }

            var enemy = baseEnemies[i];
            if (enemy?.runtimeData == null || enemy.runtimeData.spellPool == null || enemy.runtimeData.spellPool.Count == 0)
            {
                Debug.LogWarning($"[EnemyReelManager] Enemy {i} has no valid spell data.");
                continue;
            }

            reel.availableSpells = enemy.runtimeData.spellPool.ToArray();
            reel.RandomizeStart();

            enemyReels.Add(reel);
        }
    }

    // Spins all enemy reels and sets their intent spells after spins finish
    public void RollAllEnemyIntents()
    {
        StartCoroutine(RollIntentsCoroutine());
    }

    public IEnumerator RollIntentsCoroutine()
    {
        // Start all spins
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

        // Assign center spell of each reel as that enemy’s intent
        for (int i = 0; i < enemyReels.Count; i++)
        {
            if (i < baseEnemies.Count)
            {
                RuntimeSpell result = enemyReels[i].GetCenterSpell();
                baseEnemies[i].SetIntent(result);
            }
        }
    }
}
