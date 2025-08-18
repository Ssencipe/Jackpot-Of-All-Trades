using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    private void Start()
    {
        currentSpins = maxSpins;

        if (spinButton != null)
            spinButton.onClick.AddListener(() => TrySpin());

        UpdateSpinCounterText();
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
        currentSpins = maxSpins;
        UpdateSpinCounterText();

        if (spinButton != null)
            spinButton.interactable = true;
    }
}
