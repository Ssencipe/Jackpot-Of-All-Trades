using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Strike")]
public class StrikeSO : SpellSO, ISpellBehavior
{
    public int baseDamage = 5;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid)
    {
        var target = combat.GetLeftmostEnemy();
        if (target != null)
        {
            /* target.TakeDamage(baseDamage); */
        }

        Debug.Log($"{spellName} cast: dealing {baseDamage} damage.");
    }
}
