using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Context for targets being affected by spells. May want to merge into another script.

public class TargetingContext
{
    public bool isEnemyCaster;
    public BaseEnemy enemyCaster;
    public Unit playerCaster;
    public CombatManager combat;
    public GridManager grid;

    public int reelIndex; // optional, for positional logic
    public List<BaseSpell> fullGrid;
    public List<BaseEnemy> enemyTeam;

    public EnemyTargeting? overrideAllyTargeting;
}
