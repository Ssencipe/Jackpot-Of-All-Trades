using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Spells/Strike")]
public class StrikeSO : SpellSO, ISpellBehavior
{
    public int baseDamage = 5;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
{
    if (isEnemyCaster)
    {
        combat.DealDamageToPlayer(baseDamage);
        Debug.Log($"{spellName} cast by enemy! Dealt {baseDamage} damage to player.");
    }
    else
    {
        Debug.Log($"[StrikeSO] Player cast of {spellName} starting");

        var context = new TargetingContext
        {
            isEnemyCaster = false,
            playerCaster = combat.playerUnit,
            enemyCaster = null,
            combat = combat,
            grid = grid,
            enemyTeam = combat.CurrentEnemies.ToList()
        };

        var targets = TargetingManager.ResolveTargets(instance.spellData, context);
        Debug.Log($"[StrikeSO] Targets found: {targets.Count}");

        foreach (var target in targets)
        {
            Debug.Log($"[StrikeSO] Target type: {target.GetType().Name}");

            if (target is BaseEnemy enemy)
            {
                Debug.Log($"[StrikeSO] Dealing {baseDamage} damage to {enemy.baseData?.enemyName ?? "UNKNOWN"}");
                combat.DealDamage(enemy, baseDamage);
                Debug.Log($"{spellName} cast! Dealt {baseDamage} damage to {enemy.baseData.enemyName}.");
            }
        }
    }
}

}