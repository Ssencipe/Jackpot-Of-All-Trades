using System.Linq;
using UnityEngine;
using System.Collections;

// Defines methods to be called for feedback. Currently just used for on-hit effects and flash effects (brief color changes on affected sprites) when any damage, shielding, or healing occurs.
public static class FeedbackManager
{
    public static void Flash(ITargetable target, FlashType type)
    {
        Debug.Log($"[FeedbackManager] Flash triggered: {type} on {target}");

        if (target is BaseEnemy enemy)
        {
            var ui = Object.FindObjectsOfType<EnemyUI>().FirstOrDefault(e => e.BaseEnemy == enemy);
            ui?.GetComponentInChildren<FlashEffect>()?.Flash(type);
        }
        else if (target is BasePlayer player)
        {
            player.GetComponentInChildren<FlashEffect>()?.Flash(type);
        }
    }

    // not currently working
    public static void ShakeCamera(float duration = 0.2f, float magnitude = 0.15f)
    {
        var shakeController = Object.FindObjectOfType<ScreenShakeController>();
        shakeController?.Shake(duration, magnitude);
    }

    // freeze frame
    public static void HitStop(float duration = 0.05f, MonoBehaviour caller = null)
    {
        if (caller != null)
        {
            caller.StartCoroutine(HitStopRoutine(duration));
        }
    }

    private static IEnumerator HitStopRoutine(float duration)
    {
        float originalSpeed = GameSpeedManager.CurrentSpeed;

        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalSpeed;
    }
}