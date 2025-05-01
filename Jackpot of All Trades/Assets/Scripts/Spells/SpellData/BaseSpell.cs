using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell
{
    public SpellSO spellData; // Your static spell template
    public int currentCharges;
    public int reelIndex;
    public int slotIndex;

    public BaseSpell(SpellSO so, int reel, int slot)
    {
        spellData = so;
        reelIndex = reel;
        slotIndex = slot;
        currentCharges = 1;
    }

    public void Cast(CombatManager combat, GridManager grid, bool isEnemyCaster)
    {
        if (spellData is ISpellBehavior behavior)
        {
            behavior.Cast(this, combat, grid, isEnemyCaster);
            currentCharges--;
        }
    }
}

