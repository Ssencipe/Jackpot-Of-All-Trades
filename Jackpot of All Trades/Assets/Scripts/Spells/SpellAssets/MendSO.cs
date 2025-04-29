using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Mend")]
public class MendSO : SpellSO, ISpellBehavior
{
    public int baseHeal = 6;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid)
    {
        /*
        var caster = combat.GetCurrentCaster(instance);
        if (caster != null)
        {
            caster.Heal(baseHeal);
        
        }

        Debug.Log($"{spellName} cast: healing {baseHeal} health.");
        */
    }
}
