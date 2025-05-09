using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
