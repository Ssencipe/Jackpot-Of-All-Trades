#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CompositeCondition), true)]
public class CompositeConditionDrawer : PropertyDrawer
{
    private const float Padding = 2f;
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        // Draw logic type field
        DrawLine(property.FindPropertyRelative("logicType"), ref y, position);

        // Draw first condition unconditionally
        DrawLine(property.FindPropertyRelative("conditionA"), ref y, position, true);

        var logicType = (LogicType)property.FindPropertyRelative("logicType").enumValueIndex;
        var conditionB = property.FindPropertyRelative("conditionB");

        // Show helpbox if conditionB is disabled
        if (!(logicType == LogicType.AND || logicType == LogicType.OR))
        {
            var helpRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight * 1.5f);
            EditorGUI.HelpBox(helpRect, "Condition B is only used with AND/OR.", MessageType.Info);
            y += helpRect.height + Padding;
        }

        DrawDisabledIf(!(logicType == LogicType.AND || logicType == LogicType.OR), conditionB, ref y, position, true);

        // Draw effect
        DrawLine(property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        height += GetHeight(property.FindPropertyRelative("logicType"));
        height += GetHeight(property.FindPropertyRelative("conditionA"), true);

        var logicType = (LogicType)property.FindPropertyRelative("logicType").enumValueIndex;
        if (!(logicType == LogicType.AND || logicType == LogicType.OR))
            height += EditorGUIUtility.singleLineHeight * 1.5f + Padding; // HelpBox

        height += GetHeight(property.FindPropertyRelative("conditionB"), true);
        height += GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        const float ExtraBottomPadding = 12f;
        lastTotalHeight = height + ExtraBottomPadding;
        return lastTotalHeight;
    }

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