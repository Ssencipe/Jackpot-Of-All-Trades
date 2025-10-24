#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// The UI used when setting a composite condition for a spell scriptable object

[CustomPropertyDrawer(typeof(CompositeCondition), true)]
public class CompositeConditionDrawer : PropertyDrawer
{
    private float lastTotalHeight = 0f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;

        // Logic Type
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("logicType"), ref y, position);

        // Condition A
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("conditionA"), ref y, position, true);

        var logicType = (LogicType)property.FindPropertyRelative("logicType").enumValueIndex;
        var conditionB = property.FindPropertyRelative("conditionB");
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        // Condition B
        if (!(logicType == LogicType.AND || logicType == LogicType.OR))
            EditorDrawerUtils.DrawHelpBox(ref y, position, "Condition B is only used with AND/OR.");
        EditorDrawerUtils.DrawDisabledIf(!(logicType == LogicType.AND || logicType == LogicType.OR), conditionB, ref y, position, true);

        // Result type
        EditorDrawerUtils.DrawLine(property.FindPropertyRelative("resultType"), ref y, position);

        // Linked Effect
        if (resultType != ConditionResultType.TriggerEffect)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "LinkedEffect is only used when ResultType is 'TriggerEffect'.");
        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, property.FindPropertyRelative("linkedEffect"), ref y, position, true);

        // Modify Potency
        if (resultType != ConditionResultType.ModifyPotency)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "PotencyMultiplier is only used when ResultType is 'ModifyPotency'.");
        EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, property.FindPropertyRelative("potencyMultiplier"), ref y, position);

        // Modify Neighbor
        if (resultType == ConditionResultType.ModifyNeighbor)
        {
            SerializedProperty neighborMod = property.FindPropertyRelative("neighborModification");

            if (neighborMod != null)
            {
                EditorDrawerUtils.DrawHelpBox(ref y, position, "Choose which spells are affected and how.");
                EditorDrawerUtils.DrawLine(neighborMod.FindPropertyRelative("scope"), ref y, position);
                EditorDrawerUtils.DrawLine(neighborMod.FindPropertyRelative("type"), ref y, position);
                EditorDrawerUtils.DrawLine(neighborMod.FindPropertyRelative("amount"), ref y, position);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;
        var logicType = (LogicType)property.FindPropertyRelative("logicType").enumValueIndex;
        var resultType = (ConditionResultType)property.FindPropertyRelative("resultType").enumValueIndex;

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("logicType"));
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("conditionA"), true);
        if (!(logicType == LogicType.AND || logicType == LogicType.OR))
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("conditionB"), true);

        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("resultType"));

        if (resultType != ConditionResultType.TriggerEffect)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("linkedEffect"), true);

        if (resultType != ConditionResultType.ModifyPotency)
            height += EditorDrawerUtils.HelpBoxHeight();
        height += EditorDrawerUtils.GetHeight(property.FindPropertyRelative("potencyMultiplier"));

        if (resultType == ConditionResultType.ModifyNeighbor)
        {
            SerializedProperty neighborMod = property.FindPropertyRelative("neighborModification");

            if (neighborMod != null)
            {
                height += EditorDrawerUtils.HelpBoxHeight();
                height += EditorDrawerUtils.GetHeight(neighborMod.FindPropertyRelative("scope"));
                height += EditorDrawerUtils.GetHeight(neighborMod.FindPropertyRelative("type"));
                height += EditorDrawerUtils.GetHeight(neighborMod.FindPropertyRelative("amount"));
            }
        }

        lastTotalHeight = height + 12f;
        return lastTotalHeight;
    }
}
#endif