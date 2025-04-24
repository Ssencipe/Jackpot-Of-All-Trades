using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockManager : MonoBehaviour
{
    public int maxLocks = 3;
    private int currentLocks = 0;

    public TextMeshProUGUI lockCountText;
    public List<SpriteSelector> wheels;

    void Start()
    {
        ResetLockNumber();
        UpdateLockCountText();
        foreach (SpriteSelector wheel in wheels)
        {
            if (wheel != null && wheel.m_LockButton != null)
            {
                wheel.lockTuahScript.LockedIn(wheel.isLocked);
                UpdateLockButtonText(wheel);
            }
        }
    }

    public void ToggleLock(SpriteSelector wheel)
    {
        if (wheel.isLocked) UnlockWheel(wheel);
        else if (currentLocks < maxLocks) LockWheel(wheel);
        else Debug.Log("Maximum lock limit reached!");

        UpdateLockButtonText(wheel);
    }

    private void LockWheel(SpriteSelector wheel)
    {
        wheel.isLocked = true;
        currentLocks++;
        wheel.lockTuahScript.LockedIn(true);
        UpdateLockCountText();
        Debug.Log($"Locked {wheel.name}, current locks: {currentLocks}");
    }

    private void UnlockWheel(SpriteSelector wheel)
    {
        wheel.isLocked = false;
        currentLocks--;
        wheel.lockTuahScript.LockedIn(false);
        UpdateLockCountText();
        Debug.Log($"Unlocked {wheel.name}, current locks: {currentLocks}");
    }

    private void UpdateLockCountText()
    {
        if (lockCountText != null)
            lockCountText.text = $"Locks: {maxLocks - currentLocks}";
    }

    private void UpdateLockButtonText(SpriteSelector wheel)
    {
        var buttonText = wheel.m_LockButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
            buttonText.text = wheel.isLocked ? "Unlock" : "Lock";
    }

    private void ResetLockNumber()
    {
        // Think it should be reset on Start of Attack
        if (currentLocks != 0)
            currentLocks = 3;
    }
}
