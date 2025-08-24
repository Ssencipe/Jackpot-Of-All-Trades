[System.Serializable]
public abstract class SpellConditionBase : ISpellCondition
{
    public abstract bool Evaluate(SpellCastContext context);
    public abstract ConditionResultType GetResultType();
    public virtual ISpellEffect GetLinkedEffect() => null;
    public virtual float GetPotencyMultiplier() => 1f;
}