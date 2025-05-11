using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Mend")]
public class MendSO : SpellSO, ISpellBehavior
{
    public int baseHeal = 6; //quantity for healing

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

        //the actual action
        foreach (var target in targets)
        {
            target.Heal(baseHeal);
            Debug.Log($"{spellName} healed {target} for {baseHeal} HP.");
        }
    }
    // Helper to find which enemy owns the spell instance
    private BaseEnemy FindCasterEnemy(BaseSpell spell, CombatManager combat)
    {
        return combat.CurrentEnemies.FirstOrDefault(e => e.activeSpells.Contains(spell));
    }
    // force specific targeting mode based on the type of spell it is
    private void OnValidate() => targetingMode = TargetingMode.SingleAlly;
}