#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Predefined drawer elements that can be referenced to quickly build or edit chunks of drawers for interfacing. "HelpBoxHeight" is a good stand in for all "Draw[Type}Box" methods.
public static class EditorDrawerUtils
{
    private const float Padding = 2f;

    public static void DrawLine(SerializedProperty prop, ref float y, Rect position, bool includeChildren = false)
    {
        float height = EditorGUI.GetPropertyHeight(prop, includeChildren);
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, height), prop, includeChildren);
        y += height + Padding;
    }

    public static void DrawDisabledIf(bool condition, SerializedProperty prop, ref float y, Rect position, bool includeChildren = false)
    {
        EditorGUI.BeginDisabledGroup(condition);
        DrawLine(prop, ref y, position, includeChildren);
        EditorGUI.EndDisabledGroup();
    }

    public static void DrawHelpBox(ref float y, Rect position, string message)
    {
        float boxHeight = HelpBoxHeight();
        EditorGUI.HelpBox(new Rect(position.x, y, position.width, boxHeight), message, MessageType.Info);
        y += boxHeight + Padding;
    }

    public static void DrawWarningBox(ref float y, Rect position, string message)
    {
        float boxHeight = HelpBoxHeight();
        EditorGUI.HelpBox(new Rect(position.x, y, position.width, boxHeight), message, MessageType.Warning);
        y += boxHeight + Padding;
    }

    public static void DrawErrorBox(ref float y, Rect position, string message)
    {
        float boxHeight = HelpBoxHeight();
        EditorGUI.HelpBox(new Rect(position.x, y, position.width, boxHeight), message, MessageType.Error);
        y += boxHeight + Padding;
    }

    public static float GetHeight(SerializedProperty prop, bool includeChildren = false)
    {
        return EditorGUI.GetPropertyHeight(prop, includeChildren) + Padding;
    }

    public static float HelpBoxHeight()
    {
        return EditorGUIUtility.singleLineHeight * 1.5f;
    }
}
#endif