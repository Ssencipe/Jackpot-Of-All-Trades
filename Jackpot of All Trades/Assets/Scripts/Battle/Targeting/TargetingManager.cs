using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TargetingManager
{
    //Spell based targeting logics, relevant for cases where multiple targets can be affected
    public static List<ITargetable> ResolveTargets(SpellSO spell, TargetingContext context)
    {
        switch (spell.targetingMode)
        {
            //affects one enemy target
            case TargetingMode.SingleEnemy:
                return ResolveSingleEnemyTarget(context);

            //affects all enemy targets
            case TargetingMode.AllEnemies:
                return ResolveAllEnemies(context);

            //affects an ally (for enemies, has further cases in method)
            case TargetingMode.SingleAlly:
                return ResolveSingleAllyTarget(context);

            //affects all allies (for enemies)
            case TargetingMode.AllAllies:
                return ResolveAllAllies(context);

            //affects caster, this is actually redundant, mostly for edge case spells
            case TargetingMode.Self:
                return new List<ITargetable> { context.isEnemyCaster ? context.enemyCaster : context.playerCaster };

            default:
                Debug.LogWarning($"[TargetingManager] Unknown targeting mode for spell: {spell.spellName}");
                return new List<ITargetable>();
        }
    }

    private static List<ITargetable> ResolveSingleEnemyTarget(TargetingContext context)
    {
        if (context.isEnemyCaster)
            return new List<ITargetable> { context.playerCaster };

        var fallback = context.combat.GetEnemy();
        var target = TargetingOverride.GetOverrideTarget() ?? fallback;
        return target != null ? new List<ITargetable> { target } : new List<ITargetable>();
    }

    private static List<ITargetable> ResolveAllEnemies(TargetingContext context)
    {
        return context.isEnemyCaster
            ? new List<ITargetable> { context.playerCaster }
            : context.combat.CurrentEnemies.Select(e => (ITargetable)e).ToList();
    }

    private static List<ITargetable> ResolveAllAllies(TargetingContext context)
    {
        return context.isEnemyCaster
            ? context.enemyTeam.Select(e => (ITargetable)e).ToList()
            : new List<ITargetable> { context.playerCaster };
    }

    //determines which ally is being targeted from EnemyTargeting script, these are defined by the enemy, not the spell
    private static List<ITargetable> ResolveSingleAllyTarget(TargetingContext context)
    {
        if (!context.isEnemyCaster) return new List<ITargetable>();

        var team = context.enemyTeam.Where(e => !e.IsDead).ToList();
        if (team.Count == 0) return new List<ITargetable>();

        var strategy = context.overrideAllyTargeting ?? context.enemyCaster.baseData.allyTargeting;

        BaseEnemy selected = strategy switch
        {
            //targets caster
            EnemyTargeting.Self => context.enemyCaster,

            //will only target non-caster allies
            EnemyTargeting.AllyOnly =>
                team.Where(e => e != context.enemyCaster).OrderBy(_ => Random.value).FirstOrDefault(),

            //targets ally with lowest HP (good for healing/shielding)
            EnemyTargeting.WeakestAlly =>
                team.OrderBy(e => e.currentHP).ThenBy(_ => Random.value).FirstOrDefault(),

            //targets ally with highest impact score (good for buffs)
            EnemyTargeting.StrongestAlly =>
                team.OrderByDescending(e => e.baseData.impactScore).ThenBy(_ => Random.value).FirstOrDefault(),

            //targets random ally including caster
            EnemyTargeting.Random =>
                team.OrderBy(_ => Random.value).FirstOrDefault(),

            _ => context.enemyCaster
        };

        return selected != null ? new List<ITargetable> { selected } : new List<ITargetable>();
    }
}
