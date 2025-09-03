using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeEffect : SpellEffectBase, IScalableEffect
{
    public int amount = 1;
    private int scaleMultiplier = 1;

    public override TargetType GetTargetType() => TargetType.TargetAlly ;
    public override TargetingMode GetTargetingMode() => TargetingMode.Self;

    public void SetScaleMultiplier(int multiplier)
    {
        scaleMultiplier = multiplier;
    }

    public override void Apply(SpellCastContext context, List<ITargetable> targets)
    {
        var spell = context.spellInstance.runtimeSpell;
        if (!spell.hasCharges)
        {
            Debug.LogWarning($"[ChargeEffect] Spell {spell.spellName} does not support charges.");
            return;
        }

        int delta = Mathf.RoundToInt(amount * spell.potencyMultiplier * scaleMultiplier);
        int old = spell.charge;
        spell.charge = Mathf.Max(0, old + delta);

        Debug.Log($"[ChargeEffect] {spell.spellName}: Charge changed from {old} to {spell.charge} ({delta})");
    }

    public override ISpellEffect Clone()
    {
        return new ChargeEffect
        {
            amount = this.amount,
            effectSound = this.effectSound
        };
    }
}