#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AdjacencyCondition), true)]
public class AdjacencyConditionDrawer : PropertyDrawer
{
    private const float Padding = 2f;
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;

        DrawLine(property.FindPropertyRelative("scope"), ref y, position);
        DrawLine(property.FindPropertyRelative("comparison"), ref y, position);

        var comparison = (AdjacencyComparisonType)property.FindPropertyRelative("comparison").enumValueIndex;
        var scope = (NeighborScope)property.FindPropertyRelative("scope").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        // TargetTag
        if (comparison != AdjacencyComparisonType.Tag)
        {
            DrawHelpBox(ref y, position, "TargetTag only applies when Comparison is 'Tag'.");
        }
        DrawDisabledIf(comparison != AdjacencyComparisonType.Tag, property.FindPropertyRelative("targetTag"), ref y, position);

        // TargetColor
        if (comparison != AdjacencyComparisonType.Color)
        {
            DrawHelpBox(ref y, position, "TargetColor only applies when Comparison is 'Color'.");
        }
        DrawDisabledIf(comparison != AdjacencyComparisonType.Color, property.FindPropertyRelative("targetColor"), ref y, position);

        // TargetSpell
        if (comparison != AdjacencyComparisonType.ExactSpell)
        {
            DrawHelpBox(ref y, position, "TargetSpell only applies when Comparison is 'ExactSpell'.");
        }
        DrawDisabledIf(comparison != AdjacencyComparisonType.ExactSpell, property.FindPropertyRelative("targetSpell"), ref y, position);

        // TargetValue
        bool validValueComparison = comparison == AdjacencyComparisonType.TallyEquals || comparison == AdjacencyComparisonType.ChargeEquals;
        if (!validValueComparison)
        {
            DrawHelpBox(ref y, position, "TargetValue only applies to 'TallyEquals' or 'ChargeEquals'.");
        }
        DrawDisabledIf(!validValueComparison, property.FindPropertyRelative("targetValue"), ref y, position);

        // RelativeOffset
        if (scope != NeighborScope.Exact)
        {
            DrawHelpBox(ref y, position, "RelativeOffset only applies when Scope is 'Exact'.");
        }
        DrawDisabledIf(scope != NeighborScope.Exact, property.FindPropertyRelative("relativeOffset"), ref y, position);

        DrawLine(property.FindPropertyRelative("requiredMatches"), ref y, position);
        DrawLine(property.FindPropertyRelative("resultType"), ref y, position);

        // LinkedEffect
        DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        // PotencyMultiplier
        if (resultType != ConditionResultType.ModifyPotency)
        {
            DrawHelpBox(ref y, position, "PotencyMultiplier only applies when ResultType is 'ModifyPotency'.");
        }
        DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, property.FindPropertyRelative("potencyMultiplier"), ref y, position);

        DrawLine(property.FindPropertyRelative("scaleEffectWithMatches"), ref y, position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        height += GetHeight(property.FindPropertyRelative("scope"));
        height += GetHeight(property.FindPropertyRelative("comparison"));

        var comparison = (AdjacencyComparisonType)property.FindPropertyRelative("comparison").enumValueIndex;
        var scope = (NeighborScope)property.FindPropertyRelative("scope").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        if (comparison != AdjacencyComparisonType.Tag)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("targetTag"));

        if (comparison != AdjacencyComparisonType.Color)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("targetColor"));

        if (comparison != AdjacencyComparisonType.ExactSpell)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("targetSpell"));

        bool validValueComparison = comparison == AdjacencyComparisonType.TallyEquals || comparison == AdjacencyComparisonType.ChargeEquals;
        if (!validValueComparison)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("targetValue"));

        if (scope != NeighborScope.Exact)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("relativeOffset"));
        height += GetHeight(property.FindPropertyRelative("requiredMatches"));
        height += GetHeight(property.FindPropertyRelative("resultType"));
        height += GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        if (resultType != ConditionResultType.ModifyPotency)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("potencyMultiplier"));

        height += GetHeight(property.FindPropertyRelative("scaleEffectWithMatches"));

        const float ExtraBottomPadding = 12f;
        lastTotalHeight = height + ExtraBottomPadding;
        return lastTotalHeight;
    }

    private void DrawHelpBox(ref float y, Rect position, string message)
    {
        float boxHeight = HelpBoxHeight();
        EditorGUI.HelpBox(new Rect(position.x, y, position.width, boxHeight), message, MessageType.Info);
        y += boxHeight + Padding;
    }

    private float HelpBoxHeight() => EditorGUIUtility.singleLineHeight * 1.5f;

    private void DrawLine(SerializedProperty prop, ref float y, Rect position, bool includeChildren = false)
    {
        float height = EditorGUI.GetPropertyHeight(prop, includeChildren);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop, includeChildren);
        y += height + Padding;
    }

    private void DrawDisabledIf(bool condition, SerializedProperty prop, ref float y, Rect position, bool includeChildren = false)
    {
        EditorGUI.BeginDisabledGroup(condition);
        DrawLine(prop, ref y, position, includeChildren);
        EditorGUI.EndDisabledGroup();
    }

    private float GetHeight(SerializedProperty prop, bool includeChildren = false)
    {
        return EditorGUI.GetPropertyHeight(prop, includeChildren) + Padding;
    }
}
#endif