using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public enum LogicType { AND, OR, NOT }

[System.Serializable]
public class CompositeCondition : SpellConditionBase
{
    [Header("Logic Control")]
    public LogicType logicType = LogicType.AND;

    [Label("First Condition")]
    [SerializeReference, SubclassSelector]
    public ISpellCondition conditionA;

    [Label("Second Condition")]
    [EnableIf(nameof(NeedsTwoConditions))]
    [SerializeReference, SubclassSelector]
    public ISpellCondition conditionB;

    [Header("Result")]
    [Label("Triggered Spell Effect")]
    [SerializeReference, SubclassSelector]
    public ISpellEffect linkedEffect;

    private bool NeedsTwoConditions() => logicType == LogicType.AND || logicType == LogicType.OR;

    public override bool Evaluate(SpellCastContext context)
    {
        bool a = conditionA?.Evaluate(context) ?? false;
        bool b = conditionB?.Evaluate(context) ?? false;

        return logicType switch
        {
            LogicType.AND => a && b,
            LogicType.OR => a || b,
            LogicType.NOT => !a,
            _ => false
        };
    }

    public override ConditionResultType GetResultType() => ConditionResultType.TriggerEffect;
    public override ISpellEffect GetLinkedEffect() => linkedEffect;
}