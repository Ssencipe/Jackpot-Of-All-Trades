using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetingManager
{
    public static List<ITargetable> ResolveTargets(SpellSO spell, TargetingContext context)
    {
        switch (spell.targetingMode)
        {
            case TargetingMode.Self:
                return new List<ITargetable> { context.isEnemyCaster ? context.enemyCaster : context.playerCaster };

            case TargetingMode.SingleEnemy:
                return ResolveSingleEnemyTarget(context);

            case TargetingMode.AllEnemies:
                return ResolveAllEnemies(context);

            case TargetingMode.SingleAlly:
                return ResolveSingleAllyTarget(context);

            case TargetingMode.AllAllies:
                return ResolveAllAllies(context);

            default:
                Debug.LogWarning($"[TargetingManager] Unknown targeting mode for spell: {spell.spellName}");
                return new List<ITargetable>();
        }
    }

    private static List<ITargetable> ResolveSingleEnemyTarget(TargetingContext context)
    {
        if (context.isEnemyCaster)
            return new List<ITargetable> { context.playerCaster };

        var fallback = context.combat.GetLeftmostEnemy();
        var target = TargetingOverride.GetOverrideTarget() ?? fallback;
        return target != null ? new List<ITargetable> { target } : new List<ITargetable>();
    }

    private static List<ITargetable> ResolveAllEnemies(TargetingContext context)
    {
        return context.isEnemyCaster
            ? new List<ITargetable> { context.playerCaster }
            : context.combat.CurrentEnemies.Select(e => (ITargetable)e).ToList();
    }

    private static List<ITargetable> ResolveSingleAllyTarget(TargetingContext context)
    {
        if (!context.isEnemyCaster) return new List<ITargetable>();

        var lowest = context.enemyTeam
            .Where(e => !e.IsDead)
            .OrderBy(e => e.currentHP)
            .FirstOrDefault();

        return lowest != null ? new List<ITargetable> { lowest } : new List<ITargetable>();
    }

    private static List<ITargetable> ResolveAllAllies(TargetingContext context)
    {
        return context.isEnemyCaster
            ? context.enemyTeam.Select(e => (ITargetable)e).ToList()
            : new List<ITargetable> { context.playerCaster };
    }
}
