using System.Collections.Generic;

public interface ISpellEffect
{
    TargetType GetTargetType();          // TargetAlly or TargetEnemy
    TargetingMode GetTargetingMode();    // Self, SingleEnemy, AllEnemies, AllAllies, etc.
    void Apply(SpellCastContext context, List<ITargetable> targets);
}