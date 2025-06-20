using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageEffect : ISpellEffect
{
    public int damageAmount = 5;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;

    public TargetType GetTargetType() => TargetType.TargetEnemy;
    public TargetingMode GetTargetingMode() => targetingMode;

    public void Apply(SpellCastContext context, List<ITargetable> resolvedTargets)
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
    public ISpellEffect Clone()
    {
        return new DamageEffect
        {
            damageAmount = this.damageAmount,
            targetingMode = this.targetingMode
        };
    }
}