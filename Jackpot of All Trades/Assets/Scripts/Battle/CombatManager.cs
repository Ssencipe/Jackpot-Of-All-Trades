using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public Unit playerUnit;
    public List<EnemyUI> activeEnemyUIs = new List<EnemyUI>();
    public IEnumerable<BaseEnemy> CurrentEnemies => activeEnemyUIs.Select(e => e.BaseEnemy);

    public void RegisterPlayer(Unit player)
    {
        playerUnit = player;
    }

    public void RegisterEnemy(EnemyUI enemyUI)
    {
        if (!activeEnemyUIs.Contains(enemyUI))
            activeEnemyUIs.Add(enemyUI);
    }

    public EnemyUI GetEnemyUI()
    {
        return activeEnemyUIs.Count > 0 ? activeEnemyUIs[0] : null;
    }

    public BaseEnemy GetEnemy()
    {
        return GetEnemyUI()?.BaseEnemy;
    }

    public void DealDamage(BaseEnemy target, int amount)
    {
        if (target == null) return;

        target.TakeDamage(amount);
        if (target.IsDead)
        {
            RemoveEnemy(target);
        }
    }

    public void DealDamageToPlayer(int amount)
    {
        playerUnit.TakeDamage(amount);
    }

    public void HealPlayer(int amount)
    {
        Debug.Log($"[CombatManager] Healing player for {amount}");
        playerUnit.Heal(amount);
    }

    public void ShieldPlayer(int amount)
    {
        Debug.Log($"[CombatManager] Shielding player for {amount}");
        playerUnit.GainShield(amount);
    }

    public void ResetPlayerShield()
    {
        playerUnit.ResetShield();
    }

    public void ResetEnemyShields()
    {
        foreach (var enemy in CurrentEnemies)
        {
            enemy.ResetShield();
        }
    }

    public void ProcessPlayerActions(BaseSpell[,] grid)
    {
        Debug.Log("Starting spell resolution...");
        StartCoroutine(ResolvePlayerSpells(grid));
    }

    private IEnumerator ResolvePlayerSpells(BaseSpell[,] grid)
    {
        for (int x = 0; x < GridManager.Reels; x++)
        {
            BaseSpell spell = grid[x, 1];
            if (spell != null)
            {
                Debug.Log($"Casting spell at column {x}: {spell.spellData.spellName}");
                spell.Cast(this, FindObjectOfType<GridManager>(), false);
                yield return new WaitForSeconds(0.6f);
            }
        }

        yield return new WaitForSeconds(1f);
        CheckVictory();

        FindObjectOfType<BattleDirector>().EnableDoneButton();
    }

    public void ProcessEnemyActions()
    {
        Debug.Log("Processing Enemy Actions...");

        foreach (var enemyUI in activeEnemyUIs)
        {
            if (enemyUI.BaseEnemy.IsDead) continue;

            enemyUI.PerformAction(playerUnit);
            enemyUI.BaseEnemy.RollIntent(); // roll next intent
            enemyUI.ShowIntent(); //update intent sprites
        }

        CheckDefeat();
    }

    private void RemoveEnemy(BaseEnemy baseEnemy)
    {
        var ui = activeEnemyUIs.FirstOrDefault(e => e.BaseEnemy == baseEnemy);
        if (ui != null)
        {
            activeEnemyUIs.Remove(ui);
            ui.ClearIntent();
            Destroy(ui.gameObject);
        }
    }

    private void CheckVictory()
    {
        if (!CurrentEnemies.Any(e => !e.IsDead))
        {
            FindObjectOfType<BattleDirector>()?.EndBattle(true);
        }
    }

    private void CheckDefeat()
    {
        if (playerUnit.currentHP <= 0)
        {
            FindObjectOfType<BattleDirector>()?.EndBattle(false);
        }
    }
}
