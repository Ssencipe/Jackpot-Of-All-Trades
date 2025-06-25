using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class SpellEffectBase : ISpellEffect
{
    //ISpellEffect implementation:
    public abstract void Apply(SpellCastContext context, List<ITargetable> targets);
    public abstract TargetType GetTargetType();
    public abstract TargetingMode GetTargetingMode();
    public abstract ISpellEffect Clone();

    [Header("Audio is Optional")]
    public string effectSound;
}