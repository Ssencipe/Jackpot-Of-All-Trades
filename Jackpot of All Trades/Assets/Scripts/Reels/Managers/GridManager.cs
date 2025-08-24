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

            for (int y = 0; y < SlotsPerReel; y++)
            {
                RuntimeSpell runtimeSpell = reel.GetSpellAtSlot(y); // You'll need to implement this

                if (runtimeSpell != null)
                {
                    BaseSpell newInstance = new BaseSpell(runtimeSpell, x, y);
                    spellGrid[x, y] = newInstance;

                    Debug.Log($"[GridManager] Placed {newInstance.spellData.spellName} at Grid[{x},{y}]");
                }
                else
                {
                    spellGrid[x, y] = null;
                }
            }
        }
    }

    public BaseSpell[,] GetSpellGrid()
    {
        return spellGrid;
    }
}
