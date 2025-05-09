using UnityEngine;

public static class TargetingOverride
{
    private static BaseEnemy overrideTarget;

    /// <summary>
    /// Set a specific enemy to override the default player targeting (e.g. not leftmost).
    /// </summary>
    public static void SetOverrideTarget(BaseEnemy enemy)
    {
        overrideTarget = enemy;
    }

    /// <summary>
    /// Get the current override target, or null if not set.
    /// </summary>
    public static BaseEnemy GetOverrideTarget()
    {
        return overrideTarget;
    }

    /// <summary>
    /// Clear any override so targeting reverts to leftmost logic.
    /// </summary>
    public static void Clear()
    {
        overrideTarget = null;
    }

    /// <summary>
    /// Check whether an override is currently active.
    /// </summary>
    public static bool HasOverride => overrideTarget != null;
}
