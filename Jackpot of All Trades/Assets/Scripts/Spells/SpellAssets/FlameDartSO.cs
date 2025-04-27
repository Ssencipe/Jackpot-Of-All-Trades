using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Flame Dart")]
public class FlameDartSO : SpellSO, ISpellBehavior
{
    public int baseDamage = 5;
    public int bonusIfAtBottom = 3;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid)
    {
        int damage = baseDamage;

        if (instance.slotIndex == 2) // bottom row
        {
            damage += bonusIfAtBottom;
        }
    /*
        var target = combat.GetLeftmostEnemy();
        combat.DealDamage(target, damage);
    */
    }
}