using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "Spells/Guard")]
public class GuardSO : SpellSO, ISpellBehavior
{
    public int baseShield = 8;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        var context = new TargetingContext
        {
            isEnemyCaster = isEnemyCaster,
            combat = combat,
            grid = grid,
            playerCaster = combat.playerUnit,
            enemyCaster = enemyCaster,
            enemyTeam = combat.CurrentEnemies.ToList()
        };

        List<ITargetable> targets = TargetingManager.ResolveTargets(this, context);

        foreach (var target in targets)
        {
            target.GainShield(baseShield);
            Debug.Log($"{spellName} granted {baseShield} shield to {target}.");
        }
    }

    private BaseEnemy FindCasterEnemy(BaseSpell spell, CombatManager combat)
    {
        return combat.CurrentEnemies.FirstOrDefault(e => e.activeSpells.Contains(spell));
    }

    // force specific targeting mode based on the type of spell it is
    private void OnValidate() => targetingMode = TargetingMode.SingleAlly;
}