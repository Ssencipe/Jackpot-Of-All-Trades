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
            var effect = new OverTimeStatusInstance(potency, duration, type, tickTiming, icon);
            var go = (target as MonoBehaviour)?.gameObject;
            go?.GetComponent<StatusEffectController>()?.AddEffect(effect, target);
        }
    }
}