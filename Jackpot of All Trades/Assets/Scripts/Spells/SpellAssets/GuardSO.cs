using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Guard")]
public class GuardSO : SpellSO, ISpellBehavior
{
    public int baseShield = 8;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster)
    {
        if (isEnemyCaster)
        {
            var enemy = combat.GetLeftmostEnemy();
            if (enemy != null)
            {
                enemy.GainShield(baseShield);
                Debug.Log($"{spellName} cast by enemy: gained {baseShield} shield.");
            }
        }
        else
        {
            combat.ShieldPlayer(baseShield);
            Debug.Log($"{spellName} cast by player: gained {baseShield} shield.");
        }
    }
}
