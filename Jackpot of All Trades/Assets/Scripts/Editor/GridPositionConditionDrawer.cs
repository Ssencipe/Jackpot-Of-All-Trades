#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GridPositionCondition), true)]
public class GridPositionConditionDrawer : PropertyDrawer
{
    private const float Padding = 2f;
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        DrawLine(property.FindPropertyRelative("matchType"), ref y, position);

        var matchType = (GridPositionMatchType)property.FindPropertyRelative("matchType").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        if (matchType != GridPositionMatchType.Exact)
        {
            DrawHelpBox(ref y, position, "TargetReel and TargetSlot are only used in Exact match mode.");
        }

        DrawDisabledIf(matchType != GridPositionMatchType.Exact, property.FindPropertyRelative("targetReel"), ref y, position);
        DrawDisabledIf(matchType != GridPositionMatchType.Exact, property.FindPropertyRelative("targetSlot"), ref y, position);

        DrawLine(property.FindPropertyRelative("resultType"), ref y, position);

        if (resultType != ConditionResultType.TriggerEffect)
        {
            DrawHelpBox(ref y, position, "LinkedEffect is only used when ResultType is 'TriggerEffect'.");
        }

        DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        if (resultType != ConditionResultType.ModifyPotency)
        {
            DrawHelpBox(ref y, position, "PotencyMultiplier is only used when ResultType is 'ModifyPotency'.");
        }

        DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, property.FindPropertyRelative("potencyMultiplier"), ref y, position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        height += GetHeight(property.FindPropertyRelative("matchType"));
        var matchType = (GridPositionMatchType)property.FindPropertyRelative("matchType").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        if (matchType != GridPositionMatchType.Exact)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("targetReel"));
        height += GetHeight(property.FindPropertyRelative("targetSlot"));
        height += GetHeight(property.FindPropertyRelative("resultType"));

        if (resultType != ConditionResultType.TriggerEffect)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        if (resultType != ConditionResultType.ModifyPotency)
            height += HelpBoxHeight();

        height += GetHeight(property.FindPropertyRelative("potencyMultiplier"));

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