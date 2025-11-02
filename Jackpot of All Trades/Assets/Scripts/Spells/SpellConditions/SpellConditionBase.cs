[System.Serializable]

// The base class of conditions that defines basic data

public abstract class SpellConditionBase : ISpellCondition
{
    public abstract bool Evaluate(SpellCastContext context);
    public abstract ConditionResultType GetResultType();
    public virtual ISpellEffect GetLinkedEffect() => null;
    public virtual float GetPotencyMultiplier() => 1f;

    // For conditional neighbor logic
    public virtual NeighborModification GetNeighborModification() => null;
}

// The various effects that can occur if a condition is met
public enum ConditionResultType
{
    TriggerEffect,      // triggers a spell effect
    ModifyPotency,      // modify potency of THIS spell
    SkipSpell,          // skips the casting of THIS spell
    ModifyNeighbor      // modifies neighboring spells
}

// The scope of which spells are affected by a ModifyNeighbor result
public enum NeighborScope
{
    Adjacent,           // top bottm left right
    Diagonal,           // corners
    AllSurrounding,     // adjacent + diagonal
    Horizontal,         // left right
    Vertical,           // top bottom
    Exact,              // one of 8 surround spells
    AllSpells,          // entire grid of spells
    NonNeighbors        // all except 8 surrounding
}

// The various effects that can be chosen from for a ModifyNeighbor result
public enum NeighborModificationType
{
    ChangePotency,
    TakePotency,
    ChangeCharge,
    TakeCharge,
    ChangeTally,
    TakeTally
}

