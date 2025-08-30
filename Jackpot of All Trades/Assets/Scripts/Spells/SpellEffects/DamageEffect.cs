using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageEffect : SpellEffectBase, IScalableEffect
{
    public int damageAmount = 5;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;
    private int scaleMultiplier = 1;

    public override TargetType GetTargetType() => TargetType.TargetEnemy;
    public override TargetingMode GetTargetingMode() => targetingMode;

    public void SetScaleMultiplier(int multiplier)
    {
        scaleMultiplier = multiplier;
    }

    public override void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
    {
        int finalAmount = Mathf.RoundToInt(damageAmount * context.spellInstance.runtimeSpell.potencyMultiplier * scaleMultiplier);

        Debug.Log($"[DamageEffect] Applying {finalAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            if (target is BaseEnemy enemy)
                context.combat.DealDamage(enemy, finalAmount);
            else if (target is Unit player)
                context.combat.DealDamageToPlayer(finalAmount);
        }
    }

    public override ISpellEffect Clone()
    {
        return new DamageEffect
        {
            damageAmount = this.damageAmount,
            targetingMode = this.targetingMode,
            effectSound = this.effectSound
        };
    }
}