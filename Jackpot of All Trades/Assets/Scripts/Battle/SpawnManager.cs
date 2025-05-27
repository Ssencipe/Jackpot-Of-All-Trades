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
    public Image[] intentIcons;
    public BattleHUD[] enemyHUDs;

    [Header("Prefab References")]
    public GameObject playerPrefab;
    public GameObject enemyVisualPrefab;

    [Header("Manager References")]
    public CombatManager combatManager; 

    [Header("Spawner References")]
    public ReelSpawner reelSpawner;

    private EnemyUI currentEnemy;
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

        // Use the GameObject's transform for position and parenting
        Transform spawnTransform = playerSpawnObject.transform;

        GameObject playerGO = Instantiate(playerPrefab, spawnTransform.position, Quaternion.identity, spawnTransform);

        // Get WandAnimator from the instantiated player
        wandAnimator = playerGO.GetComponentInChildren<WandAnimator>();

        Unit playerUnit = playerGO.GetComponent<Unit>();
        if (playerUnit == null)
        {
            Debug.LogError("Player prefab missing Unit script!");
            return;
        }

        combatManager.RegisterPlayer(playerUnit);
        playerHUD?.Bind(playerUnit);

        // Pass the player GameObject to the reel spawner
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

        // Spawns enemies and binds data
        List<BaseEnemy> spawned = new List<BaseEnemy>();
        int count = Mathf.Min(encounterPool.Count, enemySpawnPoints.Length);
        for (int i = 0; i < count; i++)
        {
            EnemySO enemySO = encounterPool[i];
            Transform spawnPoint = enemySpawnPoints[i];

            BaseEnemy baseEnemy = new BaseEnemy(enemySO, i);
            spawned.Add(baseEnemy);
            GameObject enemyGO = Instantiate(enemyVisualPrefab, spawnPoint.position, Quaternion.identity);

            EnemyUI enemyUI = enemyGO.GetComponent<EnemyUI>();
            SpriteRenderer visual = enemyGO.GetComponent<SpriteRenderer>();

            if (enemyUI == null || visual == null)
            {
                Debug.LogError("Enemy prefab missing EnemyUI or SpriteRenderer.");
                continue;
            }

            // Bind HUD if available
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

            // Bind intent icon if available
            if (i < intentIcons.Length)
            {
                enemyUI.BindIntentIcon(intentIcons[i]);
            }

            visual.sprite = enemySO.sprite;

            combatManager.RegisterEnemy(enemyUI);
            baseEnemy.RollIntent(); // Generate starting intent
            enemyUI.ShowIntent();

            if (i == 0)
                currentEnemy = enemyUI; // for legacy compatibility
        }
        return spawned;
    }

    // Set up intent in EnemyUI
    public void HandleEnemyIntentAfterAction()
    {
        foreach (var enemyUI in combatManager.activeEnemyUIs)
        {
            enemyUI.BaseEnemy.RollIntent();
            enemyUI.ShowIntent();
        }
    }
}
