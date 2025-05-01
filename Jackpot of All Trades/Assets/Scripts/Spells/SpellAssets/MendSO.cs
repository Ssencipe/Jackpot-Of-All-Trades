using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Mend")]
public class MendSO : SpellSO, ISpellBehavior
{
    public int baseHeal = 6;

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster)
    {
        if (isEnemyCaster)
        {
            // Assuming enemy healing: we might assume the enemy heals itself
            // For now, get the leftmost enemy as a proxy (you can adjust later)
            var enemy = combat.GetLeftmostEnemy();
            if (enemy != null)
            {
                enemy.Heal(baseHeal);
                Debug.Log($"{spellName} cast by enemy: healed {baseHeal} HP.");
            }
        }
        else
        {
            combat.HealPlayer(baseHeal);
            Debug.Log($"{spellName} cast by player: healed {baseHeal} HP.");
        }
    }
}
