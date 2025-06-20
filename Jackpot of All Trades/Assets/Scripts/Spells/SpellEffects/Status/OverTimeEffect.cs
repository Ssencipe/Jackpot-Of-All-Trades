using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OverTimeEffect : ISpellEffect
{
    public int potency;
    public int duration;
    public OverTimeType type;
    public TickTiming tickTiming;
    public Sprite icon;

    public TargetType targetType = TargetType.TargetEnemy;
    public TargetingMode targetingMode = TargetingMode.SingleEnemy;

    public TargetType GetTargetType() => targetType;
    public TargetingMode GetTargetingMode() => targetingMode;


    public void Apply(SpellCastContext context, List<ITargetable> targets)
    {
        foreach (var target in targets)
        {
            Debug.Log($"[OverTimeEffect] Attempting to apply {type} to {targets.Count} target(s)");

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

            var effect = new OverTimeStatusInstance(potency, duration, type, tickTiming, icon);
            controller.AddEffect(effect, target);
        }
    }

    //runtime cloning of SO
    public ISpellEffect Clone()
    {
        return new OverTimeEffect
        {
            potency = this.potency,
            duration = this.duration,
            type = this.type,
            tickTiming = this.tickTiming,
            icon = this.icon,
            targetType = this.targetType,
            targetingMode = this.targetingMode
        };
    }
}