using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectController : MonoBehaviour
{
    private List<IStatusEffect> activeEffects = new();

    public event Action OnEffectsChanged;

    public IReadOnlyList<IStatusEffect> ActiveEffects => activeEffects;

    public void AddEffect(IStatusEffect effect, ITargetable target)
    {
        var existing = activeEffects.FirstOrDefault(e => e.ID == effect.ID);

        if (existing != null)
        {
            Debug.Log($"Refreshing status effect: {effect.ID}");
            existing.Refresh(effect);  // Stack or refresh logic
            OnEffectsChanged?.Invoke();
            return;
        }

        activeEffects.Add(effect);
        effect.OnApply(target);
        OnEffectsChanged?.Invoke();
    }

    public void TickTurnStart(ITargetable target)
    {
        foreach (var effect in activeEffects.ToList())
            effect.OnTurnStart(target);
        OnEffectsChanged?.Invoke();
    }

    public void TickTurnEnd(ITargetable target)
    {
        foreach (var effect in activeEffects.ToList())
            effect.OnTurnEnd(target);

        activeEffects.RemoveAll(e => e.Duration <= 0);
        OnEffectsChanged?.Invoke();
    }

}