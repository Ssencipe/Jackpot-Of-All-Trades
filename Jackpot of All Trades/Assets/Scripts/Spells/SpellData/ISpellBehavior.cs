using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpellBehavior
{
    
    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null);
}


