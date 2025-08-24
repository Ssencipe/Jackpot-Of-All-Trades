using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OverTimeEffect : SpellEffectBase
{
    public int potency;
    public int duration;
    public OverTimeType type;
    public TickTiming tickTiming;
    public Sprite icon;

    public string label = "Unnamed Status";

    public TargetType targetType = TargetType.TargetEnemy;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;

    public override TargetType GetTargetType() => targetType;
    public override TargetingMode GetTargetingMode() => targetingMode;


    public override void Apply(SpellCastContext context, List<ITargetable> targets)
    {
        int finalPotency = Mathf.RoundToInt(potency * context.spellInstance.runtimeSpell.potencyMultiplier);
        int finalDuration = Mathf.RoundToInt(duration * context.spellInstance.runtimeSpell.potencyMultiplier);

        foreach (var target in targets)
        {
            GameObject go = null;

            if (target is Unit unit)
                go = unit.gameObject;
            else if (target is BaseEnemy enemy)
                go = enemy.visualGameObject;

            if (go == null)
            {
                Debug.LogWarning($"[OverTimeEffect] No GameObject found for target {target}");
                continue;
            }

            var controller = go.GetComponent<StatusEffectController>();
            if (controller == null)
            {
                Debug.LogWarning($"[OverTimeEffect] No StatusEffectController on {go.name}");
                continue;
            }

            var effect = new OverTimeStatusInstance(
                finalPotency,
                finalDuration,
                type,
                tickTiming,
                icon,
                effectSound,
                label,
                context.spellInstance?.spellData?.spellName,
                context.spellInstance?.spellData?.icon
            );

            controller.AddEffect(effect, target);
        }
    }

    //runtime cloning of SO
    public override ISpellEffect Clone()
    {
        return new OverTimeEffect
        {
            potency = this.potency,
            duration = this.duration,
            type = this.type,
            tickTiming = this.tickTiming,
            icon = this.icon,
            targetType = this.targetType,
            targetingMode = this.targetingMode,
            effectSound = this.effectSound,
            label = this.label,
        };
    }
}