#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

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

        Type fieldType = GetFieldType(fieldInfo);
        if (fieldType == null)
        {
            EditorGUI.LabelField(buttonRect, "Cannot determine base type");
            return;
        }

        if (property.managedReferenceValue == null)
        {
            string niceName = ObjectNames.NicifyVariableName(property.name);
            if (GUI.Button(buttonRect, $"Add {niceName}"))
            {
                ShowContextMenu(property, fieldType);
            }
        }
        else
        {
            Type currentType = property.managedReferenceValue.GetType();
            GUI.Label(buttonRect, $"{fieldType.Name}: {currentType.Name}");

            if (GUI.Button(new Rect(position.xMax - 60, position.y, 60, ButtonHeight), "Change"))
            {
                ShowContextMenu(property, fieldType);
            }

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(fieldRect, property, true);
            EditorGUI.indentLevel--;
        }
    }

    private void ShowContextMenu(SerializedProperty property, Type baseType)
    {
        var menu = new GenericMenu();
        var subTypes = GetAllSubTypes(baseType);

        foreach (var type in subTypes)
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

    private List<Type> GetAllSubTypes(Type baseType)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => asm.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .OrderBy(t => t.Name)
            .ToList();
    }

    private Type GetFieldType(FieldInfo fieldInfo)
    {
        if (fieldInfo.FieldType.IsGenericType &&
            typeof(List<>).IsAssignableFrom(fieldInfo.FieldType.GetGenericTypeDefinition()))
        {
            return fieldInfo.FieldType.GetGenericArguments()[0];
        }

        return fieldInfo.FieldType;
    }
}
#endif
