using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("HUD & Visuals")]
    public BattleHUD hud;
    public Image intentIcon;

    /// <summary>
    /// Reference to the core data/model for this enemy.
    /// </summary>
    public BaseEnemy BaseEnemy { get; private set; }

    /// <summary>
    /// Initializes the enemy unit with its data and UI.
    /// </summary>
    /// <param name="baseData">Core logic object for the enemy.</param>
    /// <param name="enemyHUD">HUD to bind enemy state to.</param>
    public void Initialize(BaseEnemy baseData, BattleHUD enemyHUD)
    {
        Debug.Log("Initializing EnemyCombatUnit...");

        BaseEnemy = baseData;
        hud = enemyHUD;
        hud?.Bind(BaseEnemy);
    }

    /// <summary>
    /// Executes the enemy's current intent spell.
    /// </summary>
    /// <param name="playerUnit">The player, used as the default target.</param>
    public void PerformAction(Unit playerUnit)
    {
        SpellSO intent = BaseEnemy.nextIntentSpell;

        if (intent == null)
        {
            Debug.Log($"{BaseEnemy.baseData.enemyName} has no intent spell set!");
            return;
        }

        Debug.Log($"{BaseEnemy.baseData.enemyName} performs intent: {intent.spellName}");

        BaseSpell spellToCast = new BaseSpell(intent, -1, -1);
        var combat = FindObjectOfType<CombatManager>();
        var grid = FindObjectOfType<GridManager>();

        spellToCast.Cast(combat, grid, true);
    }
}