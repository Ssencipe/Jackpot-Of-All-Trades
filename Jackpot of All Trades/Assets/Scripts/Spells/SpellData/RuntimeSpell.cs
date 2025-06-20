using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeSpell
{
    public SpellSO baseData;

    public string spellName;
    public string description;
    public Sprite icon;
    public bool hasCharges;
    public int charge;
    public ColorType colorType;
    public List<SpellTag> tags;
    public List<ISpellEffect> effects;

    public bool isDisabled; // runtime-only toggle

    public RuntimeSpell(SpellSO source)
    {
        baseData = source;

        spellName = source.spellName;
        description = source.description;
        icon = source.icon;
        hasCharges = source.hasCharges;
        charge = source.charge;
        colorType = source.colorType;
        tags = new List<SpellTag>(source.tags);
        effects = source.effects.Select(e => e.Clone()).ToList(); // ensure Clone() exists
        isDisabled = false;
    }

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        var context = new SpellCastContext
        {
            spellInstance = instance,
            combat = combat,
            grid = grid,
            isEnemyCaster = isEnemyCaster,
            enemyCaster = enemyCaster,
            playerCaster = combat.playerUnit,
            enemyTeam = combat.CurrentEnemies.ToList()
        };

        foreach (var effect in effects)
        {
            var targets = TargetingManager.ResolveTargets(
                effect.GetTargetType(),
                effect.GetTargetingMode(),
                new TargetingContext
                {
                    isEnemyCaster = isEnemyCaster,
                    combat = combat,
                    grid = grid,
                    playerCaster = combat.playerUnit,
                    enemyCaster = enemyCaster,
                    enemyTeam = combat.CurrentEnemies.ToList()
                });

            Debug.Log($"[RuntimeSpell] Effect {effect.GetType().Name} resolved {targets.Count} targets");
            effect.Apply(context, targets);
        }

        if (hasCharges)
            charge--;
    }

    public void UseCharge()
    {
        if (hasCharges && charge > 0)
            charge--;
    }

    public bool IsDepleted() => hasCharges && charge <= 0;
}