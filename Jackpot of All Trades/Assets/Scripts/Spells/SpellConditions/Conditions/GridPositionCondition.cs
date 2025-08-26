using UnityEngine;

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
    public GridPositionMatchType matchType;

    public int targetReel;
    public int targetSlot;

    public ConditionResultType resultType = ConditionResultType.TriggerEffect;

    [SerializeReference, SubclassSelector]
    public ISpellEffect linkedEffect;

    public float potencyMultiplier = 1f;

    public override bool Evaluate(SpellCastContext context)
    {
        int x = context.spellInstance.reelIndex;
        int y = context.spellInstance.slotIndex;

        return matchType switch
        {
            GridPositionMatchType.TopRow => y == 0,
            GridPositionMatchType.BottomRow => y == GridManager.SlotsPerReel - 1,
            GridPositionMatchType.CenterRow => y == GridManager.SlotsPerReel / 2,
            GridPositionMatchType.LeftReel => x == 0,
            GridPositionMatchType.RightReel => x == GridManager.Reels - 1,
            GridPositionMatchType.CenterReel => x == GridManager.Reels / 2,
            GridPositionMatchType.SecondReel => x == 1,
            GridPositionMatchType.ThirdReel => x == 2,
            GridPositionMatchType.Corner => (x == 0 || x == GridManager.Reels - 1) && (y == 0 || y == GridManager.SlotsPerReel - 1),
            GridPositionMatchType.Exact => x == targetReel && y == targetSlot,
            _ => false
        };
    }

    public override ConditionResultType GetResultType() => resultType;
    public override ISpellEffect GetLinkedEffect() => linkedEffect;
    public override float GetPotencyMultiplier() => potencyMultiplier;
}