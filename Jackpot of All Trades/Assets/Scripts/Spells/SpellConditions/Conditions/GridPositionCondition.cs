using UnityEngine;
using NaughtyAttributes;

public enum GridPositionMatchType
{
    TopRow,
    BottomRow,
    CenterRow,
    LeftReel,
    RightReel,
    CenterReel,
    SecondReel,
    ThirdReel,
    Corner,
    Exact
}

[System.Serializable]
public class GridPositionCondition : SpellConditionBase
{
    [Header("Position Condition")]
    public GridPositionMatchType matchType;

    [ShowIf(nameof(IsExactMatch))]
    [Tooltip("Used only when matchType is 'Exact'")]
    public int targetReel;

    [ShowIf(nameof(IsExactMatch))]
    public int targetSlot;

    [Header("Condition Result")]
    public ConditionResultType resultType = ConditionResultType.TriggerEffect;

    [ShowIf(nameof(IsTriggerEffect))]
    [SerializeReference, SubclassSelector]
    public ISpellEffect linkedEffect;

    [ShowIf(nameof(IsModifyPotency))]
    [Tooltip("Used only when resultType is ModifyPotency")]
    public float potencyMultiplier = 1f;

    private bool IsExactMatch() => matchType == GridPositionMatchType.Exact;
    private bool IsModifyPotency() => resultType == ConditionResultType.ModifyPotency;
    private bool IsTriggerEffect() => resultType == ConditionResultType.TriggerEffect;

    public override bool Evaluate(SpellCastContext context)
    {
        int x = context.spellInstance.reelIndex;
        int y = context.spellInstance.slotIndex;

        switch (matchType)
        {
            case GridPositionMatchType.TopRow:
                return y == 0;

            case GridPositionMatchType.BottomRow:
                return y == GridManager.SlotsPerReel - 1;

            case GridPositionMatchType.CenterRow:
                return y == GridManager.SlotsPerReel / 2;

            case GridPositionMatchType.LeftReel:
                return x == 0;

            case GridPositionMatchType.RightReel:
                return x == GridManager.Reels - 1;

            case GridPositionMatchType.CenterReel:
                return x == GridManager.Reels / 2;

            case GridPositionMatchType.SecondReel:
                return x == 1;

            case GridPositionMatchType.ThirdReel:
                return x == 2;

            case GridPositionMatchType.Corner:
                return (x == 0 || x == GridManager.Reels - 1) && (y == 0 || y == GridManager.SlotsPerReel - 1);

            case GridPositionMatchType.Exact:
                return x == targetReel && y == targetSlot;

            default:
                return false;
        }
    }

    public override ConditionResultType GetResultType() => resultType;
    public override ISpellEffect GetLinkedEffect() => linkedEffect;
    public override float GetPotencyMultiplier() => potencyMultiplier;
}
