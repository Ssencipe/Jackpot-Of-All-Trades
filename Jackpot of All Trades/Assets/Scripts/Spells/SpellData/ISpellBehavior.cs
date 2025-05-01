using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellBehavior
{
    void Cast(BaseSpell spellInstance, CombatManager combat, GridManager grid, bool isEnemyCaster);
}


