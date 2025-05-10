using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("HUD & Visuals")]
    public BattleHUD hud;
    public Image intentIcon;

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

    //Bind intent HUD element to corresponging enemy
    public void BindIntentIcon(Image icon)
    {
        intentIcon = icon;
    }

    //change the intent sprite
    public void ShowIntent()
    {
        if (intentIcon == null)
        {
            Debug.LogWarning("[EnemyUI] Intent icon not bound.");
            return;
        }

        if (BaseEnemy.nextIntentSpell == null)
        {
            Debug.LogWarning("[EnemyUI] No intent spell set.");
            intentIcon.enabled = false;
            return;
        }

        intentIcon.sprite = BaseEnemy.nextIntentSpell.icon;
        intentIcon.enabled = true;
        intentIcon.gameObject.SetActive(true);

        Debug.Log($"[EnemyUI] Intent updated: {BaseEnemy.nextIntentSpell.spellName}");
    }

}