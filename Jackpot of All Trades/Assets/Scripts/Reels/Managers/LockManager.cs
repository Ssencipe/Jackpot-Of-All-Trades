using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LockManager : MonoBehaviour
{
    [Header("Lock Settings")]
    public int maxLocks = 3;
    private int currentLocks = 0;

    [Header("UI")]
    public TextMeshProUGUI lockCountText;

    [Header("Managed Reels")]
    public List<Reel> reels = new List<Reel>();

    private void Start()
    {
        ResetLocks();
    }

    public void RegisterReels(List<Reel> spawnedReels)
    {
        reels.Clear();
        reels.AddRange(spawnedReels);
    }

    public void ToggleLock(Reel reel)
    {
        if (reel.IsLocked)
            UnlockReel(reel);
        else if (currentLocks < maxLocks)
            LockReel(reel);
        else
            Debug.Log("Maximum lock limit reached!");

        UpdateLockCountText();
    }

    public void LockReel(Reel reel)
    {
        reel.Lock();
        currentLocks++;
        UpdateLockCountText();
    }

    public void UnlockReel(Reel reel)
    {
        reel.Unlock();
        currentLocks--;
        UpdateLockCountText();
    }

    private void UpdateLockCountText()
    {
        if (lockCountText != null)
        {
            maxLocks = DevSettings.IsDevMode ? 99 : maxLocks;
            lockCountText.text = $"Locks: {maxLocks - currentLocks}";
        }
    }

    public void ResetLocks()
    {
        currentLocks = 0;
        foreach (var reel in reels)
        {
            reel.Unlock();
        }
        UpdateLockCountText();
    }
}
