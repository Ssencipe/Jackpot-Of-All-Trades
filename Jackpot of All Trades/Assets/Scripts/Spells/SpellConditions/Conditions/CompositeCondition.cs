using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LogicType { AND, OR, NOT }

[System.Serializable]
public class CompositeCondition : SpellConditionBase
{
    public LogicType logicType = LogicType.AND;

    [SerializeReference, SubclassSelector]
    public List<ISpellCondition> conditions = new();

    public ConditionResultType resultType = ConditionResultType.TriggerEffect;

    [SerializeReference, SubclassSelector]
    public ISpellEffect linkedEffect;

    public float potencyMultiplier = 1f;

    public NeighborModification neighborModification;
    public override NeighborModification GetNeighborModification() => neighborModification;

    public override bool Evaluate(SpellCastContext context)
    {
        if (conditions == null || conditions.Count == 0)
            return false;

        bool passed = logicType switch
        {
            LogicType.AND => conditions.All(c => c?.Evaluate(context) ?? false),
            LogicType.OR => conditions.Any(c => c?.Evaluate(context) ?? false),
            LogicType.NOT => !(conditions[0]?.Evaluate(context) ?? false),
            _ => false
        };

        if (!passed)
            return false;

        // Apply result-based behavior if condition passed
        switch (resultType)
        {
            case ConditionResultType.ModifyPotency:
                context.spellInstance.runtimeSpell.wasPotencyModified = true;
                context.spellInstance.runtimeSpell.ApplyPotencyMultiplier(potencyMultiplier);
                break;

            case ConditionResultType.SkipSpell:
                context.spellInstance.runtimeSpell.wasMarkedToSkip = true;
                break;

            case ConditionResultType.ModifyNeighbor:
                neighborModification?.Apply(context, context.spellInstance);
                break;

            case ConditionResultType.TriggerEffect:
                linkedEffect?.Apply(context, TargetingManager.ResolveTargets(
                    linkedEffect.GetTargetType(),
                    linkedEffect.GetTargetingMode(),
                    new TargetingContext
                    {
                        isEnemyCaster = false,
                        combat = context.combat,
                        grid = context.grid,
                        playerCaster = context.playerCaster,
                        enemyCaster = null,
                        enemyTeam = context.enemyTeam.ToList()
                    }));
                break;
        }

        return true;
    }

    public override ConditionResultType GetResultType() => resultType;
    public override ISpellEffect GetLinkedEffect() => linkedEffect;
    public override float GetPotencyMultiplier() => potencyMultiplier;
}
