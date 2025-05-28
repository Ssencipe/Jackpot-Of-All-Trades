using System.Collections.Generic;

public class SpellCastContext
{
    public BaseSpell spellInstance;
    public CombatManager combat;
    public GridManager grid;
    public bool isEnemyCaster;
    public BaseEnemy enemyCaster;
    public Unit playerCaster;
    public IEnumerable<BaseEnemy> enemyTeam;
}