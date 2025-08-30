using UnityEngine;

public static class SpellVisualUtil
{
    public static Color GetColorForRuntimeSpell(RuntimeSpell spell)
    {
        if (spell == null) return Color.white;

        if (spell.wasMarkedToSkip)
            return new Color(0.3f, 0.3f, 0.3f, 0.65f); // Skipped: gray

        if (spell.wasPotencyModified)
            return (spell.potencyMultiplier < 1f)
                ? new Color(1f, 0.5f, 0f)              // Nerfed: orange
                : Color.blue;                          // Buffed: blue

        return Color.white; // Default
    }
}