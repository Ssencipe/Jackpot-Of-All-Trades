using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("HUD & Visuals")]
    public BattleHUD hud;
    public EnemyReel reel;

    // Reference to the core data/model for this enemy.
    public BaseEnemy BaseEnemy { get; private set; }

    // Initializes the enemy unit with its data and UI.
    public void Initialize(BaseEnemy baseEnemy, BattleHUD assignedHUD)
    {
        Debug.Log("Initializing EnemyCombatUnit...");

        BaseEnemy = baseEnemy;
        hud = assignedHUD;
        hud?.Bind(BaseEnemy);
    }

    // Executes the enemy's current intent spell.
    public void PerformAction(Unit playerUnit)
    {
        SpellSO spell = BaseEnemy.selectedSpellToCast;
        if (spell == null)
        {
            Debug.Log($"{BaseEnemy.baseData.enemyName} has no spell selected!");
            return;
        }

        Debug.Log($"{BaseEnemy.baseData.enemyName} casting: {spell.spellName}");
        BaseSpell toCast = new BaseSpell(spell, -1, -1);
        toCast.Cast(FindObjectOfType<CombatManager>(), FindObjectOfType<GridManager>(), true, BaseEnemy);
    }

    public void DeactivateVisuals()
    {
        if (reel != null)
            reel.gameObject.SetActive(false);

        if (hud != null)
            hud.gameObject.SetActive(false);
    }
}