using UnityEngine;

public static class TargetingOverride
{
    private static BaseEnemy overrideTarget;

    // Set a specific enemy to override the default player targeting (e.g. not leftmost).
    public static void SetOverrideTarget(BaseEnemy enemy)
    {
        overrideTarget = enemy;
    }

    // Get the current override target, or null if not set.
    public static BaseEnemy GetOverrideTarget()
    {
        return overrideTarget;
    }

    // Clear any override so targeting reverts to leftmost logic.
    public static void Clear()
    {
        overrideTarget = null;
    }

    // Check whether an override is currently active.
    public static bool HasOverride => overrideTarget != null;
}
