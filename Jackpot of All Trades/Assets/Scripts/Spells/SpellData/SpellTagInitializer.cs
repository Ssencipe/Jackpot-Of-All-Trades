using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class SpellTagInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSpellTags()
    {
        var allSpells = Resources.LoadAll<SpellSO>("");

        foreach (var spell in allSpells)
        {
            if (spell == null) continue;

            HashSet<SpellTag> detectedTags = new();

            // --- Check Conditions ---
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

            // --- Check Direct Spell Effects ---
            foreach (var effect in spell.effects)
            {
                if (effect is HealEffect) detectedTags.Add(SpellTag.Healing);
                if (effect is ShieldEffect) detectedTags.Add(SpellTag.Defense);
                if (effect is DamageEffect) detectedTags.Add(SpellTag.Offense);
            }

            // Ensure tag list exists
            if (spell.tags == null)
                spell.tags = new List<SpellTag>();

            // Sync tag list to match detected tags
            foreach (SpellTag tag in System.Enum.GetValues(typeof(SpellTag)))
            {
                bool shouldHave = detectedTags.Contains(tag);
                bool doesHave = spell.tags.Contains(tag);

                if (shouldHave && !doesHave)
                    spell.tags.Add(tag);
                else if (!shouldHave && doesHave)
                    spell.tags.Remove(tag);
            }

#if UNITY_EDITOR
            Debug.Log($"[SpellTagInitializer] Updated tags on '{spell.spellName}'");
#endif
        }
    }
}