using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeSpell
{
    public SpellSO baseData;

    public string spellName;
    public string description;
    public Sprite icon;
    public bool hasCharges;         // If spell has limited uses
    public int charge;              // How many uses it has
    public bool hasTallies;         // If spell has intrinsic economy
    public int tally;               // Initial economy value
    private int previousTally = -1; // Used for change detection
    public ColorType colorType;
    public List<SpellTag> tags;
    public List<ISpellEffect> effects;
    public string castSound;
    public AudioCategory castSoundCategory;

    public bool wasMarkedToSkip;
    public bool wasPotencyModified;
    public bool wasModifiedByCondition => wasMarkedToSkip || wasPotencyModified;


    public bool isDisabled; // runtime-only toggle

    public float potencyMultiplier = 1f; //runtime changes to spell value

    public RuntimeSpell(SpellSO source)
    {
        baseData = source;

        spellName = source.spellName;
        description = source.description;
        icon = source.icon;
        hasCharges = source.hasCharges;
        charge = source.charge;
        hasTallies = source.hasTallies;
        tally = source.tally;
        colorType = source.colorType;
        tags = new List<SpellTag>(source.tags);
        effects = source.effects.Select(e => e.Clone()).ToList(); // ensure Clone() exists
        isDisabled = false;
        castSound = baseData.castSound;
        castSoundCategory = baseData.castSoundCategory;
    }

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        // Apply stored skip flag
        if (wasMarkedToSkip)
        {
            Debug.Log($"[RuntimeSpell] Skipping {spellName} due to pre-cast flag.");
            ClearModificationFlags(); // Reset after use
            return;
        }

        // Use modified potency if set (already applied in ProcessGrid)
        if (wasPotencyModified)
        {
            Debug.Log($"[RuntimeSpell] Casting {spellName} with modified potency: {potencyMultiplier}");
        }

        // Skip spell effects if disabled
        if (isDisabled)
        {
            Debug.Log($"[RuntimeSpell] Skipping cast for {spellName} due to isDisabled flag.");
            return;
        }

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

        var targetingContext = new TargetingContext
        {
            isEnemyCaster = isEnemyCaster,
            combat = combat,
            grid = grid,
            playerCaster = combat.playerUnit,
            enemyCaster = enemyCaster,
            enemyTeam = combat.CurrentEnemies.ToList()
        };

        // Conditions — these may modify potency
        foreach (var condition in baseData.conditions)
        {
            if (condition.Evaluate(context))
            {
                switch (condition.GetResultType())
                {
                    case ConditionResultType.ModifyPotency:
                        ApplyPotencyMultiplier(condition.GetPotencyMultiplier());
                        break;

                    case ConditionResultType.SkipSpell:
                        Debug.Log($"[RuntimeSpell] Spell skipped due to condition.");
                        return;

                    case ConditionResultType.TriggerEffect:
                        var effect = condition.GetLinkedEffect();
                        if (effect != null)
                        {
                            var targets = TargetingManager.ResolveTargets(effect.GetTargetType(), effect.GetTargetingMode(), targetingContext);
                            effect.Apply(context, targets);
                        }
                        break;
                }
            }
        }

        // Main effects
        foreach (var effect in effects)
        {
            var targets = TargetingManager.ResolveTargets(effect.GetTargetType(), effect.GetTargetingMode(), targetingContext);
            effect.Apply(context, targets);
        }

        if (hasCharges)
        {
            charge--;
        }

        // Reset potency multiplier for a clean cast
        ClearModificationFlags();
        GridManager.Instance?.linkedReels[instance.reelIndex]?.reelVisual?.RefreshAllVisuals();
        potencyMultiplier = 1f;
    }

    public void UseCharge()
    {
        if (hasCharges && charge > 0)
            charge--;
    }

    public bool HasTallyChanged()
    {
        return tally != previousTally;
    }

    public void SetTally(int value)
    {
        previousTally = tally;
        tally = value;
    }

    public void ApplyPotencyMultiplier(float value)
    {
        potencyMultiplier *= value;
    }

    public void ClearModificationFlags()
    {
        wasMarkedToSkip = false;
        wasPotencyModified = false;
    }

    public bool IsDepleted() => hasCharges && charge <= 0;
}