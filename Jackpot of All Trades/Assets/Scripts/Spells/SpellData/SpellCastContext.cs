using System.Collections.Generic;

// Basic references you want for casting a spell

public class SpellCastContext
{
    public BaseSpell spellInstance;
    public CombatManager combat;
    public GridManager grid;
    public bool isEnemyCaster;
    public BaseEnemy enemyCaster;
    public BasePlayer playerCaster;
    public IEnumerable<BaseEnemy> enemyTeam;
}