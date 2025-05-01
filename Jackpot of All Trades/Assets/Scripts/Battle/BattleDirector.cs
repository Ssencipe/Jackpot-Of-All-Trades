using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDirector : MonoBehaviour
{
    [Header("Encounter Setup")]
    public List<EnemySO> encounterPool = new List<EnemySO>();

    [Header("UI References")]
    public Button doneButton;

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
        spawnManager.SpawnEnemies(encounterPool);

        wandAnimator = spawnManager.wandAnimator;

        battleFlow.ShowBattleScreen();
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        if (battleEnded) return;

        isPlayerTurn = true;
        waitingForPlayerDone = true;

        // Reset reels visually and logically
        FindObjectOfType<ReelSpawner>()?.ResetReels();

        // Reset player shield
        combatManager.ResetPlayerShield();

        Debug.Log("Player's Turn: Spin to attack!");
        battleFlow.ShowSlotMachineScreen();
    }

    public void OnPlayerDonePressed()
    {
        if (!isPlayerTurn || !waitingForPlayerDone || battleEnded)
            return;

        Debug.Log("Done button pressed — resolving player turn!");

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

        // Play spells DURING the lowered wand state
        BaseSpell[,] grid = gridManager.GetSpellGrid();
        combatManager.ProcessPlayerActions(grid);

        // Wait to simulate casting animations (customize if you have animation logic later)
        yield return new WaitForSeconds(4f);

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

        yield return new WaitForSeconds(1f);

        Debug.Log("Enemy's Turn: Thinking...");
        combatManager.ProcessEnemyActions();

        yield return new WaitForSeconds(1f);

        StartPlayerTurn();
    }

    public void EndBattle(bool playerWon)
    {
        battleEnded = true;
        Debug.Log(playerWon ? "Player Won!" : "Player Lost!");
    }
}
