public interface ISpellCondition
{
    bool Evaluate(SpellCastContext context);
    ConditionResultType GetResultType(); // e.g., TriggerEffect, ModifyPotency, SkipSpell, etc.
    ISpellEffect GetLinkedEffect(); // if result is to trigger a specific effect
    float GetPotencyMultiplier(); // optional for ModifyPotency
}
