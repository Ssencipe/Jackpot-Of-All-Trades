using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BattleDirector : MonoBehaviour
{
    [Header("Encounter Setup")]
    public List<EnemySO> encounterPool = new List<EnemySO>();
    public EnemyReelManager enemyReelManager;

    [Header("UI References")]
    public UnityEngine.UI.Button doneButton;
    public UnityEngine.UI.Button spinButton;

    [Header("Spell Display UI")]
    public SpellPreviewUI spellPreviewUI;

    public CombatManager combatManager;
    public GridManager gridManager;
    public SpawnManager spawnManager;
    public GridProcessor gridProcessor;

    private bool isPlayerTurn = true;
    private bool battleEnded = false;
    private bool waitingForPlayerDone = true;
    private WandAnimator wandAnimator;

    private void Start()
    {
        gridProcessor.Initialize(combatManager, gridManager);
        doneButton.onClick.AddListener(OnPlayerDonePressed);
        StartBattle();
    }

    private void StartBattle()
    {
        spawnManager.SpawnPlayer();
        List<BaseEnemy> enemies = spawnManager.SpawnEnemies(encounterPool);

        enemyReelManager.PopulateReelsFromEnemies(enemies);

        SetPlayerReelInteraction(false);

        StartCoroutine(SpinEnemyReels());

        wandAnimator = spawnManager.wandAnimator;
    }

    private IEnumerator SpinEnemyReels()
    {
        yield return StartCoroutine(enemyReelManager.RollIntentsCoroutine());
        yield return new WaitForSeconds(1f); // for reel spinning duration

        // Status effects for enemies
        combatManager.TickEnemyTurnEnd(); // End of enemy's turn

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        if (battleEnded) return;

        isPlayerTurn = true;
        waitingForPlayerDone = true;

        TargetingOverride.Clear();

        // Reset reels visually and logically
        FindObjectOfType<ReelSpawner>()?.ResetReels();

        // Reset player shield
        combatManager.ResetPlayerShield();

        // Status effects for player at turn start
        combatManager.TickPlayerTurnStart();

        // Enable player inputs
        SetPlayerReelInteraction(true);

        Debug.Log("Player's Turn: Spin to attack!");
    }

    public void OnPlayerDonePressed()
    {
        if (!isPlayerTurn || !waitingForPlayerDone || battleEnded)
            return;

        Debug.Log("Done button pressed — resolving player turn!");

        doneButton.interactable = false;
        waitingForPlayerDone = false;

        // Disable inputs, but keep visuals visible for conditional processing
        DisableReelInput();

        // Populate the spell grid from currently visible spells
        gridManager.PopulateGridFromSpin();

        // Begin coroutine to process player spells and then enemy turn
        StartCoroutine(ResolvePlayerTurn());
    }

    public void EnableDoneButton()
    {
        doneButton.interactable = true;
        waitingForPlayerDone = true;
    }

    private void DisableReelInput()
    {
        if (spinButton != null) spinButton.interactable = false;
        if (doneButton != null) doneButton.interactable = false;

        foreach (var cursor in FindObjectsOfType<ReelCursorHandler>())
            cursor.enabled = false;

        foreach (var click in FindObjectsOfType<ReelClickRegion>())
            click.enabled = false;
    }

    private IEnumerator ResolvePlayerTurn()
    {
        Debug.Log("Processing full grid for conditional effects...");

        BaseSpell[,] grid = gridManager.GetSpellGrid();

        // Leave reels visible while processing conditional effects
        yield return gridProcessor.ProcessGridForConditionalEffects(grid);

        // Let Unity render one frame before hiding reels
        yield return null;

        // Buffer delay after condition pass
        yield return new WaitForSeconds(1.5f);

        // Gather center spells for preview
        List<BaseSpell> centerSpells = new List<BaseSpell>();
        for (int x = 0; x < GridManager.Reels; x++)
        {
            if (grid[x, 1] != null)
                centerSpells.Add(grid[x, 1]);
        }

        // Hide reels after processing
        FindObjectOfType<ReelSpawner>()?.SetReelsVisible(false);

        // Show preview UI
        spellPreviewUI.Display(centerSpells);
        yield return new WaitForSeconds(1f);

        // Cast center row spells
        combatManager.ProcessPlayerActions(grid);
        yield return new WaitForSeconds(4f); // Simulated casting time

        spellPreviewUI.Clear();

        yield return wandAnimator.RotateTo(0f);
        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        if (battleEnded) yield break;

        isPlayerTurn = false;

        // Reset enemy shields
        combatManager.ResetEnemyShields();

        // Status effects for player and enemies
        combatManager.TickPlayerTurnEnd();   // End of player's turn
        combatManager.TickEnemyTurnStart();  // Start of enemy's turn

        yield return new WaitForSeconds(1f);

        Debug.Log("Enemy's Turn: Thinking...");

        // Sequentially process enemy actions
        yield return StartCoroutine(combatManager.ProcessEnemyActionsSequentially());

        // Begin spin phase for next turn
        StartCoroutine(SpinEnemyReels());
    }

    public void SetPlayerReelInteraction(bool isEnabled)
    {
        FindObjectOfType<ReelSpawner>()?.SetReelsVisible(isEnabled);

        if (spinButton != null) spinButton.interactable = isEnabled;
        if (doneButton != null) doneButton.interactable = isEnabled;

        foreach (var cursor in FindObjectsOfType<ReelCursorHandler>())
            cursor.enabled = isEnabled;

        foreach (var click in FindObjectsOfType<ReelClickRegion>())
            click.enabled = isEnabled;
    }

    public void EndBattle(bool playerWon)
    {
        battleEnded = true;
        Debug.Log(playerWon ? "Player Won!" : "Player Lost!");
    }
}