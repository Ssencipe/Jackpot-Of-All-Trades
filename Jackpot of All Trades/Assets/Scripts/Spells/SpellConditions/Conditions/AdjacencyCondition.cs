using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public enum NeighborScope
{
    Adjacent,
    Diagonal,
    AllSurrounding,
    Horizontal,
    Vertical,
    Exact,
    GlobalGrid
}

public enum AdjacencyComparisonType
{
    Tag,
    Color,
    ExactSpell,
    TallyEquals,
    TallyChanged,
    ChargeEquals,
    IsDuplicate,
    IsMirrored,
    MirroredDuplicate
}

[System.Serializable]
public class AdjacencyCondition : SpellConditionBase
{
    public NeighborScope scope;
    public AdjacencyComparisonType comparison;

    public SpellTag targetTag;
    public ColorType targetColor;
    public SpellSO targetSpell;
    public int targetValue;
    public Vector2Int relativeOffset;

    public int requiredMatches = 1;

    public ConditionResultType resultType = ConditionResultType.TriggerEffect;

    public bool scaleEffectWithMatches = false;

    [SerializeReference, SubclassSelector]
    public ISpellEffect linkedEffect;
    public float potencyMultiplier = 1f;

    public override bool Evaluate(SpellCastContext context)
    {
        int x = context.spellInstance.reelIndex;
        int y = context.spellInstance.slotIndex;

        SpellSO centerData = context.spellInstance.spellData;

        if ((comparison == AdjacencyComparisonType.IsDuplicate || comparison == AdjacencyComparisonType.MirroredDuplicate)
            && scope == NeighborScope.GlobalGrid)
        {
            int total = GridManager.AllSpells().Count(s => s != null && s.spellData == centerData);
            return total > 1;
        }

        if (comparison == AdjacencyComparisonType.IsMirrored || comparison == AdjacencyComparisonType.MirroredDuplicate)
        {
            bool IsSame(BaseSpell a, BaseSpell b) =>
                a != null && b != null && a.spellData == b.spellData;

            bool IsDuplicateOfCenter(BaseSpell a) =>
                a != null && a.spellData == centerData;

            switch (scope)
            {
                case NeighborScope.Horizontal:
                    var left = GridManager.GetSpellAt(x - 1, y);
                    var right = GridManager.GetSpellAt(x + 1, y);
                    return comparison == AdjacencyComparisonType.IsMirrored
                        ? IsSame(left, right)
                        : IsDuplicateOfCenter(left) && IsDuplicateOfCenter(right);

                case NeighborScope.Vertical:
                    var down = GridManager.GetSpellAt(x, y - 1);
                    var up = GridManager.GetSpellAt(x, y + 1);
                    return comparison == AdjacencyComparisonType.IsMirrored
                        ? IsSame(down, up)
                        : IsDuplicateOfCenter(down) && IsDuplicateOfCenter(up);

                case NeighborScope.Diagonal:
                    var tl = GridManager.GetSpellAt(x - 1, y + 1);
                    var br = GridManager.GetSpellAt(x + 1, y - 1);
                    var tr = GridManager.GetSpellAt(x + 1, y + 1);
                    var bl = GridManager.GetSpellAt(x - 1, y - 1);
                    return comparison == AdjacencyComparisonType.IsMirrored
                        ? IsSame(tl, br) || IsSame(tr, bl)
                        : (IsDuplicateOfCenter(tl) && IsDuplicateOfCenter(br)) ||
                          (IsDuplicateOfCenter(tr) && IsDuplicateOfCenter(bl));

                case NeighborScope.AllSurrounding:
                    return comparison == AdjacencyComparisonType.IsMirrored
                        ? (
                            IsSame(GridManager.GetSpellAt(x - 1, y), GridManager.GetSpellAt(x + 1, y)) ||
                            IsSame(GridManager.GetSpellAt(x, y - 1), GridManager.GetSpellAt(x, y + 1)) ||
                            IsSame(GridManager.GetSpellAt(x - 1, y - 1), GridManager.GetSpellAt(x + 1, y + 1)) ||
                            IsSame(GridManager.GetSpellAt(x - 1, y + 1), GridManager.GetSpellAt(x + 1, y - 1))
                        )
                        : (
                            IsDuplicateOfCenter(GridManager.GetSpellAt(x - 1, y)) && IsDuplicateOfCenter(GridManager.GetSpellAt(x + 1, y)) ||
                            IsDuplicateOfCenter(GridManager.GetSpellAt(x, y - 1)) && IsDuplicateOfCenter(GridManager.GetSpellAt(x, y + 1)) ||
                            IsDuplicateOfCenter(GridManager.GetSpellAt(x - 1, y - 1)) && IsDuplicateOfCenter(GridManager.GetSpellAt(x + 1, y + 1)) ||
                            IsDuplicateOfCenter(GridManager.GetSpellAt(x - 1, y + 1)) && IsDuplicateOfCenter(GridManager.GetSpellAt(x + 1, y - 1))
                        );

                case NeighborScope.GlobalGrid:
                    int mirrorX = GridManager.Reels - 1 - x;
                    int mirrorY = GridManager.SlotsPerReel - 1 - y;
                    var mirror = GridManager.GetSpellAt(mirrorX, mirrorY);
                    return mirror != null && mirror.spellData == centerData;

                default:
                    return false;
            }
        }

        // Non-mirrored logic path
        List<BaseSpell> neighbors = scope switch
        {
            NeighborScope.Adjacent => GridManager.GetVisibleNeighbors(x, y, cardinalOnly: true),
            NeighborScope.Diagonal => GridManager.GetVisibleNeighbors(x, y, diagonalsOnly: true),
            NeighborScope.AllSurrounding => GridManager.GetVisibleNeighbors(x, y),
            NeighborScope.Horizontal => GridManager.GetVisibleDirectionalNeighbors(x, y, new[] { Vector2Int.left, Vector2Int.right }),
            NeighborScope.Vertical => GridManager.GetVisibleDirectionalNeighbors(x, y, new[] { Vector2Int.up, Vector2Int.down }),
            NeighborScope.Exact =>
                GridManager.IsVisible(x + relativeOffset.x, y + relativeOffset.y)
                    ? new List<BaseSpell> { GridManager.GetSpellAt(x + relativeOffset.x, y + relativeOffset.y) }
                    : new List<BaseSpell>(),

            NeighborScope.GlobalGrid => GridManager.AllSpells(),

            _ => new List<BaseSpell>()
        };

        int matchCount = 0;

        if (comparison == AdjacencyComparisonType.IsDuplicate)
        {
            matchCount = neighbors.Count(s => s != null && s.spellData == centerData);
        }
        else
        {
            matchCount = neighbors.Count(spell => SpellMatches(spell, context));
        }

        if (scaleEffectWithMatches && linkedEffect is IScalableEffect scalable)
        {
            scalable.SetScaleMultiplier(matchCount);
        }

        return matchCount >= requiredMatches;
    }

    private bool SpellMatches(BaseSpell spell, SpellCastContext context)
    {
     if (spell?.runtimeSpell == null) return false;

      return comparison switch
        {
        AdjacencyComparisonType.Tag => spell.runtimeSpell.tags.Contains(targetTag),
        AdjacencyComparisonType.Color => spell.runtimeSpell.colorType == targetColor,
        AdjacencyComparisonType.ExactSpell => spell.spellData == targetSpell,
        AdjacencyComparisonType.TallyEquals => spell.runtimeSpell.tally == targetValue,
        AdjacencyComparisonType.ChargeEquals => spell.runtimeSpell.charge == targetValue,
        AdjacencyComparisonType.TallyChanged => spell.runtimeSpell.HasTallyChanged(),
        _ => false
         };
    }

    public override ConditionResultType GetResultType() => resultType;
    public override ISpellEffect GetLinkedEffect() => linkedEffect;
    public override float GetPotencyMultiplier() => potencyMultiplier;
}
