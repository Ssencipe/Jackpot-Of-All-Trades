using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;
    public int maxHP;
    public int currentHP;
    public int currentShield;

    public event Action<int> OnHealthChanged;
    public event Action<int> OnShieldChanged;
    public event Action<FloatingNumberData> OnFloatingNumber;

    public bool TakeDamage(int amount)
    {
        if (currentShield > 0)
        {
            int remainingDamage = amount - currentShield;
            currentShield -= amount;

            if (currentShield < 0)
                currentShield = 0;

            OnShieldChanged?.Invoke(currentShield);

            if (remainingDamage > 0)
            {
                currentHP -= remainingDamage;
            }
        }
        else
        {
            currentHP -= amount;
        }

        OnHealthChanged?.Invoke(currentHP);
        OnFloatingNumber?.Invoke(new FloatingNumberData(amount, FloatingNumberType.Damage));

        return currentHP <= 0;
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
