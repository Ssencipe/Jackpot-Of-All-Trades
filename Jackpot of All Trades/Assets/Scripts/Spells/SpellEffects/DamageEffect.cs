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
        int finalAmount = Mathf.RoundToInt(damageAmount * context.spellInstance.runtimeSpell.potencyMultiplier);

        Debug.Log($"[DamageEffect] Applying {finalAmount} to {resolvedTargets.Count} target(s)");

        foreach (var target in resolvedTargets)
        {
            if (target is BaseEnemy enemy)
                context.combat.DealDamage(enemy, finalAmount);
            else if (target is Unit player)
                context.combat.DealDamageToPlayer(finalAmount);

            Debug.Log($"[DamageEffect] Dealt {finalAmount} damage to {target}");
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