using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReelUI : MonoBehaviour
{
    [Header("UI References")]
    public Button spinButton;
    public Button lockButton;
    public Button nudgeUpButton;
    public Button nudgeDownButton;
    public TextMeshProUGUI lockButtonText;

    [Header("Linked Reel Logic")]
    public Reel linkedReel;

    [Header("Managers")]
    public LockManager lockManager;
    public NudgeManager nudgeManager;

    [Header("Spin Tracking")]
    public TextMeshProUGUI spinCounter;
    public int maxSpins = 3;
    private int currentSpins;

    //unused events good for sounds and aniamtions and stuff later
    public event Action<ReelUI> OnSpinPressed;
    public event Action<ReelUI> OnLockToggled;
    public event Action<ReelUI> OnNudged;

    private void Start()
    {
        currentSpins = maxSpins;

        if (spinButton != null)
            spinButton.onClick.AddListener(() => TrySpin());

        if (lockButton != null)
            lockButton.onClick.AddListener(() =>
            {
                if (lockManager != null)
                    lockManager.ToggleLock(linkedReel);
                else
                    linkedReel.Lock();
                UpdateLockButtonVisual();
            });

        if (nudgeUpButton != null)
            nudgeUpButton.onClick.AddListener(() =>
            {
                if (nudgeManager != null)
                    nudgeManager.TryNudge(linkedReel, true);
                else
                    linkedReel.NudgeUp();
            });

        if (nudgeDownButton != null)
            nudgeDownButton.onClick.AddListener(() =>
            {
                if (nudgeManager != null)
                    nudgeManager.TryNudge(linkedReel, false);
                else
                    linkedReel.NudgeDown();
            });

        UpdateLockButtonVisual();
        UpdateSpinCounterText();
    }

    private void UpdateLockButtonVisual()
    {
        if (lockButtonText != null)
            lockButtonText.text = linkedReel.IsLocked ? "Unlock" : "Lock";
    }

    private void TrySpin()
    {
        if (currentSpins <= 0)
        {
            Debug.Log("No spins remaining!");
            return;
        }

        if (linkedReel != null && !linkedReel.IsLocked)
        {
            linkedReel.Spin();
        }

        currentSpins--;
        UpdateSpinCounterText();
    }

    private void UpdateSpinCounterText()
    {
        if (spinCounter != null)
        {
            spinCounter.text = $"Spins: {currentSpins}";
        }

        if (currentSpins <= 0 && spinButton != null)
        {
            spinButton.interactable = false;
        }
    }

    public void ResetSpins()
    {
        currentSpins = maxSpins;
        UpdateSpinCounterText();

        if (spinButton != null)
            spinButton.interactable = true;

        // Reset other UI states if needed
    }
}
