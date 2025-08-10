using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public const int SlotsPerReel = 3;
    public const int Reels = 5;

    [Header("Grid Data")]
    public BaseSpell[,] spellGrid = new BaseSpell[Reels, SlotsPerReel];

    [Header("Linked Reels")]
    public List<Reel> linkedReels = new List<Reel>();

    public void RegisterReels(List<Reel> reels)
    {
        linkedReels.Clear();
        linkedReels.AddRange(reels);
    }

    public void PopulateGridFromSpin()
    {
        if (linkedReels.Count != Reels)
        {
            Debug.LogError("[GridManager] Incorrect number of reels linked!");
            return;
        }

        for (int x = 0; x < linkedReels.Count; x++)
        {
            Reel reel = linkedReels[x];
            RuntimeSpell spell = reel.GetCenterSpell();

            if (spell != null)
            {
                BaseSpell newInstance = new BaseSpell(spell.baseData, x, 1);
                spellGrid[x, 1] = newInstance;

                Debug.Log($"[GridManager] Placed {newInstance.spellData.spellName} at Grid[{x},1]");
            }
            else
            {
                Debug.LogWarning($"[GridManager] No SpellSO found at center of reel {x}.");
                spellGrid[x, 1] = null;
            }

            // Top and bottom rows can be expanded later
            spellGrid[x, 0] = null;
            spellGrid[x, 2] = null;
        }
    }

    public BaseSpell[,] GetSpellGrid()
    {
        return spellGrid;
    }
}
