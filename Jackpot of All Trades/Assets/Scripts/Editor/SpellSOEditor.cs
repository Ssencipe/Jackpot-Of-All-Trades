#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpellSO))]
public class SpellSOEditor : Editor
{
    private SpellSO spell;

    private void OnEnable()
    {
        spell = (SpellSO)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Auto-Tag from Effects & Conditions"))
        {
            AutoTagSpell();
        }
    }

    private void AutoTagSpell()
    {
        if (spell == null) return;

        HashSet<SpellTag> detectedTags = new();

        foreach (var condition in spell.conditions)
        {
            if (condition is GridPositionCondition)
                detectedTags.Add(SpellTag.Positional);

            if (condition is AdjacencyCondition)
                detectedTags.Add(SpellTag.Adjacency);

            var linked = condition.GetLinkedEffect();
            if (linked is HealEffect) detectedTags.Add(SpellTag.Healing);
            if (linked is ShieldEffect) detectedTags.Add(SpellTag.Defense);
            if (linked is DamageEffect) detectedTags.Add(SpellTag.Offense);
        }

        foreach (var effect in spell.effects)
        {
            if (effect is HealEffect) detectedTags.Add(SpellTag.Healing);
            if (effect is ShieldEffect) detectedTags.Add(SpellTag.Defense);
            if (effect is DamageEffect) detectedTags.Add(SpellTag.Offense);
        }

        if (spell.tags == null)
            spell.tags = new List<SpellTag>();

        foreach (SpellTag tag in System.Enum.GetValues(typeof(SpellTag)))
        {
            bool shouldHave = detectedTags.Contains(tag);
            bool doesHave = spell.tags.Contains(tag);

            if (shouldHave && !doesHave)
                spell.tags.Add(tag);
            else if (!shouldHave && doesHave)
                spell.tags.Remove(tag);
        }

        EditorUtility.SetDirty(spell);
        AssetDatabase.SaveAssets();
        Debug.Log($"[SpellSOEditor] Updated tags on '{spell.spellName}'");
    }
}
#endif