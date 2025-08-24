using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioLibrary;

public class BaseSpell
{
    // Both base and runtime spell data
    public SpellSO spellData;             // Original static reference (for fallback)
    public RuntimeSpell runtimeSpell;     // Runtime override (primary if present)

    public int currentCharges;
    public int reelIndex;
    public int slotIndex;

    public bool IsDepleted => currentCharges <= 0;

    // Primary constructor for ScriptableObject version
    public BaseSpell(SpellSO so, int reel, int slot)
    {
        spellData = so;
        runtimeSpell = new RuntimeSpell(so);

        reelIndex = reel;
        slotIndex = slot;
        currentCharges = so.hasCharges ? so.charge : int.MaxValue;
    }

    // New constructor for RuntimeSpell support
    public BaseSpell(RuntimeSpell spell, int reel, int slot)
    {
        runtimeSpell = spell;
        spellData = spell.baseData;

        reelIndex = reel;
        slotIndex = slot;
        currentCharges = spell.hasCharges ? spell.charge : int.MaxValue;
    }

    // Uses runtimeSpell if present; otherwise falls back to spellData
    public void Cast(CombatManager combat, GridManager grid, bool isEnemyCaster, BaseEnemy enemyCaster = null)
    {
        var source = runtimeSpell ??= new RuntimeSpell(spellData); // fallback clone from static

        if (source == null || source.baseData == null)
            return;

        if (!string.IsNullOrEmpty(source.castSound))
        {
            AudioManager.Instance.PlaySFX(source.castSound, AudioManager.Instance.spellLibrary);
        }

        Debug.Log($"[BaseSpell] Casting {source.spellName} by {(isEnemyCaster ? "Enemy" : "Player")}");

        source.Cast(this, combat, grid, isEnemyCaster, enemyCaster);

        if (source.hasCharges)
            currentCharges--;
    }
}
