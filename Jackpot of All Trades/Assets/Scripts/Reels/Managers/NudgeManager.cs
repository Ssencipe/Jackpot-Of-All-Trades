using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NudgeManager : MonoBehaviour
{
    [Header("Nudge Settings")]
    public int maxNudges = 2;
    private int currentNudges;

    [Header("UI")]
    public TextMeshProUGUI nudgeText;

    [Header("Managed Reels")]
    public List<Reel> reels = new List<Reel>();

    private void Start()
    {
        ResetNudges();
    }

    public void RegisterReels(List<Reel> spawnedReels)
    {
        reels.Clear();
        reels.AddRange(spawnedReels);
    }

    public void ResetNudges()
    {
        currentNudges = maxNudges;
        UpdateNudgeText();
    }
    public bool TryNudge(Reel reel, bool up)
    {
        if (currentNudges > 0 && !reel.IsLocked)
        {
            if (up)
                reel.NudgeUp();
            else
                reel.NudgeDown();

            currentNudges--;
            UpdateNudgeText();
            return true;
        }
        return false;
    }

    private void UpdateNudgeText()
    {
        if (nudgeText != null)
            nudgeText.text = $"Nudges: {currentNudges}";
    }
}
