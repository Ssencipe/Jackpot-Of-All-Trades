#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    private const float ButtonHeight = 18f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true) + ButtonHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect buttonRect = new Rect(position.x, position.y, position.width, ButtonHeight);
        Rect fieldRect = new Rect(position.x, position.y + ButtonHeight, position.width, position.height - ButtonHeight);

        if (property.managedReferenceValue == null)
        {
            if (GUI.Button(buttonRect, "Add Spell Effect"))
            {
                ShowContextMenu(property);
            }
        }
        else
        {
            Type currentType = property.managedReferenceValue.GetType();
            GUI.Label(buttonRect, $"Effect: {currentType.Name}");

            if (GUI.Button(new Rect(position.xMax - 60, position.y, 60, ButtonHeight), "Change"))
            {
                ShowContextMenu(property);
            }

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(fieldRect, property, true);
            EditorGUI.indentLevel--;
        }
    }

    private void ShowContextMenu(SerializedProperty property)
    {
        var menu = new GenericMenu();
        var effectTypes = GetAllEffectTypes();

        foreach (var type in effectTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                property.serializedObject.Update();
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }

    private List<Type> GetAllEffectTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => typeof(ISpellEffect).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();
    }
}
#endif
