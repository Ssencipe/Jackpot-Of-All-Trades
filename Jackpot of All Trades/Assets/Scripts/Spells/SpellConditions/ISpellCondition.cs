public interface ISpellCondition
{
    bool Evaluate(SpellCastContext context);
    ConditionResultType GetResultType(); // e.g., TriggerEffect, ModifyPotency, SkipSpell, etc.
    ISpellEffect GetLinkedEffect(); // if result is to trigger a specific effect
    float GetPotencyMultiplier(); // for changing spell potency
    NeighborModification GetNeighborModification(); // for modifying neighboring spells
}
