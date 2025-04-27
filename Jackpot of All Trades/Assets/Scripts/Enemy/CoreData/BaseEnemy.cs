using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy
{
    public EnemySO baseData;
    public int currentHealth;

    public int positionIndex; // 0 = left, 1 = center, 2 = right

    public List<BaseSpell> activeSpells; // Reel outputs

    public BaseEnemy(EnemySO so, int pos)
    {
        baseData = so;
        currentHealth = so.maxHealth;
        positionIndex = pos;
        activeSpells = new List<BaseSpell>(); // Will be filled on spin
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{baseData.enemyName} takes {amount} damage. Remaining HP: {currentHealth}");
    }

    public void CastAllSpells(CombatManager combat, GridManager grid)
    {
        foreach (var spell in activeSpells)
        {
            spell.Cast(combat, grid);
        }
    }
}
