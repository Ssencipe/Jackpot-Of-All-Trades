using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealEffect : ISpellEffect
{
    public int healAmount = 6;
    public TargetingMode targetingMode = TargetingMode.SingleAlly;

    public TargetType GetTargetType() => TargetType.TargetAlly;
    public TargetingMode GetTargetingMode() => targetingMode;

    public void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
    {
        Debug.Log($"[DamageEffect] Applying {healAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            target.Heal(healAmount);
            FeedbackManager.Flash(target, FlashType.Heal);
            Debug.Log($"[HealEffect] Healed {healAmount} HP to {target}");
        }
    }
}