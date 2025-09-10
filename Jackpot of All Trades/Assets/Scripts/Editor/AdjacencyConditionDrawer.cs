#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AdjacencyCondition), true)]
public class AdjacencyConditionDrawer : PropertyDrawer
{
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;

        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("scope"), ref y, position);
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("comparison"), ref y, position);

        var comparison = (AdjacencyComparisonType)property.FindPropertyRelative("comparison").enumValueIndex;
        var scope = (NeighborScope)property.FindPropertyRelative("scope").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        // TargetTag
        if (comparison != AdjacencyComparisonType.Tag)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetTag only applies when Comparison is 'Tag'.");
        EditorDrawerUtils.DrawDisabledIf(comparison != AdjacencyComparisonType.Tag, property.FindPropertyRelative("targetTag"), ref y, position);

        // TargetColor
        if (comparison != AdjacencyComparisonType.Color)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetColor only applies when Comparison is 'Color'.");
        EditorDrawerUtils.DrawDisabledIf(comparison != AdjacencyComparisonType.Color, property.FindPropertyRelative("targetColor"), ref y, position);

        // TargetSpell
        if (comparison != AdjacencyComparisonType.ExactSpell)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetSpell only applies when Comparison is 'ExactSpell'.");
        EditorDrawerUtils.DrawDisabledIf(comparison != AdjacencyComparisonType.ExactSpell, property.FindPropertyRelative("targetSpell"), ref y, position);

        // TargetValue
        bool validValueComparison = comparison == AdjacencyComparisonType.TallyEquals || comparison == AdjacencyComparisonType.ChargeEquals;
        if (!validValueComparison)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetValue only applies to 'TallyEquals' or 'ChargeEquals'.");
        EditorDrawerUtils.DrawDisabledIf(!validValueComparison, property.FindPropertyRelative("targetValue"), ref y, position);

        // RelativeOffset
        if (scope != NeighborScope.Exact)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "RelativeOffset only applies when Scope is 'Exact'.");
        EditorDrawerUtils.DrawDisabledIf(scope != NeighborScope.Exact, property.FindPropertyRelative("relativeOffset"), ref y, position);

        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("requiredMatches"), ref y, position);
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("resultType"), ref y, position);

        // LinkedEffect
        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        // PotencyMultiplier
        if (resultType != ConditionResultType.ModifyPotency)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "PotencyMultiplier only applies when ResultType is 'ModifyPotency'.");
        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, property.FindPropertyRelative("potencyMultiplier"), ref y, position);

        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("scaleEffectWithMatches"), ref y, position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        var comparison = (AdjacencyComparisonType)property.FindPropertyRelative("comparison").enumValueIndex;
        var scope = (NeighborScope)property.FindPropertyRelative("scope").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;
        bool validValueComparison = comparison == AdjacencyComparisonType.TallyEquals || comparison == AdjacencyComparisonType.ChargeEquals;

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("scope"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("comparison"));

        if (comparison != AdjacencyComparisonType.Tag)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetTag"));

        if (comparison != AdjacencyComparisonType.Color)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetColor"));

        if (comparison != AdjacencyComparisonType.ExactSpell)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetSpell"));

        if (!validValueComparison)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetValue"));

        if (scope != NeighborScope.Exact)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("relativeOffset"));

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("requiredMatches"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("resultType"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        if (resultType != ConditionResultType.ModifyPotency)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("potencyMultiplier"));

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("scaleEffectWithMatches"));

        lastTotalHeight = height + 12f;
        return lastTotalHeight;
    }
}
#endif