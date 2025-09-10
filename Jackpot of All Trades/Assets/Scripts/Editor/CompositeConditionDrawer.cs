#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CompositeCondition), true)]
public class CompositeConditionDrawer : PropertyDrawer
{
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;

        // Draw logic type field
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("logicType"), ref y, position);

        // Draw first condition unconditionally
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("conditionA"), ref y, position, true);

        var logicType = (LogicType)property.FindPropertyRelative("logicType").enumValueIndex;
        var conditionB = property.FindPropertyRelative("conditionB");

        // Show helpbox if conditionB is disabled
        if (!(logicType == LogicType.AND || logicType == LogicType.OR))
        {
            EditorDrawerUtils.DrawHelpBox(ref y, position, "Condition B is only used with AND/OR.");
        }

        EditorDrawerUtils.DrawDisabledIf(!(logicType == LogicType.AND || logicType == LogicType.OR), conditionB, ref y, position, true);

        // Draw effect
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        var logicType = (LogicType)property.FindPropertyRelative("logicType").enumValueIndex;

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("logicType"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("conditionA"), true);
        if (!(logicType == LogicType.AND || logicType == LogicType.OR))
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("conditionB"), true);
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        lastTotalHeight = height + 12f;
        return lastTotalHeight;
    }
}
#endif