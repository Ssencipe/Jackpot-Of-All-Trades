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
        var order = GetGridProcessingOrder();

        foreach (var pos in order)
        {
            int x = pos.x;
            int y = pos.y;

            var spell = grid[x, y];
            if (spell == null) continue;

            var spellSO = spell.spellData;
            if (spellSO.conditions == null || spellSO.conditions.Count == 0)
                continue;

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

            foreach (var condition in spellSO.conditions)
            {
                if (!condition.Evaluate(context)) continue;

                bool triggerPlayed = false;

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
                            triggerPlayed = true;
                        }
                        break;

                    case ConditionResultType.ModifyPotency:
                        var mpSpell = context.spellInstance.runtimeSpell;
                        mpSpell.wasPotencyModified = true;
                        mpSpell.ApplyPotencyMultiplier(condition.GetPotencyMultiplier());
                        reel?.PlayEffectAtSlot(y);
                        reel?.reelVisual?.RefreshAllVisuals();
                        triggerPlayed = true;
                        break;

                    case ConditionResultType.SkipSpell:
                        var skSpell = context.spellInstance.runtimeSpell;
                        skSpell.wasMarkedToSkip = true;
                        reel?.PlayEffectAtSlot(y);
                        reel?.reelVisual?.RefreshAllVisuals();
                        triggerPlayed = true;
                        break;
                }

                if (triggerPlayed)
                    yield return new WaitForSeconds(reelDelay);
            }
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