using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Condition that checks data of neighboring spells

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
    DuplicateTally,             // tally is same as spell tally
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
    IsMirrored,                 // checked spells mirrored (same as each other) on axis across this spell
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
    public NeighborScope scope;                        // Scope of the check
    public AdjacencyComparisonType comparison;                     // What to compare
    public NeighborModification neighborModification;              // Optional neighbor modification to apply
    public override NeighborModification GetNeighborModification() => neighborModification;

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

        Debug.Log($"[AdjacencyCondition] Evaluating for: {centerData.spellName}");

        // Special handling for IsDuplicate / MirroredDuplicate across global grid
        if ((comparison == AdjacencyComparisonType.IsDuplicate || comparison == AdjacencyComparisonType.MirroredDuplicate)
            && scope == NeighborScope.AllSpells)
        {
            int total = GridManager.AllSpells().Count(s => s != null && s.spellData == centerData);
            return total > 1;
        }

        // Mirrored checks
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

                case NeighborScope.AllSpells:
                    int mirrorX = GridManager.Reels - 1 - x;
                    int mirrorY = GridManager.SlotsPerReel - 1 - y;
                    var mirror = GridManager.GetSpellAt(mirrorX, mirrorY);
                    return mirror != null && mirror.spellData == centerData;

                default:
                    return false;
            }
        }

        List<BaseSpell> neighbors = GridManager.GetSpellsInScope(context, context.spellInstance, scope);
        Debug.Log($"[AdjacencyCondition] Found {neighbors.Count} neighbors for scope {scope}");

        // Step 1: Count how many spells match the condition
        List<BaseSpell> matchingSpells = comparison == AdjacencyComparisonType.IsDuplicate
            ? neighbors.Where(s => s != null && s.spellData == centerData).ToList()
            : neighbors.Where(s => SpellMatches(s, context)).ToList();

        int matchCount = matchingSpells.Count;
        Debug.Log($"[AdjacencyCondition] Comparison: {comparison}, MatchCount: {matchCount}, RequiredMatches: {requiredMatches}");

        // Step 2: If it's a sum-based comparison, calculate the numeric sum of matched spells
        int valueSum = 0;

        switch (comparison)
        {
            case AdjacencyComparisonType.TotalTally:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.tally);
                break;

            case AdjacencyComparisonType.TotalCharge:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.charge);
                break;

            case AdjacencyComparisonType.TotalPotency:
                valueSum = Mathf.RoundToInt(matchingSpells.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.potencyMultiplier));
                break;

            case AdjacencyComparisonType.TotalTags:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).Sum(n => n.runtimeSpell.tags.Count);
                break;

            case AdjacencyComparisonType.TotalUniqueTags:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).SelectMany(n => n.runtimeSpell.tags).Distinct().Count();
                break;

            case AdjacencyComparisonType.TotalUniqueColors:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).Select(n => n.runtimeSpell.colorType).Distinct().Count();
                break;

            case AdjacencyComparisonType.TotalUniqueSpells:
                valueSum = matchingSpells.Where(n => n?.spellData != null).Select(n => n.spellData).Distinct().Count();
                break;

            case AdjacencyComparisonType.TotalUniqueTally:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).Select(n => n.runtimeSpell.tally).Distinct().Count();
                break;

            case AdjacencyComparisonType.TotalUniqueCharge:
                valueSum = matchingSpells.Where(n => n?.runtimeSpell != null).Select(n => n.runtimeSpell.charge).Distinct().Count();
                break;

            case AdjacencyComparisonType.TotalUniquePotency:
                valueSum = matchingSpells
                    .Where(n => n?.runtimeSpell != null)
                    .Select(n => Mathf.RoundToInt(n.runtimeSpell.potencyMultiplier * 100f))
                    .Distinct().Count();
                break;
        }

        // Step 3: Evaluate both match count and optional value total
        bool passesMatchRequirement = matchCount >= requiredMatches;
        bool passesValueCheck = true;

        bool isSumBasedComparison = comparison.ToString().StartsWith("Total") && !comparison.ToString().Contains("Unique");

        if (isSumBasedComparison)
        {
            passesValueCheck = PassesValueComparison(valueSum);
            Debug.Log($"[AdjacencyCondition] ValueSum: {valueSum}, Target: {targetValue}, PassedValueCheck: {passesValueCheck}");
        }

        // Step 4: Optional scaling for effects
        if (scaleEffectWithMatches && linkedEffect is IScalableEffect scalable)
        {
            scalable.SetScaleMultiplier(matchCount);
        }

        // Final result: both match and value conditions must be met
        bool passed = passesMatchRequirement && passesValueCheck;
        Debug.Log($"[AdjacencyCondition] Final Passed: {passed}");

        // Apply neighbor modification only if condition passes
        if (passed && resultType == ConditionResultType.ModifyNeighbor && neighborModification != null)
        {
            Debug.Log($"[AdjacencyCondition] Triggering neighbor modification");
            neighborModification.Apply(context, context.spellInstance);
        }

        return passed;
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