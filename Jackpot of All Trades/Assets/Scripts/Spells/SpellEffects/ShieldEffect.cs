using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldEffect : SpellEffectBase, IScalableEffect
{
    public int shieldAmount = 8;
    public TargetingMode targetingMode = TargetingMode.SingleAlly;
    private int scaleMultiplier = 1;

    public override TargetType GetTargetType() => TargetType.TargetAlly;
    public override TargetingMode GetTargetingMode() => targetingMode;

    public void SetScaleMultiplier(int multiplier)
    {
        scaleMultiplier = multiplier;
    }

    public override void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
    {
        int finalAmount = Mathf.RoundToInt(shieldAmount * context.spellInstance.runtimeSpell.potencyMultiplier * scaleMultiplier);

        Debug.Log($"[ShieldEffect] Applying {finalAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            target.GainShield(finalAmount);
            FeedbackManager.Flash(target, FlashType.Shield);
        }
    }

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