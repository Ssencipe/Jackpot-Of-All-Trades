using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell
{
    public SpellSO spellData; // Your static spell template
    public int currentCharges;
    public int reelIndex;
    public int slotIndex;
    public bool IsDepleted => currentCharges <= 0;


    public BaseSpell(SpellSO so, int reel, int slot)
    {
        spellData = so;
        reelIndex = reel;
        slotIndex = slot;
        currentCharges = 1;
    }

    //cast the spell and reduce charges if possible
    public void Cast(CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        if (spellData is ISpellBehavior behavior)
        {
            Debug.Log($"[BaseSpell] Casting {spellData.spellName} by {(isEnemyCaster ? "Enemy" : "Player")}");
            behavior.Cast(this, combat, grid, isEnemyCaster, enemyCaster);
            currentCharges--;
        }
    }
}

