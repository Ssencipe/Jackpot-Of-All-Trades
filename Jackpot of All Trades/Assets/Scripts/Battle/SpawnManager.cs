using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    public GameObject playerSpawnObject;           // Changed from Transform to GameObject
    public Transform enemySpawnPoint;

    [Header("HUD References")]
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    [Header("Prefab References")]
    public GameObject playerPrefab;
    public GameObject enemyVisualPrefab;

    [Header("Manager References")]
    public CombatManager combatManager;

    [Header("Intent UI")]
    public Image enemyIntentIcon;

    [Header("Spawner References")]
    public ReelSpawner reelSpawner;

    private EnemyCombatUnit currentEnemy;
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

    public void SpawnEnemies(List<EnemySO> encounterPool)
    {
        Debug.Log($"Spawning enemies from pool: {encounterPool.Count} entries");

        if (encounterPool == null || encounterPool.Count == 0)
        {
            Debug.LogWarning("Encounter pool is empty.");
            return;
        }

        // Support one enemy for now
        EnemySO enemySO = encounterPool[0];
        BaseEnemy enemyData = new BaseEnemy(enemySO, 0);

        GameObject enemyGO = Instantiate(enemyVisualPrefab, enemySpawnPoint.position, Quaternion.identity);

        EnemyCombatUnit enemyUnit = enemyGO.GetComponent<EnemyCombatUnit>();
        SpriteRenderer visual = enemyGO.GetComponent<SpriteRenderer>();

        if (enemyUnit == null || visual == null)
        {
            Debug.LogError("Enemy prefab must have EnemyCombatUnit and SpriteRenderer!");
            return;
        }

        // Set data and bind to HUD
        enemyUnit.Initialize(enemyData, enemyHUD);
        enemyHUD?.Bind(enemyData);
        visual.sprite = enemySO.sprite;
        combatManager.RegisterEnemy(enemyUnit);

        // Roll first intent and show it immediately
        enemyData.RollIntent();
        UpdateIntentUI(enemyData.nextIntentSpell);
        currentEnemy = enemyUnit;
    }

    public void UpdateIntentUI(SpellSO spell)
    {
        if (enemyIntentIcon != null && spell != null)
        {
            enemyIntentIcon.sprite = spell.icon;
            enemyIntentIcon.enabled = true;
        }
    }

    public void HandleEnemyIntentAfterAction()
    {
        if (currentEnemy != null && currentEnemy.baseEnemy != null)
        {
            currentEnemy.baseEnemy.RollIntent();
            UpdateIntentUI(currentEnemy.baseEnemy.nextIntentSpell);
        }
    }
}
