using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Flame Dart")]
public class FlameDartSO : SpellSO, ISpellBehavior
{
    public int baseDamage = 5;
    public int bonusIfAtBottom = 3;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster)
    {
        int damage = baseDamage;
        if (instance.slotIndex == 2)
        {
            damage += bonusIfAtBottom;
        }
        // For simplicity, this spell always damages the opponent:
        if (isEnemyCaster)
        {
            // Enemy casts: target the player
            combat.DealDamageToPlayer(damage);
        }
        else
        {
            // Player casts: target the enemy (leftmost)
            var enemy = combat.GetLeftmostEnemy();
            if (enemy != null)
            {
                combat.DealDamage(enemy, damage);
            }
        }
        Debug.Log($"{spellName} cast! Dealt {damage} damage.");
    }
}
