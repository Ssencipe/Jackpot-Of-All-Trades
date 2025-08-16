using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReelManager : MonoBehaviour
{
    [Header("References")]
    public EnemyReelSpawner reelSpawner;
    public List<BaseEnemy> baseEnemies;

    public void PopulateReelsFromEnemies(List<BaseEnemy> freshEnemies = null)
    {
        if (freshEnemies != null)
            baseEnemies = freshEnemies;

        if (reelSpawner == null)
        {
            Debug.LogError("EnemyReelSpawner not assigned!");
            return;
        }

        reelSpawner.SpawnEnemyReels(baseEnemies);
    }

    public void RollAllEnemyIntents()
    {
        StartCoroutine(RollIntentsCoroutine());
    }

    public IEnumerator RollIntentsCoroutine()
    {
        if (reelSpawner == null)
        {
            Debug.LogError("EnemyReelSpawner is missing!");
            yield break;
        }

        List<EnemyReel> allReels = reelSpawner.GetAllReels();

        //start spinning all reels
        foreach (var reel in allReels)
        {
            if (reel != null && reel.gameObject.activeInHierarchy)
                reel.Spin();
        }

        //wait until all reels are done
        bool allDone;
        do
        {
            allDone = true;
            foreach (var reel in allReels)
            {
                if (reel != null && reel.gameObject.activeInHierarchy && reel.IsSpinning())
                {
                    allDone = false;
                    break;
                }
            }
            yield return null;
        } while (!allDone);
    }

}
