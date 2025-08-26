using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealEffect : SpellEffectBase, IScalableEffect
{
    public int healAmount = 6;
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
        int finalAmount = Mathf.RoundToInt(healAmount * context.spellInstance.runtimeSpell.potencyMultiplier * scaleMultiplier);

        Debug.Log($"[HealEffect] Applying {finalAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            target.Heal(finalAmount);
            FeedbackManager.Flash(target, FlashType.Heal);
        }
    }

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