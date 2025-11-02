#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// What is drawn when a spell SO is given a composite condition

[CustomPropertyDrawer(typeof(CompositeCondition), true)]
public class CompositeConditionDrawer : PropertyDrawer
{
    private float lastTotalHeight = 0f;
    private const float spacing = 2f;

    public float GetTotalPropertyHeight() => lastTotalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        float y = position.y;

        // Logic type
        var logicProp = property.FindPropertyRelative("logicType");
        EditorDrawerUtils.DrawLine(logicProp, ref y, position);

        // Conditions list
        var conditionsProp = property.FindPropertyRelative("conditions");
        if (conditionsProp != null)
        {
            EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "Conditions");
            y += EditorGUIUtility.singleLineHeight + spacing;

            for (int i = 0; i < conditionsProp.arraySize; i++)
            {
                var condProp = conditionsProp.GetArrayElementAtIndex(i);
                EditorDrawerUtils.DrawLine(condProp, ref y, position, true);
            }

            if (GUI.Button(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight), "Add Condition"))
            {
                conditionsProp.InsertArrayElementAtIndex(conditionsProp.arraySize);
            }
            y += EditorGUIUtility.singleLineHeight + spacing;
        }

        EditorDrawerUtils.DrawWarningBox(ref y, position, "This sets the result for the entire composite condition, not the subconditions.");
        var resultTypeProp = property.FindPropertyRelative("resultType");
        var resultType = (ConditionResultType)(resultTypeProp?.enumValueIndex ?? 0);

        // Result type
        if (resultTypeProp != null)
            EditorDrawerUtils.DrawLine(resultTypeProp, ref y, position);

        // Linked effect
        var linkedEffectProp = property.FindPropertyRelative("linkedEffect");
        if (resultType != ConditionResultType.TriggerEffect)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "LinkedEffect is only used when ResultType is 'TriggerEffect'.");
        if (linkedEffectProp != null)
            EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.TriggerEffect, linkedEffectProp, ref y, position, true);

        // Modify potency
        var potencyProp = property.FindPropertyRelative("potencyMultiplier");
        if (resultType != ConditionResultType.ModifyPotency)
            EditorDrawerUtils.DrawHelpBox(ref y, position, "PotencyMultiplier is only used when ResultType is 'ModifyPotency'.");
        if (potencyProp != null)
            EditorDrawerUtils.DrawDisabledIf(resultType != ConditionResultType.ModifyPotency, potencyProp, ref y, position);

        // Modify neighbor
        var neighborModProp = property.FindPropertyRelative("neighborModification");
        if (resultType == ConditionResultType.ModifyNeighbor && neighborModProp != null)
        {
            EditorDrawerUtils.DrawHelpBox(ref y, position, "Choose which spells are affected and how.");
            var scopeProp = neighborModProp.FindPropertyRelative("scope");
            var typeProp = neighborModProp.FindPropertyRelative("type");
            var amountProp = neighborModProp.FindPropertyRelative("amount");

            if (scopeProp != null) EditorDrawerUtils.DrawLine(scopeProp, ref y, position);
            if (typeProp != null) EditorDrawerUtils.DrawLine(typeProp, ref y, position);
            if (amountProp != null) EditorDrawerUtils.DrawLine(amountProp, ref y, position);
        }

        EditorGUI.EndProperty();
    }

    // Here you define how much spacing should be added to the UI for each of the drawer elements
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0f;

        var logicProp = property.FindPropertyRelative("logicType");
        var conditionsProp = property.FindPropertyRelative("conditions");
        var resultTypeProp = property.FindPropertyRelative("resultType");
        var linkedEffectProp = property.FindPropertyRelative("linkedEffect");
        var potencyProp = property.FindPropertyRelative("potencyMultiplier");
        var neighborModProp = property.FindPropertyRelative("neighborModification");

        var resultType = (ConditionResultType)(resultTypeProp?.enumValueIndex ?? 0);

        if (logicProp != null) height += EditorDrawerUtils.GetHeight(logicProp);

        if (conditionsProp != null)
        {
            height += EditorGUIUtility.singleLineHeight + spacing;
            for (int i = 0; i < conditionsProp.arraySize; i++)
            {
                height += EditorDrawerUtils.GetHeight(conditionsProp.GetArrayElementAtIndex(i), true);
            }
            height += EditorGUIUtility.singleLineHeight + spacing; // for Add button
        }

        height += EditorDrawerUtils.HelpBoxHeight();
        if (resultTypeProp != null) height += EditorDrawerUtils.GetHeight(resultTypeProp);

        if (resultType != ConditionResultType.TriggerEffect)
            height += EditorDrawerUtils.HelpBoxHeight();
        if (linkedEffectProp != null) height += EditorDrawerUtils.GetHeight(linkedEffectProp, true);

        if (resultType != ConditionResultType.ModifyPotency)
            height += EditorDrawerUtils.HelpBoxHeight();
        if (potencyProp != null) height += EditorDrawerUtils.GetHeight(potencyProp);

        if (resultType == ConditionResultType.ModifyNeighbor && neighborModProp != null)
        {
            height += EditorDrawerUtils.HelpBoxHeight();
            height += EditorDrawerUtils.GetHeight(neighborModProp.FindPropertyRelative("scope"));
            height += EditorDrawerUtils.GetHeight(neighborModProp.FindPropertyRelative("type"));
            height += EditorDrawerUtils.GetHeight(neighborModProp.FindPropertyRelative("amount"));
        }

        lastTotalHeight = height + 12f;
        return lastTotalHeight;
    }
}
#endif