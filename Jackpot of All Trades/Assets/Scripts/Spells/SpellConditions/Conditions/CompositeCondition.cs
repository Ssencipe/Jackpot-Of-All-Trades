using System.Collections.Generic;
using UnityEngine;

public enum LogicType { AND, OR, NOT }

[System.Serializable]
public class CompositeCondition : SpellConditionBase
{
    public LogicType logicType = LogicType.AND;

    [SerializeReference, SubclassSelector]
    public ISpellCondition conditionA;

    [SerializeReference, SubclassSelector]
    public ISpellCondition conditionB;

    [SerializeReference, SubclassSelector]
    public ISpellEffect linkedEffect;

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