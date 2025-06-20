using System.Collections.Generic;
using System;
using UnityEngine;

public class BaseEnemy : ITargetable
{
    public RuntimeEnemy runtimeData;
    public RuntimeSpell selectedSpellToCast;

    public int currentHP { get; private set; }
    public int currentShield { get; private set; }
    public int positionIndex;
    public bool IsDead => currentHP <= 0;

    public List<BaseSpell> activeSpells;
    public GameObject visualGameObject; //assigned by EnemyUI when created

    public StatusEffectController StatusEffects => visualGameObject?.GetComponent<StatusEffectController>();

    public event Action<int> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    public event Action<FloatingNumberData> OnFloatingNumber;

    //constructor using SO data as backup
    public BaseEnemy(EnemySO so, int pos)
    {
        runtimeData = new RuntimeEnemy(so);
        currentHP = runtimeData.maxHealth;
        currentShield = 0;
        positionIndex = pos;
        activeSpells = new List<BaseSpell>();
    }

    //constructor using runtime data
    public BaseEnemy(RuntimeEnemy runtime, int pos)
    {
        runtimeData = runtime;
        currentHP = runtime.maxHealth;
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
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Damage));    //enemy flashes red
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, runtimeData.maxHealth);
        OnHealthChanged?.Invoke(currentHP);
        OnShieldChanged?.Invoke(currentShield);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Heal));  //spawn healing number
        FeedbackManager.Flash(this, FlashType.Heal);    //enemy flashes green
    }

    public void GainShield(int amount)
    {
        currentShield += amount;
        OnShieldChanged?.Invoke(currentShield);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Shield));    //spawn shielding number
        FeedbackManager.Flash(this, FlashType.Shield);  //enemy flashes blue
    }

    public void ResetShield()
    {
        currentShield = 0;
        OnShieldChanged?.Invoke(currentShield);
    }

    public void SetIntent(RuntimeSpell intent)
    {
        selectedSpellToCast = intent;
    }
}