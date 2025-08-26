using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public const int SlotsPerReel = 3;
    public const int Reels = 5;

    [Header("Grid Data")]
    public BaseSpell[,] spellGrid = new BaseSpell[Reels, SlotsPerReel];

    [Header("Linked Reels")]
    public List<Reel> linkedReels = new List<Reel>();

    private void Awake()
    {
        Instance = this;
    }

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

    public static BaseSpell GetSpellAt(int x, int y)
    {
        if (x < 0 || x >= Reels || y < 0 || y >= SlotsPerReel)
            return null;

        return Instance.spellGrid[x, y];
    }

    public static List<BaseSpell> AllSpells()
    {
        List<BaseSpell> all = new();

        for (int x = 0; x < Reels; x++)
        {
            for (int y = 0; y < SlotsPerReel; y++)
            {
                var spell = Instance.spellGrid[x, y];
                if (spell != null)
                    all.Add(spell);
            }
        }

        return all;
    }

    //For checking adjacency
    private static readonly Vector2Int[] CardinalDirs = {
    new Vector2Int(0, 1),  // Up
    new Vector2Int(1, 0),  // Right
    new Vector2Int(0, -1), // Down
    new Vector2Int(-1, 0), // Left
};

    private static readonly Vector2Int[] DiagonalDirs = {
    new Vector2Int(1, 1),   // NE
    new Vector2Int(1, -1),  // SE
    new Vector2Int(-1, -1), // SW
    new Vector2Int(-1, 1),  // NW
};

    public static List<BaseSpell> GetNeighbors(int x, int y, bool cardinalOnly = false, bool diagonalsOnly = false)
    {
        List<BaseSpell> result = new();

        var directions = new List<Vector2Int>();
        if (!diagonalsOnly) directions.AddRange(CardinalDirs);
        if (!cardinalOnly) directions.AddRange(DiagonalDirs);

        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (IsVisible(nx, ny))
            {
                var spell = GetSpellAt(nx, ny);
                if (spell != null)
                    result.Add(spell);
            }
        }

        return result;
    }

    public static List<BaseSpell> GetDirectionalNeighbors(int x, int y, Vector2Int[] directions)
    {
        List<BaseSpell> result = new();

        foreach (var dir in directions)
        {
            int nx = x + dir.x;
            int ny = y + dir.y;

            if (IsVisible(nx, ny))
            {
                var spell = GetSpellAt(nx, ny);
                if (spell != null)
                    result.Add(spell);
            }
        }

        return result;
    }

    public static bool IsVisible(int x, int y)
    {
        return x >= 0 && x < Reels && y >= 0 && y < SlotsPerReel;
    }

    public static List<BaseSpell> GetVisibleNeighbors(int x, int y, bool cardinalOnly = false, bool diagonalsOnly = false)
    {
        return GetNeighbors(x, y, cardinalOnly, diagonalsOnly);
    }

    public static List<BaseSpell> GetVisibleDirectionalNeighbors(int x, int y, Vector2Int[] directions)
    {
        return GetDirectionalNeighbors(x, y, directions);
    }

    public static bool IsMirrored(BaseSpell a, BaseSpell b)
    {
        if (a == null || b == null) return false;
        if (a.spellData != b.spellData) return false;

        int mirrorX = Reels - 1 - a.reelIndex;
        int mirrorY = SlotsPerReel - 1 - a.slotIndex;

        return b.reelIndex == mirrorX && b.slotIndex == mirrorY;
    }
}
