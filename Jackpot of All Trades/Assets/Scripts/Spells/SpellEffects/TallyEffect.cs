using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TallyEffect : SpellEffectBase, IScalableEffect
{
    public int amount = 1;
    private int scaleMultiplier = 1;

    public override TargetType GetTargetType() => TargetType.TargetAlly;
    public override TargetingMode GetTargetingMode() => TargetingMode.Self;

    public void SetScaleMultiplier(int multiplier)
    {
        scaleMultiplier = multiplier;
    }

    public override void Apply(SpellCastContext context, List<ITargetable> targets)
    {
        var spell = context.spellInstance.runtimeSpell;
        if (!spell.hasTallies)
        {
            Debug.LogWarning($"[TallyEffect] Spell {spell.spellName} does not support tallies.");
            return;
        }

        int delta = Mathf.RoundToInt(amount * spell.potencyMultiplier * scaleMultiplier);
        int old = spell.tally;
        spell.SetTally(old + delta);

        Debug.Log($"[TallyEffect] {spell.spellName}: Tally changed from {old} to {spell.tally} ({delta})");
    }

    public override ISpellEffect Clone()
    {
        return new TallyEffect
        {
            amount = this.amount,
            effectSound = this.effectSound
        };
    }
}