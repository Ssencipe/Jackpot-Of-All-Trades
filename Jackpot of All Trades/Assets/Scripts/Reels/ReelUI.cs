using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Affects UI elements like buttons and counters related to reels that change when affected or spun (like spin, lock, or nudge counters)

public class ReelUI : MonoBehaviour
{
    [Header("UI References")]
    public Button spinButton;

    [Header("Linked Reel Logic")]
    public Reel linkedReel;

    [Header("Managers")]
    public LockManager lockManager;
    public NudgeManager nudgeManager;

    [Header("Spin Tracking")]
    public TextMeshProUGUI spinCounter;
    public int maxSpins = 3;
    private int currentSpins;

    [HideInInspector]
    public bool isMasterReel = false;

    private void Start()
    {
        maxSpins = DevSettings.IsDevMode ? 99 : maxSpins;
        currentSpins = maxSpins;

        if (spinButton != null)
        {
            // Prevent multiple listeners from stacking
            spinButton.onClick.RemoveAllListeners();
            spinButton.onClick.AddListener(() => TrySpin());
        }

        UpdateSpinCounterText();
    }

    private void TrySpin()
    {
        if (!isMasterReel) return;

        if (AreAnyReelsSpinning())
        {
            Debug.Log("Spin prevented: Reels are already spinning.");
            return;
        }

        if (currentSpins <= 0)
        {
            Debug.Log("No spins remaining!");
            UpdateSpinCounterText();
            return;
        }

        Debug.Log($"[ReelUI] TrySpin() called. CurrentSpins: {currentSpins}");

        // Disable spin button immediately
        if (spinButton != null)
            spinButton.interactable = false;

        // Spin all unlocked reels
        Reel[] allReels = FindObjectsOfType<Reel>();
        foreach (var reel in allReels)
        {
            if (!reel.IsLocked)
                reel.Spin();
        }

        currentSpins--;
        UpdateSpinCounterText();

        // Re-enable button once all reels finish spinning
        StartCoroutine(WaitForSpinToFinish());
    }

    private void UpdateSpinCounterText()
    {
        if (!isMasterReel) return;

        if (spinCounter != null)
        {
            maxSpins = DevSettings.IsDevMode ? 99 : maxSpins;
            spinCounter.text = $"Spins: {currentSpins}";
        }

        if (currentSpins <= 0 && spinButton != null)
        {
            spinButton.interactable = false;
        }
    }

    public void ResetSpins()
    {
        if (!isMasterReel) return;

        maxSpins = DevSettings.IsDevMode ? 99 : maxSpins;
        currentSpins = maxSpins;
        UpdateSpinCounterText();

        if (spinButton != null)
            spinButton.interactable = true;
    }

    private IEnumerator WaitForSpinToFinish()
    {
        while (AreAnyReelsSpinning())
            yield return null;

        if (currentSpins > 0 && spinButton != null)
            spinButton.interactable = true;
    }

    private bool AreAnyReelsSpinning()
    {
        Reel[] allReels = FindObjectsOfType<Reel>();
        return allReels.Any(reel => reel.IsSpinning());
    }
}
