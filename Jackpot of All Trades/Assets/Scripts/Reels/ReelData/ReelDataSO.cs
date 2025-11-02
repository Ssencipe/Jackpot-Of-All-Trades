using UnityEngine;

// Defines reel scriptable objects for slotting into enemies and players. Only defines spells currently but can be expanded for special or cursed reels later.

[CreateAssetMenu(menuName = "Reel")]
public class ReelDataSO : ScriptableObject
{
    public SpellSO[] spells;
}