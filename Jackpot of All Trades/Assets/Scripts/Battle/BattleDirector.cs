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

        SetPlayerReelInteraction(false);

        StartCoroutine(SpinEnemyReels());

        wandAnimator = spawnManager.wandAnimator;
    }

    private IEnumerator SpinEnemyReels()
    {
        yield return StartCoroutine(enemyReelManager.RollIntentsCoroutine());
        yield return new WaitForSeconds(4f); // for reel spinning duration

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

    // Disables interactivity on player reels without hiding the visuals
    private void DisableReelInput()
    {
        if (spinButton != null) spinButton.interactable = false;
        if (doneButton != null) doneButton.interactable = false;

        foreach (var cursor in FindObjectsOfType<ReelCursorHandler>())
            cursor.enabled = false;

        foreach (var click in FindObjectsOfType<ReelClickRegion>())
            click.enabled = false;
    }

    // This method processes the player's spell grid. Called when player presses Done.
    private IEnumerator ResolvePlayerTurn()
    {
        Debug.Log("Processing full grid for conditional effects...");

        BaseSpell[,] grid = gridManager.GetSpellGrid();

        // Leave reels visible while processing conditional effects
        yield return ProcessGridForConditionalEffects(grid);

        // Let Unity render one frame before hiding reels
        yield return null;
        yield return new WaitForSeconds(0.25f); // Optional visual buffer

        // Gather center spells for preview
        List<BaseSpell> centerSpells = new List<BaseSpell>();
        for (int x = 0; x < GridManager.Reels; x++)
        {
            if (grid[x, 1] != null)
                centerSpells.Add(grid[x, 1]);
        }

        // Hide reels after condition effects had time to show
        FindObjectOfType<ReelSpawner>()?.SetReelsVisible(false);

        // Show center spell preview
        spellPreviewUI.Display(centerSpells);
        yield return new WaitForSeconds(1f);

        // Cast center row spells
        combatManager.ProcessPlayerActions(grid);

        yield return new WaitForSeconds(4f); // Simulate casting animations
        spellPreviewUI.Clear();

        yield return wandAnimator.RotateTo(0f);
        yield return new WaitForSeconds(0.3f);

        // Begin enemy turn
        yield return StartCoroutine(StartEnemyTurn());
    }

    // Evaluates and applies conditional effects before main spell casting
    private IEnumerator ProcessGridForConditionalEffects(BaseSpell[,] grid)
    {
        for (int y = 0; y < GridManager.SlotsPerReel; y++)
        {
            for (int x = 0; x < GridManager.Reels; x++)
            {
                var spell = grid[x, y];
                if (spell == null) continue;

                var spellSO = spell.spellData;
                if (spellSO.conditions == null || spellSO.conditions.Count == 0) continue;

                var context = new SpellCastContext
                {
                    spellInstance = spell,
                    combat = combatManager,
                    grid = gridManager,
                    isEnemyCaster = false,
                    playerCaster = combatManager.playerUnit,
                    enemyTeam = combatManager.CurrentEnemies.ToList()
                };

                foreach (var condition in spellSO.conditions)
                {
                    if (condition.Evaluate(context))
                    {
                        switch (condition.GetResultType())
                        {
                            case ConditionResultType.TriggerEffect:
                                var effect = condition.GetLinkedEffect();
                                if (effect != null)
                                {
                                    var targets = TargetingManager.ResolveTargets(
                                        effect.GetTargetType(),
                                        effect.GetTargetingMode(),
                                        new TargetingContext
                                        {
                                            isEnemyCaster = false,
                                            combat = combatManager,
                                            grid = gridManager,
                                            playerCaster = combatManager.playerUnit,
                                            enemyCaster = null,
                                            enemyTeam = combatManager.CurrentEnemies.ToList()
                                        });

                                    effect.Apply(context, targets);

                                    // Visual feedback (pulse reel)
                                    var reel = gridManager.linkedReels[x];
                                    reel?.PlayEffectAtSlot(y);

                                    yield return new WaitForSeconds(1f);
                                }
                                break;

                            case ConditionResultType.ModifyPotency:
                                context.spellInstance.runtimeSpell.ApplyPotencyMultiplier(condition.GetPotencyMultiplier());
                                break;

                            case ConditionResultType.SkipSpell:
                                spell.runtimeSpell.isDisabled = true;
                                break;
                        }
                    }
                }
            }
        }
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

    // Controls full reel interaction (visual + logic)
    public void SetPlayerReelInteraction(bool isEnabled)
    {
        // Toggle reel visuals
        FindObjectOfType<ReelSpawner>()?.SetReelsVisible(isEnabled);

        // Toggle spin and done buttons
        if (spinButton != null) spinButton.interactable = isEnabled;
        if (doneButton != null) doneButton.interactable = isEnabled;

        // Toggle reel-related UI input scripts
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