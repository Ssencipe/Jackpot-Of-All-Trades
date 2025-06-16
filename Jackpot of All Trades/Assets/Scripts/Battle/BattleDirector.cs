using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDirector : MonoBehaviour
{
    [Header("Encounter Setup")]
    public List<EnemySO> encounterPool = new List<EnemySO>();
    public EnemyReelManager enemyReelManager;
    [Header("UI References")]
    public Button doneButton;

    [Header("Spell Display UI")]
    public SpellPreviewUI spellPreviewUI;

    public BattleFlow battleFlow;
    public CombatManager combatManager;
    public GridManager gridManager;
    public SpawnManager spawnManager;

    private bool isPlayerTurn = true;
    private bool battleEnded = false;
    private bool waitingForPlayerDone = true;
    private WandAnimator wandAnimator;

    private void Start()
    {
        doneButton.onClick.AddListener(OnPlayerDonePressed);
        StartBattle();
    }

    private void StartBattle()
    {
        spawnManager.SpawnPlayer();
        List<BaseEnemy> enemies = spawnManager.SpawnEnemies(encounterPool);

        enemyReelManager.PopulateReelsFromEnemies(enemies);
        StartCoroutine(SpinEnemyReels());


        wandAnimator = spawnManager.wandAnimator;

        battleFlow.ShowBattleScreen();
        StartPlayerTurn();
    }

    private IEnumerator SpinEnemyReels()
    {
        yield return StartCoroutine(enemyReelManager.RollIntentsCoroutine());
        yield return new WaitForSeconds(4f); //for reel spinning duration

        //for status effects
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

        //for status effects
        combatManager.TickPlayerTurnStart(); //for player turn start

        Debug.Log("Player's Turn: Spin to attack!");
    }

    public void OnPlayerDonePressed()
    {
        if (!isPlayerTurn || !waitingForPlayerDone || battleEnded)
            return;

        Debug.Log("Done button pressed � resolving player turn!");

        doneButton.interactable = false;
        waitingForPlayerDone = false;

        // Populate the spell grid first
        gridManager.PopulateGridFromSpin();

        // Begin coroutine to process player spells and then enemy turn
        StartCoroutine(ResolvePlayerTurn());
    }


    public void EnableDoneButton()
    {
        doneButton.interactable = true;
        waitingForPlayerDone = true;
    }

    // This method processes the player's spell grid. Called when player presses Done.

    private IEnumerator ResolvePlayerTurn()
    {
        Debug.Log("Resolving player spell grid...");
        battleFlow.ShowBattleScreen();

        // Rotate wand down (cast prep)
        yield return wandAnimator.RotateTo(35f);

        // Prepare spells for processing and visuals
        BaseSpell[,] grid = gridManager.GetSpellGrid();
        List<BaseSpell> centerSpells = new List<BaseSpell>();

        for (int x = 0; x < GridManager.Reels; x++)
        {
            if (grid[x, 1] != null)
                centerSpells.Add(grid[x, 1]);
        }

        // Show spell display UI
        spellPreviewUI.Display(centerSpells);
        yield return new WaitForSeconds(1f);

        // Then cast them through CombatManager using the full grid
        combatManager.ProcessPlayerActions(grid);

        // Wait to simulate casting animations (customize if you have animation logic later)
        yield return new WaitForSeconds(4f);

        //Clean up spell preview UI
        spellPreviewUI.Clear();

        // Rotate wand back up (return to idle)
        yield return wandAnimator.RotateTo(0f);

        // Wait before enemy turn
        yield return new WaitForSeconds(0.3f);

        // Now proceed to enemy turn
        yield return StartCoroutine(StartEnemyTurn());
    }

    private IEnumerator StartEnemyTurn()
    {
        if (battleEnded) yield break;

        isPlayerTurn = false;

        combatManager.ResetEnemyShields();

        //for status effects
        combatManager.TickPlayerTurnEnd(); // End of player's turn
        combatManager.TickEnemyTurnStart(); // Start of enemy's turn

        yield return new WaitForSeconds(1f);

        Debug.Log("Enemy's Turn: Thinking...");

        // Sequentially perform actions
        yield return StartCoroutine(combatManager.ProcessEnemyActionsSequentially());

        //spin actions for next turn
        StartCoroutine(SpinEnemyReels());
    }

    public void EndBattle(bool playerWon)
    {
        battleEnded = true;
        Debug.Log(playerWon ? "Player Won!" : "Player Lost!");
    }
}
