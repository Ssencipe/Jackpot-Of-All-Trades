using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldEffect : SpellEffectBase
{
    public int shieldAmount = 8;
    public TargetingMode targetingMode = TargetingMode.SingleAlly;

    public override TargetType GetTargetType() => TargetType.TargetAlly;
    public override TargetingMode GetTargetingMode() => targetingMode;

    public override void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
    {
        Debug.Log($"[DamageEffect] Applying {shieldAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            target.GainShield(shieldAmount);
            FeedbackManager.Flash(target, FlashType.Shield);
            Debug.Log($"[ShieldEffect] Granted {shieldAmount} shield to {target}");
        }
    }

    //runtime cloning of SO
    public override ISpellEffect Clone()
    {
        return new ShieldEffect
        {
            shieldAmount = this.shieldAmount,
            targetingMode = this.targetingMode,
            effectSound = this.effectSound
        };
    }
}