using UnityEngine;

public class GridManager : MonoBehaviour
{
    public const int SlotsPerReel = 3;
    public const int Reels = 5;

    public BaseSpell[,] spellGrid = new BaseSpell[Reels, SlotsPerReel];
}
