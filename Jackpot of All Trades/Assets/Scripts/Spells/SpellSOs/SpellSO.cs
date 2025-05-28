using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/Generic Spell")]
public class SpellSO : ScriptableObject
{
    [Header("Info")]
    public string spellName;
    [TextArea] public string description;
    public Sprite icon;
    public int charge;

    [Header("Classification")]
    public ColorType colorType;
    public List<SpellTag> tags;

    [Header("Effects")]
    [Tooltip("Each effect defines a separate behavior (e.g., damage, heal, shield).")]
    [SerializeReference, SubclassSelector]
    public List<ISpellEffect> effects = new();

    public void Cast(BaseSpell instance, CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        var context = new SpellCastContext
        {
            spellInstance = instance,
            combat = combat,
            grid = grid,
            isEnemyCaster = isEnemyCaster,
            enemyCaster = enemyCaster,
            playerCaster = combat.playerUnit,
            enemyTeam = combat.CurrentEnemies.ToList()
        };

        foreach (var effect in effects)
        {
            var targets = TargetingManager.ResolveTargets(
                effect.GetTargetType(),
                effect.GetTargetingMode(),
                new TargetingContext
                {
                    isEnemyCaster = isEnemyCaster,
                    combat = combat,
                    grid = grid,
                    playerCaster = combat.playerUnit,
                    enemyCaster = enemyCaster,
                    enemyTeam = combat.CurrentEnemies.ToList()
                });

            Debug.Log($"Effect {effect.GetType().Name} resolved {targets.Count} targets");

            effect.Apply(context, targets);
        }
    }
}