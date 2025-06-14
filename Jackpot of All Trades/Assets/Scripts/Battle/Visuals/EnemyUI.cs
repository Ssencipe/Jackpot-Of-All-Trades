using UnityEngine;
using System.Collections;

public class EnemyUI : MonoBehaviour
{
    [Header("HUD & Visuals")]
    public BattleHUD hud;
    public EnemyReel reel;

    [SerializeField] private GameObject actionIndicator;

    // Reference to the core data/model for this enemy.
    public BaseEnemy BaseEnemy { get; private set; }

    // Initializes the enemy unit with its data and UI.
    public void Initialize(BaseEnemy baseEnemy, BattleHUD assignedHUD)
    {
        Debug.Log("Initializing EnemyCombatUnit...");

        BaseEnemy = baseEnemy;
        hud = assignedHUD;
        hud?.Bind(BaseEnemy);
        actionIndicator?.SetActive(false);
    }

    // Executes the enemy's current intent spell.
    public void PerformAction(Unit playerUnit)
    {
        //show indicator that enemy is active
        ShowActionIndicator(true);

        SpellSO spell = BaseEnemy.selectedSpellToCast;
        if (spell == null)
        {
            Debug.Log($"{BaseEnemy.baseData.enemyName} has no spell selected!");
            return;
        }

        Debug.Log($"{BaseEnemy.baseData.enemyName} casting: {spell.spellName}");
        BaseSpell toCast = new BaseSpell(spell, -1, -1);
        toCast.Cast(FindObjectOfType<CombatManager>(), FindObjectOfType<GridManager>(), true, BaseEnemy);

        // Deactivate indicator after action is finished
        StartCoroutine(HideIndicatorAfterDelay(1f));
    }

    public void DeactivateVisuals()
    {
        if (reel != null)
            reel.gameObject.SetActive(false);

        if (hud != null)
            hud.gameObject.SetActive(false);
    }

    //for visual element that appers below active enemy
    public void ShowActionIndicator(bool show)
    {
        if (actionIndicator != null)
            actionIndicator.SetActive(show);
    }
    private IEnumerator HideIndicatorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowActionIndicator(false);
    }
}