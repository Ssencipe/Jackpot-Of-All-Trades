using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell
{
    public SpellSO spellData; // This is your unique scriptable object (FlameDartSO, HealingBloomSO, etc.)
    public int currentCharges;
    public int reelIndex;
    public int slotIndex;

    public BaseSpell(SpellSO so, int reel, int slot)
    {
        spellData = so;
        reelIndex = reel;
        slotIndex = slot;
        currentCharges = 1; // or so.defaultCharges
    }

    public void Cast(CombatManager combat, GridManager grid)
    {
        if (spellData is ISpellBehavior behavior)
        {
            behavior.Cast(this, combat, grid);
            currentCharges--; // You can expand this later for tally, costs, etc.
        }
    }
}
