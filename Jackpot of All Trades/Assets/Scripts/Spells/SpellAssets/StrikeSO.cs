using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strike")]
public class StrikeSO : SpellSO, ISpellBehavior
{
    public int baseDamage = 5;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster)
    {
        if (isEnemyCaster)
        {
            combat.DealDamageToPlayer(baseDamage);
        }
        else
        {
            var enemy = combat.GetLeftmostEnemy();
            if (enemy != null)
            {
                combat.DealDamage(enemy, baseDamage);
            }
        }
        Debug.Log($"{spellName} cast! Dealt {baseDamage} damage.");
    }
}
