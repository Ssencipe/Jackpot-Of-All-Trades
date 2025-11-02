using System.Collections.Generic;
using UnityEngine;

// defines the logic of what a stun effect does

[System.Serializable]
public class StunEffect : SpellEffectBase, IScalableEffect
{
    [Tooltip("Base number of turns stunned (minimum 1)")]
    public int duration = 1;

    public Sprite icon;
    public string label = "Stun";

    public TargetType targetType = TargetType.TargetEnemy;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;



    // IScalableEffect backing field
    private int scaleMultiplier = 1;
    public void SetScaleMultiplier(int multiplier)
    {
        scaleMultiplier = Mathf.Max(1, multiplier);
    }

    public override TargetType GetTargetType() => targetType;
    public override TargetingMode GetTargetingMode() => targetingMode;

    public override void Apply(SpellCastContext context, List<ITargetable> targets)
    {
        // compute final duration using the scale multiplier (ensure at least 1)
        int finalDuration = Mathf.Max(1, Mathf.RoundToInt(duration * scaleMultiplier));

        string sourceSpellName = context?.spellInstance?.spellData?.spellName;
        Sprite sourceIcon = context?.spellInstance?.spellData?.icon;

        foreach (var target in targets)
        {
            GameObject go = null;
            if (target is Unit unit)
                go = unit.gameObject;
            else if (target is BaseEnemy enemy)
                go = enemy.visualGameObject;
            else if (target is Component comp)
                go = comp.gameObject;

            if (go == null)
            {
                Debug.LogWarning($"[StunEffect] No GameObject found for target {target}. Skipping.");
                continue;
            }

            var controller = go.GetComponent<StatusEffectController>();
            if (controller == null)
            {
                Debug.LogWarning($"[StunEffect] No StatusEffectController on {go.name}. Skipping.");
                continue;
            }

            var instance = new StunStatusInstance(
                finalDuration,
                icon,
                label,
                sourceSpellName,
                sourceIcon,
                effectSound
            );

            // Expectation:StatusEffectController exposes AddEffect(IStatusEffect, ITargetable)
            controller.AddEffect(instance, target);
        }
    }

    public override ISpellEffect Clone()
    {
        return new StunEffect
        {
            duration = this.duration,
            icon = this.icon,
            label = this.label,
            targetingMode = this.targetingMode,
            targetType = this.targetType,
            effectSound = this.effectSound
            
        };
    }
}
