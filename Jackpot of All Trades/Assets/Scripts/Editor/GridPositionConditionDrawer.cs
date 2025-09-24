#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GridPositionCondition), true)]
public class GridPositionConditionDrawer : PropertyDrawer
{
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;

        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("matchType"), ref y, position);

        var matchType = (GridPositionMatchType)property.FindPropertyRelative("matchType").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        if (matchType != GridPositionMatchType.Exact)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "TargetReel and TargetSlot are only used in Exact match mode.");

        EditorDrawerUtils.DrawDisabledIf(matchType != GridPositionMatchType.Exact, property.FindPropertyRelative("targetReel"), ref y, position);
        EditorDrawerUtils.DrawDisabledIf(matchType != GridPositionMatchType.Exact, property.FindPropertyRelative("targetSlot"), ref y, position);

        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("resultType"), ref y, position);

        if (resultType != ConditionResultType.TriggerEffect)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "LinkedEffect is only used when ResultType is 'TriggerEffect'.");

        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        if (resultType != ConditionResultType.ModifyPotency)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "PotencyMultiplier is only used when ResultType is 'ModifyPotency'.");

        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, property.FindPropertyRelative("potencyMultiplier"), ref y, position);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        var matchType = (GridPositionMatchType)property.FindPropertyRelative("matchType").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("matchType"));
        if (matchType != GridPositionMatchType.Exact)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetReel"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("targetSlot"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("resultType"));
        if (resultType != ConditionResultType.TriggerEffect)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("linkedEffect"), true);
        if (resultType != ConditionResultType.ModifyPotency)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("potencyMultiplier"));

        lastTotalHeight = height + 12f;
        return lastTotalHeight;
    }
}
#endif