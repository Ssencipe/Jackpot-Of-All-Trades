using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strike")]
public class StrikeSO : SpellSO, ISpellBehavior
{
    public int baseDamage = 5;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        if (isEnemyCaster)
        {
            combat.DealDamageToPlayer(baseDamage);
        }
        else
        {
            var enemy = combat.GetEnemy();
            if (enemy != null)
            {
                combat.DealDamage(enemy, baseDamage);
            }
        }
        Debug.Log($"{spellName} cast! Dealt {baseDamage} damage.");
    }
}
