using UnityEngine;
using System.Collections.Generic;

public class EnemyReelSpawner : MonoBehaviour
{
    [Header("Reel Setup")]
    public GameObject enemyReelPrefab;

    [Header("Reel Parents")]
    public Transform leftReelArea;
    public Transform centerReelArea;
    public Transform rightReelArea;

    private Dictionary<BaseEnemy, List<EnemyReel>> spawnedReels = new();

    public void SpawnEnemyReels(List<BaseEnemy> baseEnemies)
    {
        // Clear any previously spawned reels
        foreach (var list in spawnedReels.Values)
        {
            foreach (var reel in list)
            {
                if (reel != null)
                    Destroy(reel.gameObject);
            }
        }

        spawnedReels.Clear();

        for (int i = 0; i < baseEnemies.Count && i < 3; i++)
        {
            BaseEnemy enemy = baseEnemies[i];
            Transform parentArea = GetAreaForIndex(i);

            if (parentArea == null || enemy?.runtimeData?.runtimeReelSpellPools == null)
                continue;

            List<RuntimeSpell> pool;
            List<EnemyReel> enemyReels = new();

            var spellPools = enemy.runtimeData.runtimeReelSpellPools;

            float totalWidth = (spellPools.Count - 1);
            float startX = -totalWidth / 2f;

            for (int j = 0; j < spellPools.Count; j++)
            {
                pool = spellPools[j];

                GameObject reelGO = Instantiate(enemyReelPrefab, parentArea);
                RectTransform rt = reelGO.GetComponent<RectTransform>();
                if (rt != null)
                    rt.anchoredPosition = new Vector2(startX + j, 0f);

                EnemyReel reel = reelGO.GetComponentInChildren<EnemyReel>();
                if (reel != null)
                {
                    reel.availableSpells = pool.ToArray();
                    reel.RandomizeStart();
                    enemyReels.Add(reel);
                }
            }

            spawnedReels.Add(enemy, enemyReels);
        }
    }

    private Transform GetAreaForIndex(int index)
    {
        return index switch
        {
            0 => leftReelArea,
            1 => centerReelArea,
            2 => rightReelArea,
            _ => null
        };
    }

    public List<EnemyReel> GetReelsForEnemy(BaseEnemy enemy)
    {
        return spawnedReels.ContainsKey(enemy) ? spawnedReels[enemy] : new List<EnemyReel>();
    }

    public List<EnemyReel> GetAllReels()
    {
        List<EnemyReel> all = new();
        foreach (var list in spawnedReels.Values)
            all.AddRange(list);

        return all;
    }
}
