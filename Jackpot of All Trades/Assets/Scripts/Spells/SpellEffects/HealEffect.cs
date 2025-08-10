using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealEffect : SpellEffectBase
{
    public int healAmount = 6;
    public TargetingMode targetingMode = TargetingMode.SingleAlly;

    public override TargetType GetTargetType() => TargetType.TargetAlly;
    public override TargetingMode GetTargetingMode() => targetingMode;

    public override void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
    {
        Debug.Log($"[DamageEffect] Applying {healAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            target.Heal(healAmount);
            FeedbackManager.Flash(target, FlashType.Heal);
            Debug.Log($"[HealEffect] Healed {healAmount} HP to {target}");
        }
    }

    //runtime cloning of SO
    public override ISpellEffect Clone()
    {
        return new HealEffect
        {
            healAmount = this.healAmount,
            targetingMode = this.targetingMode,
            effectSound = this.effectSound
        };
    }
}