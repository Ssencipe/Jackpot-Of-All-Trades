using UnityEngine;
using System;

public class Unit : MonoBehaviour, ITargetable
{
    public int maxHP;
    public int currentHP { get; private set; }
    public int currentShield { get; private set; }

    public event Action<int> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    public event Action<FloatingNumberData> OnFloatingNumber;

    public void TakeDamage(int amount)
    {
        int finalDamage = ApplyShield(amount);
        if (finalDamage > 0)
        {
            currentHP -= finalDamage;
            OnHealthChanged?.Invoke(currentHP);
        }

        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Damage));
    }
    private void Awake()
{
    currentHP = maxHP;       // Set health to full when the unit spawns
    currentShield = 0;       // Start with no shield
}
    private int ApplyShield(int amount)
    {
        if (currentShield > 0)
        {
            int remainingDamage = amount - currentShield;
            currentShield = Mathf.Max(0, currentShield - amount);
            OnShieldChanged?.Invoke(currentShield);
            return Mathf.Max(remainingDamage, 0);
        }
        return amount;
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        OnHealthChanged?.Invoke(currentHP);
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
        OnShieldChanged?.Invoke(currentShield);
    }

}
