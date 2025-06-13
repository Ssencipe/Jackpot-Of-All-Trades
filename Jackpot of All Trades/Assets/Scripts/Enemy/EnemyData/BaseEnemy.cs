using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : ITargetable
{
    public EnemySO baseData;
    public SpellSO selectedSpellToCast; //unnecessary but keeping around just in case

    public int currentHP { get; private set; }
    public int currentShield { get; private set; }
    public int positionIndex;
    public bool IsDead => currentHP <= 0;

    public List<BaseSpell> activeSpells;

    public event Action<int> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    public event Action<FloatingNumberData> OnFloatingNumber;

    public BaseEnemy(EnemySO so, int pos)
    {
        baseData = so;
        currentHP = so.maxHealth;
        currentShield = 0;
        positionIndex = pos;
        activeSpells = new List<BaseSpell>();
    }

    private int ApplyShield(int damage)
    {
        if (currentShield <= 0) return damage;

        int remainingDamage = damage - currentShield;
        currentShield = Mathf.Max(0, currentShield - damage);
        OnShieldChanged?.Invoke(currentShield);
        return Mathf.Max(remainingDamage, 0);
    }

    public void TakeDamage(int amount)
    {
        int finalDamage = ApplyShield(amount);
        if (finalDamage > 0)
        {
            currentHP -= finalDamage;
            OnHealthChanged?.Invoke(currentHP);
        }

        OnShieldChanged?.Invoke(currentShield);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Damage));
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, baseData.maxHealth);
        OnHealthChanged?.Invoke(currentHP);
        OnShieldChanged?.Invoke(currentShield);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Heal));  //spawn healing number
        FeedbackManager.Flash(this, FlashType.Heal); //sprite flashes green
    }

    public void GainShield(int amount)
    {
        currentShield += amount;
        OnShieldChanged?.Invoke(currentShield);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Shield));  //spawn shielding number
        FeedbackManager.Flash(this, FlashType.Shield); //sprite flashes blue
    }

    public void ResetShield()
    {
        currentShield = 0;
        OnShieldChanged?.Invoke(currentShield);
    }

    public void SetIntent(SpellSO intent)
    {
        selectedSpellToCast = intent;
    }
}
