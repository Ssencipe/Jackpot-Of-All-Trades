using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetingManager
{

    //First select a broader targeting type (whether you want this to affect enemy targets or ally targets in the POV of the caster)
    public static List<ITargetable> ResolveTargets(TargetType targetType, TargetingMode mode, TargetingContext context)
    {
        return targetType switch
        {
            TargetType.TargetAlly => ResolveAllyTarget(mode, context),
            TargetType.TargetEnemy => ResolveEnemyTarget(mode, context),
            _ => new List<ITargetable>()
        };
    }

    // ----------- Ally Targeting --------------
    private static List<ITargetable> ResolveAllyTarget(TargetingMode mode, TargetingContext context)
    {
        if (mode == TargetingMode.Self)
        {
            return new List<ITargetable> { context.isEnemyCaster ? context.enemyCaster : context.playerCaster };
        }

        if (mode == TargetingMode.AllAllies)
        {
            return context.isEnemyCaster
                ? context.enemyTeam.Where(e => !e.IsDead).Cast<ITargetable>().ToList()
                : new List<ITargetable> { context.playerCaster };
        }

        // Default to single ally targeting strategy
        if (!context.isEnemyCaster)
            return new List<ITargetable> { context.playerCaster };

        var team = context.enemyTeam.Where(e => !e.IsDead).ToList();
        if (team.Count == 0) return new List<ITargetable>();

        var strategy = context.overrideAllyTargeting ?? context.enemyCaster.baseData.allyTargeting;

        //Targeting strategies tied to specific enemy AI
        BaseEnemy selected = strategy switch
        {
            EnemyTargeting.Self => context.enemyCaster, //target caster
            EnemyTargeting.AllyOnly =>  //only target other random allies, not self
                team.Where(e => e != context.enemyCaster).OrderBy(_ => Random.value).FirstOrDefault(),
            EnemyTargeting.WeakestAlly =>   //target lowest health ally
                team.OrderBy(e => e.currentHP).ThenBy(_ => Random.value).FirstOrDefault(),
            EnemyTargeting.StrongestAlly => //target highest impact score ally
                team.OrderByDescending(e => e.baseData.impactScore).ThenBy(_ => Random.value).FirstOrDefault(),
            EnemyTargeting.Random =>    //target random ally including self
                team.OrderBy(_ => Random.value).FirstOrDefault(),
            _ => context.enemyCaster
        };

        return selected != null ? new List<ITargetable> { selected } : new List<ITargetable>();
    }

    // ----------- Enemy Targeting --------------
    private static List<ITargetable> ResolveEnemyTarget(TargetingMode mode, TargetingContext context)
    {
        switch (mode)
        {
            case TargetingMode.Self:
                return new List<ITargetable> { context.isEnemyCaster ? context.enemyCaster : context.playerCaster };

            case TargetingMode.SingleEnemy:
                if (context.isEnemyCaster)
                    return new List<ITargetable> { context.playerCaster };

                var fallback = context.combat.GetEnemy();
                var target = TargetingOverride.GetOverrideTarget() ?? fallback;
                return target != null ? new List<ITargetable> { target } : new List<ITargetable>();

            case TargetingMode.AllEnemies:
                return context.isEnemyCaster
                    ? new List<ITargetable> { context.playerCaster }
                    : context.combat.CurrentEnemies.Where(e => !e.IsDead).Cast<ITargetable>().ToList();

            default:
                Debug.LogWarning($"[TargetingManager] Unknown enemy targeting mode: {mode}");
                return new List<ITargetable>();
        }
    }
}