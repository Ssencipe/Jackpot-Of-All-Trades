#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom drawer for fields marked with [SubclassSelector]. Allows runtime subclass selection and correct property rendering.
/// </summary>
[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    private const float ButtonHeight = 20f;
    private const float Padding = 4f;

    // Calculates the height needed for the entire property (single or list)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (IsGenericArray(property))
        {
            float height = EditorGUIUtility.singleLineHeight + Padding;

            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                height += GetElementHeight(element) + Padding * 2;
            }

            height += EditorGUIUtility.singleLineHeight + Padding;
            return height;
        }

        return GetElementHeight(property) + ButtonHeight + Padding;
    }

    // Main entry point for GUI rendering
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (IsGenericArray(property))
            DrawReferenceList(position, property, label);
        else
            DrawReferenceProperty(position, property, label);
    }

    // Handles rendering of lists (e.g., List<ICondition>)
    private void DrawReferenceList(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label);
        position.y += EditorGUIUtility.singleLineHeight + Padding;

        for (int i = 0; i < property.arraySize; i++)
        {
            SerializedProperty element = property.GetArrayElementAtIndex(i);
            float elementHeight = GetElementHeight(element);

            var contentRect = new Rect(position.x, position.y, position.width - 50f, elementHeight);
            DrawWithProperDrawer(contentRect, element, new GUIContent($"Element {i + 1}"));

            var removeBtn = new Rect(position.xMax - 22, position.y + 2, 18, 18);
            if (GUI.Button(removeBtn, "X"))
            {
                property.DeleteArrayElementAtIndex(i);
                break;
            }

            position.y += elementHeight + Padding;
        }

        // Dropdown to add new subclass
        if (GUI.Button(new Rect(position.x, position.y, 120, EditorGUIUtility.singleLineHeight), "Add Entry"))
        {
            Type listElementType = GetElementType(fieldInfo);
            var subTypes = GetAllSubTypes(listElementType);
            GenericMenu menu = new GenericMenu();

            foreach (var type in subTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    property.serializedObject.Update();
                    property.arraySize++;
                    var newElement = property.GetArrayElementAtIndex(property.arraySize - 1);
                    newElement.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }

    // Handles single property selection/drawing
    private void DrawReferenceProperty(Rect position, SerializedProperty property, GUIContent label)
    {
        var buttonRect = new Rect(position.x, position.y, position.width, ButtonHeight);
        var fieldRect = new Rect(position.x, position.y + ButtonHeight + Padding, position.width, GetElementHeight(property));
        var fieldType = GetFieldType(fieldInfo);

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
            var currentType = property.managedReferenceValue.GetType();
            GUI.Label(buttonRect, $"{fieldType.Name}: {currentType.Name}");

            if (GUI.Button(new Rect(buttonRect.xMax - 60, buttonRect.y, 60, ButtonHeight), "Change"))
            {
                ShowContextMenu(property, fieldType);
            }

            DrawWithProperDrawer(fieldRect, property, label);
        }
    }

    // Draws the actual object using a custom drawer if available
    private void DrawWithProperDrawer(Rect rect, SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
        {
            EditorGUI.PropertyField(rect, property, label, true);
            return;
        }

        var drawerType = GetCustomDrawerType(property.managedReferenceValue.GetType());

        if (drawerType != null)
        {
            property.isExpanded = true;

            var drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
            drawer.OnGUI(rect, property, label);
        }
        else
        {
            property.isExpanded = true;
            EditorGUI.PropertyField(rect, property, label, true);
        }
    }

    // Calculates correct height — uses GetTotalPropertyHeight() if drawer supports it
    private float GetElementHeight(SerializedProperty property)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        property.isExpanded = true;

        var drawerType = GetCustomDrawerType(property.managedReferenceValue.GetType());

        if (drawerType != null)
        {
            var drawer = (PropertyDrawer)Activator.CreateInstance(drawerType);
            var heightMethod = drawerType.GetMethod("GetTotalPropertyHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (heightMethod != null)
            {
                drawer.GetPropertyHeight(property, GUIContent.none); // force drawer to update its layout
                return (float)heightMethod.Invoke(drawer, null);
            }

            return drawer.GetPropertyHeight(property, GUIContent.none);
        }

        return EditorGUI.GetPropertyHeight(property, true);
    }

    // Finds custom drawer type for a subclass
    private Type GetCustomDrawerType(Type objectType)
    {
        foreach (var drawerType in TypeCache.GetTypesWithAttribute<CustomPropertyDrawer>())
        {
            var attributes = drawerType.GetCustomAttributes(typeof(CustomPropertyDrawer), false) as CustomPropertyDrawer[];

            if (attributes == null) continue;

            foreach (var attr in attributes)
            {
                var typeField = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
                var useForChildrenField = typeof(CustomPropertyDrawer).GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);

                if (typeField == null || useForChildrenField == null) continue;

                var targetType = typeField.GetValue(attr) as Type;
                var useForChildren = (bool)useForChildrenField.GetValue(attr);

                if (targetType == objectType || (useForChildren && targetType.IsAssignableFrom(objectType)))
                    return drawerType;
            }
        }

        return null;
    }

    // Show context menu for selecting subclasses
    private void ShowContextMenu(SerializedProperty property, Type baseType)
    {
        var subTypes = GetAllSubTypes(baseType);
        var menu = new GenericMenu();

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
            .SelectMany(a => a.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .OrderBy(t => t.Name)
            .ToList();
    }

    private Type GetFieldType(FieldInfo info)
    {
        if (info.FieldType.IsGenericType && info.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            return info.FieldType.GetGenericArguments()[0];

        return info.FieldType;
    }

    private Type GetElementType(FieldInfo info) => GetFieldType(info);

    private bool IsGenericArray(SerializedProperty property)
    {
        return property.isArray && property.propertyType == SerializedPropertyType.Generic;
    }
}
#endif