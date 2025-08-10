using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReelSpawner : MonoBehaviour
{
    [Header("Reels")]
    public List<ReelDataSO> reelConfigs;

    [Header("Reel Settings")]
    public GameObject reelPrefab;
    public int numberOfReels = 5;
    public float spacing = 50f;

    [Header("References")]
    public LockManager lockManager;
    public NudgeManager nudgeManager;
    public GridManager gridManager;
    public ReelUI reelUI;

    [Header("UI References")]
    public Button spinButton;
    public TextMeshProUGUI spinCounter;

    [Header("Runtime Spawn Data")]
    private List<Reel> spawnedReels = new List<Reel>();
    private Transform reelHolderTransform;

    public void SetPlayerReference(GameObject playerGO)
    {
        if (playerGO == null)
        {
            Debug.LogWarning("SetPlayerReference called with null.");
            return;
        }

        reelHolderTransform = playerGO.transform.Find("ReelHolder");

        if (reelHolderTransform == null)
        {
            Debug.LogError("ReelHolder not found in Player prefab.");
            return;
        }

        Debug.Log("ReelHolder successfully linked.");
        SpawnReels();
    }
    private void SpawnReels()
    {
        if (reelPrefab == null || reelHolderTransform == null)
        {
            Debug.LogError("Cannot spawn reels: Missing reelPrefab or reelHolderTransform.");
            return;
        }

        float totalWidth = (numberOfReels - 1) * spacing;
        float startX = -totalWidth / 2f;

        spawnedReels.Clear();

        //add components that are outside of the prefabs

        for (int i = 0; i < numberOfReels; i++)
        {
            GameObject reelGO = Instantiate(reelPrefab, reelHolderTransform);
            RectTransform rect = reelGO.GetComponent<RectTransform>();

            if (rect != null)
                rect.anchoredPosition = new Vector2(startX + i * spacing, 0f);

            Reel reelScript = reelGO.GetComponentInChildren<Reel>();
            if (reelScript != null)
            {
                spawnedReels.Add(reelScript);
                reelScript.gameObject.name = $"Reel_{i}";

                //assigns individual reel data from ReelSOs
                if (i < reelConfigs.Count)
                {
                    var runtimeReel = new RuntimeReel(reelConfigs[i]);  // Convert SO to runtime
                    reelScript.runtimeReel = runtimeReel; // Assign unique ReelDataSO
                }
                else
                {
                    Debug.LogWarning($"No ReelDataSO defined for Reel {i}");
                }
            }

            ReelVisual visual = reelGO.GetComponentInChildren<ReelVisual>();
            if (visual != null && reelScript != null)
                reelScript.reelVisual = visual;

            ReelUI ui = reelGO.GetComponentInChildren<ReelUI>();
            if (ui != null)
            {
                ui.lockManager = lockManager;
                ui.nudgeManager = nudgeManager;
                ui.spinButton = spinButton;
                ui.spinCounter = spinCounter;
            }

            ReelClickRegion clickRegion = reelGO.GetComponentInChildren<ReelClickRegion>();
            if (clickRegion != null)
            {
                clickRegion.SetLockManager(lockManager);
                clickRegion.SetNudgeManager(nudgeManager);
                clickRegion.SetReel(reelScript); // Add this method to cleanly assign the reel reference
            }
        }

        lockManager?.RegisterReels(spawnedReels);
        nudgeManager?.RegisterReels(spawnedReels);
        gridManager?.RegisterReels(spawnedReels);
    }
    public void ResetReels()
    {
        foreach (Reel reel in spawnedReels)
        {
            reel.Unlock();
            reel.RandomizeStart();
        }

        // Reset UI state via ReelUI
        foreach (ReelUI ui in FindObjectsOfType<ReelUI>())
        {
            ui.ResetSpins();
        }

        lockManager?.ResetLocks();
        nudgeManager?.ResetNudges();
    }
}
