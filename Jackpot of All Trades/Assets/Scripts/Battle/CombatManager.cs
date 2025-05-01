using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    public List<EnemyCombatUnit> currentEnemies = new List<EnemyCombatUnit>();
    public Unit playerUnit;

    public void RegisterPlayer(Unit player)
    {
        playerUnit = player;
    }

    public void RegisterEnemy(EnemyCombatUnit enemy)
    {
        currentEnemies.Add(enemy);
    }

    public EnemyCombatUnit GetLeftmostEnemy()
    {
        return currentEnemies.Count > 0 ? currentEnemies[0] : null;
    }

    public void DealDamage(EnemyCombatUnit target, int amount)
    {
        if (target == null) return;

        bool killed = target.TakeDamage(amount);
        if (killed)
        {
            currentEnemies.Remove(target);
            Object.Destroy(target.gameObject);
            Debug.Log("Enemy defeated!");
        }
    }

    public void DealDamageToPlayer(int amount)
    {
        playerUnit.TakeDamage(amount);
    }

    public void HealPlayer(int amount)
    {
        playerUnit.Heal(amount);
    }

    public void ShieldPlayer(int amount)
    {
        playerUnit.GainShield(amount);
    }

    public void ResetPlayerShield()
    {
        playerUnit.ResetShield();
    }

    public void ResetEnemyShields()
    {
        foreach (var enemy in currentEnemies)
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
            BaseSpell spell = grid[x, 1]; // center row only
            if (spell != null)
            {
                Debug.Log($"Casting spell at column {x}: {spell.spellData.spellName}");
                spell.Cast(this, FindObjectOfType<GridManager>(), false);

                yield return new WaitForSeconds(0.6f); // Slight pause between spells
            }
        }

        yield return new WaitForSeconds(1f); // Extra pause before checking outcome
        CheckVictory();

        // Let BattleDirector know it's safe to press Done now (optional)
        FindObjectOfType<BattleDirector>().EnableDoneButton();
    }

    public void ProcessEnemyActions()
    {
        Debug.Log("Processing Enemy Actions...");

        if (currentEnemies.Count == 0)
            return;

        var actingEnemy = GetLeftmostEnemy();
        if (actingEnemy != null)
        {
            actingEnemy.PerformAction(playerUnit);

            // After action, roll next intent and show it
            FindObjectOfType<SpawnManager>()?.HandleEnemyIntentAfterAction();
        }

        CheckDefeat();
    }

    private void CheckVictory()
    {
        if (currentEnemies.Count <= 0)
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
