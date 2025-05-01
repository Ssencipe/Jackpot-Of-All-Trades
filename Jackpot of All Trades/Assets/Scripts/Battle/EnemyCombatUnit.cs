using UnityEngine;
using UnityEngine.UI;

public class EnemyCombatUnit : MonoBehaviour
{
    public BattleHUD hud;
    public BaseEnemy baseEnemy;

    [Header("UI")]
    public Image intentIcon;

    public void Initialize(BaseEnemy baseData, BattleHUD enemyHUD)
    {
        Debug.Log("Initializing EnemyCombatUnit...");
        baseEnemy = baseData;
        hud = enemyHUD;
        hud?.Bind(baseEnemy);
    }

    public bool TakeDamage(int amount)
    {
        baseEnemy.TakeDamage(amount);
        return baseEnemy.currentHealth <= 0;
    }

    public void Heal(int amount)
    {
        baseEnemy.Heal(amount);
    }

    public void GainShield(int amount)
    {
        baseEnemy.GainShield(amount);
    }

    public void ResetShield()
    {
        baseEnemy.ResetShield();

        if (hud != null)
        {
            hud.SetShield(baseEnemy.currentShield);
        }
    }

    public void PerformAction(Unit playerUnit)
    {
        SpellSO intent = baseEnemy.nextIntentSpell;

        if (intent == null)
        {
            Debug.Log($"{baseEnemy.baseData.enemyName} has no intent spell set!");
            return;
        }

        Debug.Log($"{baseEnemy.baseData.enemyName} performs intent: {intent.spellName}");

        BaseSpell spellToCast = new BaseSpell(intent, -1, -1);
        spellToCast.Cast(FindObjectOfType<CombatManager>(), FindObjectOfType<GridManager>(), true);
    }
}
