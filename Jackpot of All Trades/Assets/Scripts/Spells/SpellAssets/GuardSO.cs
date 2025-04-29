using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Guard")]
public class GuardSO : SpellSO, ISpellBehavior
{
    public int baseShield = 8;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid)
    {
        /*
        var caster = combat.GetCurrentCaster(instance);
        if (caster != null)
        {
            caster.GainShield(baseShield);
        }

        Debug.Log($"{spellName} cast: gaining {baseShield} shield.");
        */
    }
}
