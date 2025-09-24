using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEngine;

public enum NeighborScope
{
    Adjacent,                   // left right top bottom
    Diagonal,                   // the corners
    AllSurrounding,             // adjacent + diagonal
    Horizontal,                 // left right
    Vertical,                   // top bottom
    Exact,                      // one of the 8 surrounding
    GlobalGrid                  // a specific spell from the entire grid
}

public enum AdjacencyComparisonType
{
    Tag,                        // has specific tag
    TotalTags,                  // all checked spells have threshold of tags
    TotalUniqueTags,            // all checked spells have threshold of non-repeat tags
    Color,                      // has specific color
    TotalUniqueColors,          // all checked spells have threshold of non-repeat colors
    ExactSpell,                 // is a specific spell
    TotalUniqueSpells,          // all checked spells have threshold of non-repeat spells
    HasTally,                   // has tally
    ExactTally,                 // has specific tally value
    TotalTally,                 // all checked spells have threshold of tally
    TotalUniqueTally,           // all checked spells have threshold of non-repeat tallies
    TallyChanged,               // tally value changes
    DuplicateTally,             // tally is same as  spell tally
    HasCharge,                  // has charge
    ExactCharge,                // has specific charge value
    TotalCharge,                // all checked spells at threshold of charge
    TotalUniqueCharge,          // all checked spells have threshold of non-repeat charges
    DuplicateCharge,            // charge is same as this spell charge
    ExactPotency,               // specific potency value
    TotalPotency,               // all checked spells have threshold of potency value
    TotalUniquePotency,         // all checked spells have threshold of non-repeat potencies
    LowPotency,                 // potency is below 1
    HighPotency,                // potency is above 1
    DuplicatePotency,           // potency is same as this spell potency
    IsDuplicate,                // checked spells same as this spell
    IsMirrored,                 // checked spells mirrored (same as each other) on axis across this spell (left and right, top and bottom, corners)
    MirroredDuplicate           // checked spells are mirrored and same as this spell
}

public enum ValueComparisonMode
{
    Exact,                      // checks for a target value
    Floor,                      // checks for above a target value
    Ceiling,                    // checks for below a target value
    Range                       // checks for value between floor and ceiling
}

[System.Serializable]
public class AdjacencyCondition : SpellConditionBase
{
    public NeighborScope scope;
    public AdjacencyComparisonType comparison;

    public SpellTag targetTag;
    public ColorType targetColor;
    public SpellSO targetSpell;

    public ValueComparisonMode valueMode = ValueComparisonMode.Exact;
    public int targetValue;
    public int ceilingValue;
    public int floorValue;

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

        // Handle sum-based comparisons
        switch (comparison)
        {
            case AdjacencyComparisonType.TotalTally:
                matchCount = neighbors.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.tally);
                break;

            case AdjacencyComparisonType.TotalCharge:
                matchCount = neighbors.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.charge);
                break;

            case AdjacencyComparisonType.TotalPotency:
                matchCount = Mathf.RoundToInt(neighbors.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.potencyMultiplier));
                break;

            case AdjacencyComparisonType.TotalTags:
                matchCount = neighbors
                    .Where(n => n?.runtimeSpell != null)
                    .Sum(n => n.runtimeSpell.tags.Count);
                break;

            case AdjacencyComparisonType.TotalUniqueTags:
                matchCount = neighbors
                    .Where(n => n?.runtimeSpell != null)
                    .SelectMany(n => n.runtimeSpell.tags)
                    .Distinct()
                    .Count();
                break;

            case AdjacencyComparisonType.TotalUniqueColors:
                matchCount = neighbors
                    .Where(n => n?.runtimeSpell != null)
                    .Select(n => n.runtimeSpell.colorType)
                    .Distinct()
                    .Count();
                break;

            case AdjacencyComparisonType.TotalUniqueSpells:
                matchCount = neighbors
                    .Where(n => n?.spellData != null)
                    .Select(n => n.spellData)
                    .Distinct()
                    .Count();
                break;

            case AdjacencyComparisonType.TotalUniqueTally:
                matchCount = neighbors
                    .Where(n => n?.runtimeSpell != null)
                    .Select(n => n.runtimeSpell.tally)
                    .Distinct()
                    .Count();
                break;

            case AdjacencyComparisonType.TotalUniqueCharge:
                matchCount = neighbors
                    .Where(n => n?.runtimeSpell != null)
                    .Select(n => n.runtimeSpell.charge)
                    .Distinct()
                    .Count();
                break;

            case AdjacencyComparisonType.TotalUniquePotency:
                matchCount = neighbors
                    .Where(n => n?.runtimeSpell != null)
                    .Select(n => Mathf.RoundToInt(n.runtimeSpell.potencyMultiplier * 100f)) // avoid float precision issues
                    .Distinct()
                    .Count();
                break;
        }

        // evaluate comparisons
        if (scaleEffectWithMatches && linkedEffect is IScalableEffect scalable)
        {
            scalable.SetScaleMultiplier(matchCount);
            return matchCount > 0;
        }
        else
        {
            return PassesValueComparison(matchCount);
        }
    }

    private bool SpellMatches(BaseSpell spell, SpellCastContext context)
    {
        if (spell?.runtimeSpell == null) return false;
        var rs = spell.runtimeSpell;
        var center = context.spellInstance.runtimeSpell;

        return comparison switch
        {
            AdjacencyComparisonType.Tag => rs.tags.Contains(targetTag),
            AdjacencyComparisonType.Color => rs.colorType == targetColor,
            AdjacencyComparisonType.ExactSpell => spell.spellData == targetSpell,
            AdjacencyComparisonType.TallyChanged => rs.HasTallyChanged(),
            AdjacencyComparisonType.HasTally => rs.tally > 0,
            AdjacencyComparisonType.HasCharge => rs.charge > 0,
            AdjacencyComparisonType.DuplicateTally => center != null && rs.tally == center.tally,
            AdjacencyComparisonType.DuplicateCharge => center != null && rs.charge == center.charge,
            AdjacencyComparisonType.DuplicatePotency => Mathf.Approximately(rs.potencyMultiplier, center.potencyMultiplier),
            AdjacencyComparisonType.ExactTally => rs.tally == targetValue,
            AdjacencyComparisonType.ExactCharge => rs.charge == targetValue,
            AdjacencyComparisonType.ExactPotency => Mathf.Approximately(rs.potencyMultiplier, targetValue),
            AdjacencyComparisonType.LowPotency => rs.potencyMultiplier < 1f,
            AdjacencyComparisonType.HighPotency => rs.potencyMultiplier > 1f,
            _ => false
        };
    }

    private bool HasValueRangeIssue() =>
    valueMode == ValueComparisonMode.Range && floorValue > ceilingValue;

    private bool PassesValueComparison(int matchCount)
    {
        return valueMode switch
        {
            ValueComparisonMode.Exact => matchCount == targetValue,
            ValueComparisonMode.Floor => matchCount >= floorValue,
            ValueComparisonMode.Ceiling => matchCount <= ceilingValue,
            ValueComparisonMode.Range => matchCount >= floorValue && matchCount <= ceilingValue,
            _ => false
        };
    }

    public override ConditionResultType GetResultType() => resultType;
    public override ISpellEffect GetLinkedEffect() => linkedEffect;
    public override float GetPotencyMultiplier() => potencyMultiplier;
}
