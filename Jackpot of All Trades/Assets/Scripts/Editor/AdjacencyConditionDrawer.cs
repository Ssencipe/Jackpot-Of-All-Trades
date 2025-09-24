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

        // Scope dropdown
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("scope"), ref y, position);

        // Help box for comparison types
        EditorDrawerUtils.DrawHelpBox(ref y, position, "See AdjacencyCondition script for explanations of types.");
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("comparison"), ref y, position);

        var comparison = (AdjacencyComparisonType)property.FindPropertyRelative("comparison").enumValueIndex;
        var scope = (NeighborScope)property.FindPropertyRelative("scope").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        // Scaling toggle
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("scaleEffectWithMatches"), ref y, position);

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

        // TargetValue (multiple valid comparisons)
        bool usesTargetValue =
            comparison.ToString().StartsWith("Exact") ||
            comparison.ToString().StartsWith("Total");

        bool supportsRange =
    comparison.ToString().StartsWith("Total") &&
    !comparison.ToString().Contains("Exact");

        // target value logic
        if (supportsRange)
        {
            EditorDrawerUtils.DrawHelpBox(ref y, position, "Exact for specific values, Floor for minimum, Ceiling for maximum, Range for between Floor and Ceiling");
            EditorDrawerUtils.DrawLine(property.FindPropertyRelative("valueMode"), ref y, position);

            var valueMode = (ValueComparisonMode)property.FindPropertyRelative("valueMode").enumValueIndex;

            EditorDrawerUtils.DrawDisabledIf(valueMode != ValueComparisonMode.Exact, property.FindPropertyRelative("targetValue"), ref y, position);
            EditorDrawerUtils.DrawDisabledIf(valueMode != ValueComparisonMode.Floor && valueMode != ValueComparisonMode.Range, property.FindPropertyRelative("floorValue"), ref y, position);
            EditorDrawerUtils.DrawDisabledIf(valueMode != ValueComparisonMode.Ceiling && valueMode != ValueComparisonMode.Range, property.FindPropertyRelative("ceilingValue"), ref y, position);

            if (valueMode == ValueComparisonMode.Range &&
                property.FindPropertyRelative("floorValue").intValue >
                property.FindPropertyRelative("ceilingValue").intValue)
            {
                EditorDrawerUtils.DrawErrorBox(ref y, position, "Warning: Floor Value is greater than Ceiling Value!");
            }
        }
        else
        {
            if (usesTargetValue)
            {
                EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetValue only applies to numeric comparisons.");
                EditorDrawerUtils.DrawLine(property.FindPropertyRelative("targetValue"), ref y, position);
            }
            else
            {
                EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetValue only applies to comparisons involving numeric matching or totals.");
            }
        }

        // RelativeOffset
        if (scope != NeighborScope.Exact)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "RelativeOffset only applies when Scope is 'Exact'.");
        EditorDrawerUtils.DrawDisabledIf(scope != NeighborScope.Exact, property.FindPropertyRelative("relativeOffset"), ref y, position);

        // Required Matches
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("requiredMatches"), ref y, position);

        // Result Type
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("resultType"), ref y, position);

        // Linked Effect (if trigger effect)
        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        // Potency Multiplier (if modify potency)
        if (resultType != ConditionResultType.ModifyPotency)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "PotencyMultiplier only applies when ResultType is 'ModifyPotency'.");
        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, property.FindPropertyRelative("potencyMultiplier"), ref y, position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        var comparison = (AdjacencyComparisonType)property.FindPropertyRelative("comparison").enumValueIndex;
        var scope = (NeighborScope)property.FindPropertyRelative("scope").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;
        var valueMode = (ValueComparisonMode)property.FindPropertyRelative("valueMode").enumValueIndex;

        bool usesTargetValue =
            comparison.ToString().StartsWith("Exact") ||
            comparison.ToString().StartsWith("Total");

        bool supportsRange =
            comparison.ToString().StartsWith("Total") &&
            !comparison.ToString().Contains("Exact");

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("scope"));
        height += EditorDrawerUtils.HelpBoxHeight(); // Help box above comparison
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("comparison"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("scaleEffectWithMatches"));

        if (comparison != AdjacencyComparisonType.Tag)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetTag"));

        if (comparison != AdjacencyComparisonType.Color)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetColor"));

        if (comparison != AdjacencyComparisonType.ExactSpell)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetSpell"));


        // value comparisons
        if (supportsRange)
        {
            height += EditorDrawerUtils.HelpBoxHeight(); // Explanation box
            height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("valueMode"));

            // Always include heights — even if disabled
            height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetValue"));
            height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("floorValue"));
            height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("ceilingValue"));

            if (valueMode == ValueComparisonMode.Range &&
                property.FindPropertyRelative("floorValue").intValue >
                property.FindPropertyRelative("ceilingValue").intValue)
            {
                height += EditorDrawerUtils.HelpBoxHeight(); // Error box
            }
        }
        else
        {
            height += EditorDrawerUtils.HelpBoxHeight(); // Explanation or fallback
            height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetValue"));
        }

        if (scope != NeighborScope.Exact)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("relativeOffset"));

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("requiredMatches"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("resultType"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        if (resultType != ConditionResultType.ModifyPotency)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("potencyMultiplier"));

        lastTotalHeight = height + 12f;
        return lastTotalHeight;
    }
}
#endif