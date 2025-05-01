using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy
{
    public EnemySO baseData;
    public SpellSO nextIntentSpell;
    public int currentHealth;
    public int currentShield;
    public int positionIndex;

    public List<BaseSpell> activeSpells;

    public event Action<int> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    public event Action<FloatingNumberData> OnFloatingNumber;

    public BaseEnemy(EnemySO so, int pos)
    {
        baseData = so;
        currentHealth = so.maxHealth;
        currentShield = 0;
        positionIndex = pos;
        activeSpells = new List<BaseSpell>();
    }

    public void TakeDamage(int amount)
    {
        if (currentShield > 0)
        {
            int remainingDamage = amount - currentShield;
            currentShield -= amount;
            if (currentShield < 0) currentShield = 0;
            OnShieldChanged?.Invoke(currentShield);

            if (remainingDamage > 0)
                currentHealth -= remainingDamage;
        }
        else
        {
            currentHealth -= amount;
        }

        OnHealthChanged?.Invoke(currentHealth);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Damage));
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, baseData.maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Heal));
    }

    public void GainShield(int amount)
    {
        currentShield += amount;
        OnShieldChanged?.Invoke(currentShield);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Shield));
    }

    public void ResetShield()
    {
        currentShield = 0;
        Debug.Log($"{baseData.enemyName}'s shield resets to 0.");
    }

    public void CastAllSpells(CombatManager combat, GridManager grid)
    {
        foreach (var spell in activeSpells)
        {
            spell.Cast(combat, grid, true);
        }
    }
    public void RollIntent()
    {
        if (baseData.spellPool == null || baseData.spellPool.Count == 0)
        {
            Debug.LogWarning($"{baseData.enemyName} has no spells to pick from!");
            return;
        }

        nextIntentSpell = baseData.spellPool[UnityEngine.Random.Range(0, baseData.spellPool.Count)];
        Debug.Log($"{baseData.enemyName} intent set: {nextIntentSpell.spellName}");
    }
}
