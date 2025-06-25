using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageEffect : SpellEffectBase
{
    public int damageAmount = 5;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;

    public override TargetType GetTargetType() => TargetType.TargetEnemy;
    public override TargetingMode GetTargetingMode() => targetingMode;

    public override void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
    {
        Debug.Log($"[DamageEffect] Applying {damageAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            if (target is BaseEnemy enemy)
                context.combat.DealDamage(enemy, damageAmount);
            else if (target is Unit player)
                context.combat.DealDamageToPlayer(damageAmount);

            Debug.Log($"[DamageEffect] Dealt {damageAmount} damage to {target}");
        }
    }

    //runtime cloning of SO
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