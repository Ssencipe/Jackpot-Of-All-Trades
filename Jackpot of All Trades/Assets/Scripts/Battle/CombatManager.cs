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

    public void TickPlayerTurnStart() =>    //status effect for player at turn start
    playerUnit.GetComponent<StatusEffectController>()?.TickTurnStart(playerUnit);

    public void TickPlayerTurnEnd() =>      //status effect for player at turn end
        playerUnit.GetComponent<StatusEffectController>()?.TickTurnEnd(playerUnit);

    //status effect for enemy at start of turn
    public void TickEnemyTurnStart()
    {
        playerUnit.GetComponent<StatusEffectController>()?.TickTurnStart(playerUnit);

        // Tick enemy effects
        foreach (var enemyUI in activeEnemyUIs.ToList())  // Use ToList to safely modify list
        {
            var enemy = enemyUI.BaseEnemy;
            if (!enemy.IsDead)
            {
                enemyUI.GetComponent<StatusEffectController>()?.TickTurnStart(enemy);
            }

            //Check if the status effect killed the enemy
            if (enemy.IsDead)
            {
                RemoveEnemy(enemy);
            }
        }
    }

    //status effect for enemy at end of turn
    public void TickEnemyTurnEnd()
    {
        playerUnit.GetComponent<StatusEffectController>()?.TickTurnEnd(playerUnit);

        // Tick enemy effects
        foreach (var enemyUI in activeEnemyUIs.ToList())  // Use ToList to safely modify list
        {
            var enemy = enemyUI.BaseEnemy;
            if (!enemy.IsDead)
            {
                enemyUI.GetComponent<StatusEffectController>()?.TickTurnEnd(enemy);
            }

            //Check if the status effect killed the enemy
            if (enemy.IsDead)
            {
                RemoveEnemy(enemy);
            }
        }
    }


    public void DealDamage(BaseEnemy target, int amount)
    {
        if (target == null) return;

        target.TakeDamage(amount);

        //screeen shake, hit stop, flash effect
        FeedbackManager.HitStop(0.05f, this);
        FeedbackManager.ShakeCamera();
        FeedbackManager.Flash(target, FlashType.Damage);

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
        StartCoroutine(ProcessEnemyActionsSequentially());
    }

    // does each enemy action sequentially from left to right
    public IEnumerator ProcessEnemyActionsSequentially()
    {
        Debug.Log("Processing Enemy Actions Sequentially...");

        foreach (var enemyUI in activeEnemyUIs)
        {
            if (enemyUI.BaseEnemy.IsDead) continue;

            enemyUI.PerformAction(playerUnit);

            yield return new WaitForSeconds(1f); // brief pause before next
        }

        CheckDefeat();
    }

    private void RemoveEnemy(BaseEnemy baseEnemy)
    {
        var ui = activeEnemyUIs.FirstOrDefault(e => e.BaseEnemy == baseEnemy);
        if (ui != null)
        {
            ui.DeactivateVisuals();
            activeEnemyUIs.Remove(ui);
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
