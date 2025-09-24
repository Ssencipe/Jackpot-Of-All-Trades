using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridProcessor : MonoBehaviour
{
    public CombatManager combatManager;
    public GridManager gridManager;

    public void Initialize(CombatManager combat, GridManager grid)
    {
        combatManager = combat;
        gridManager = grid;
    }

    public IEnumerator ProcessGridForConditionalEffects(BaseSpell[,] grid)
    {
        // Define tag phases (can be adjusted or expanded later)
        SpellTag[][] tagPhases =
        {
            new[] { SpellTag.Mover },                                // Phase 1
            new[] { SpellTag.Tally, SpellTag.Transformer },          // Phase 2
            new[] { SpellTag.Adjacency, SpellTag.Positional }        // Phase 3
        };

        // Track which spells have already been processed
        HashSet<BaseSpell> processedSpells = new();

        foreach (var tagGroup in tagPhases)
        {
            yield return ProcessTaggedSpells(grid, tagGroup, processedSpells);
        }

        // Final pass: untagged or uncategorized spells
        yield return ProcessTaggedSpells(grid, null, processedSpells);
    }

    private IEnumerator ProcessTaggedSpells(BaseSpell[,] grid, SpellTag[] validTags, HashSet<BaseSpell> alreadyProcessed)
    {
        var order = GetGridProcessingOrder();

        foreach (var pos in order)
        {
            int x = pos.x;
            int y = pos.y;

            var spell = grid[x, y];
            if (spell == null || alreadyProcessed.Contains(spell)) continue;

            var tags = spell.runtimeSpell?.tags ?? spell.spellData?.tags;

            // If spell has no tags or doesn't match, skip
            if (validTags != null && (tags == null || !tags.Any(tag => validTags.Contains(tag))))
                continue;

            var spellSO = spell.spellData;
            if (spellSO.conditions == null || spellSO.conditions.Count == 0)
            {
                alreadyProcessed.Add(spell);
                continue;
            }

            var reel = gridManager.linkedReels[x];
            float reelDelay = reel != null ? reel.orbitDuration : 1f;

            var context = new SpellCastContext
            {
                spellInstance = spell,
                combat = combatManager,
                grid = gridManager,
                isEnemyCaster = false,
                playerCaster = combatManager.playerUnit,
                enemyTeam = combatManager.CurrentEnemies.ToList()
            };

            bool triggeredSomething = false;

            foreach (var condition in spellSO.conditions)
            {
                if (!condition.Evaluate(context)) continue;

                switch (condition.GetResultType())
                {
                    case ConditionResultType.TriggerEffect:
                        var effect = condition.GetLinkedEffect();
                        if (effect != null)
                        {
                            var targets = TargetingManager.ResolveTargets(
                                effect.GetTargetType(),
                                effect.GetTargetingMode(),
                                new TargetingContext
                                {
                                    isEnemyCaster = false,
                                    combat = combatManager,
                                    grid = gridManager,
                                    playerCaster = combatManager.playerUnit,
                                    enemyCaster = null,
                                    enemyTeam = combatManager.CurrentEnemies.ToList()
                                });

                            effect.Apply(context, targets);
                            reel?.PlayEffectAtSlot(y);
                            triggeredSomething = true;
                        }
                        break;

                    case ConditionResultType.ModifyPotency:
                        context.spellInstance.runtimeSpell.wasPotencyModified = true;
                        context.spellInstance.runtimeSpell.ApplyPotencyMultiplier(condition.GetPotencyMultiplier());
                        reel?.PlayEffectAtSlot(y);
                        reel?.reelVisual?.RefreshAllVisuals();
                        triggeredSomething = true;
                        break;

                    case ConditionResultType.SkipSpell:
                        context.spellInstance.runtimeSpell.wasMarkedToSkip = true;
                        reel?.PlayEffectAtSlot(y);
                        reel?.reelVisual?.RefreshAllVisuals();
                        triggeredSomething = true;
                        break;
                }

                if (triggeredSomething)
                    yield return new WaitForSeconds(reelDelay);
            }

            alreadyProcessed.Add(spell);
        }
    }

    private List<Vector2Int> GetGridProcessingOrder()
    {
        List<Vector2Int> order = new();
        for (int x = 0; x < GridManager.Reels; x++)
        {
            for (int y = 0; y < GridManager.SlotsPerReel; y++)
            {
                order.Add(new Vector2Int(x, y));
            }
        }
        return order;
    }
}