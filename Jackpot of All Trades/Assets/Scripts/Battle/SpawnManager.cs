using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public GameObject playerSpawnObject;
    public Transform[] enemySpawnPoints;

    [Header("Player UI")]
    public BattleHUD playerHUD;

    [Header("Enemy UI")]
    public BattleHUD[] enemyHUDs;

    [Header("Prefab References")]
    public GameObject playerPrefab;
    public GameObject enemyVisualPrefab;

    [Header("Manager References")]
    public CombatManager combatManager; 

    [Header("Spawner References")]
    public ReelSpawner reelSpawner;

    private EnemyUI currentEnemy;

    public EnemyReelManager enemyReelManager;
    public WandAnimator wandAnimator { get; private set; }



    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab not assigned!");
            return;
        }

        if (playerSpawnObject == null)
        {
            Debug.LogError("Player spawn object not assigned!");
            return;
        }

        //use the GameObject's transform for position and parenting
        Transform spawnTransform = playerSpawnObject.transform;

        GameObject playerGO = Instantiate(playerPrefab, spawnTransform.position, Quaternion.identity, spawnTransform);

        //get WandAnimator from the instantiated player
        wandAnimator = playerGO.GetComponentInChildren<WandAnimator>();

        Unit playerUnit = playerGO.GetComponent<Unit>();
        if (playerUnit == null)
        {
            Debug.LogError("Player prefab missing Unit script!");
            return;
        }

        combatManager.RegisterPlayer(playerUnit);

        playerHUD = playerGO.GetComponentInChildren<BattleHUD>();
        if (playerHUD != null)
        {
            playerHUD.Bind(playerUnit);
        }
        else
        {
            Debug.LogWarning("No BattleHUD found inside player prefab.");
        }

        //pass the player GameObject to the reel spawner
        if (reelSpawner != null)
            reelSpawner.SetPlayerReference(playerGO);
    }

    public List<BaseEnemy> SpawnEnemies(List<EnemySO> encounterPool)
    {
        Debug.Log($"Spawning enemies from pool: {encounterPool.Count} entries");

        if (encounterPool == null || encounterPool.Count == 0)
        {
            Debug.LogWarning("Encounter pool is empty.");
            return new List<BaseEnemy>();
        }

        List<BaseEnemy> spawned = new List<BaseEnemy>();
        int count = Mathf.Min(encounterPool.Count, enemySpawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            EnemySO enemySO = encounterPool[i];
            Transform spawnPoint = enemySpawnPoints[i];

            RuntimeEnemy runtimeEnemy = new RuntimeEnemy(enemySO);
            BaseEnemy baseEnemy = new BaseEnemy(runtimeEnemy, i);

            spawned.Add(baseEnemy);
            GameObject enemyGO = Instantiate(enemyVisualPrefab, spawnPoint.position, Quaternion.identity);

            EnemyUI enemyUI = enemyGO.GetComponent<EnemyUI>();
            SpriteRenderer visual = enemyGO.GetComponent<SpriteRenderer>();

            if (enemyUI == null || visual == null)
            {
                Debug.LogError("Enemy prefab missing EnemyUI or SpriteRenderer.");
                continue;
            }

            //bind HUD if available
            if (i < enemyHUDs.Length)
            {
                var hud = enemyHUDs[i];
                hud?.Bind(baseEnemy);
                enemyUI.Initialize(baseEnemy, hud);
            }
            else
            {
                enemyUI.Initialize(baseEnemy, null);
            }

            visual.sprite = runtimeEnemy.sprite;

            combatManager.RegisterEnemy(enemyUI);

            if (i == 0)
                currentEnemy = enemyUI;
        }

        //let the reel manager handle the new enemy reel creation
        if (enemyReelManager != null)
            enemyReelManager.PopulateReelsFromEnemies(spawned);

        return spawned;
    }

}
