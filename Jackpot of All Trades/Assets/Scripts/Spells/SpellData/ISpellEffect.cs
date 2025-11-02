using System.Collections.Generic;

// Defines core references needed for a spell effect (so it can affect stuff)

public interface ISpellEffect
{
    TargetType GetTargetType();          // TargetAlly or TargetEnemy
    TargetingMode GetTargetingMode();    // Self, SingleEnemy, AllEnemies, AllAllies, etc.
    void Apply(SpellCastContext context, List<ITargetable> targets);
    ISpellEffect Clone();
}