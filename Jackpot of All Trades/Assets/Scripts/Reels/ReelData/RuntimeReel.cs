using System.Collections.Generic;
using System.Linq;

// Runtime instance of reel SOs since scriptable objects are immutable and some spells may be changed for the duration of a turn (movers), battle (transformed for rest of battle), or potentially even a playthrough (transforms or core stats change like Inscryption Ourobors).

public class RuntimeReel
{
    public List<RuntimeSpell> spells;

    public RuntimeReel(ReelDataSO source)
    {
        spells = source.spells.Select(spell => new RuntimeSpell(spell)).ToList();
    }

    public void ReplaceSpellAt(int index, RuntimeSpell newSpell)
    {
        if (index >= 0 && index < spells.Count)
            spells[index] = newSpell;
    }

    //bridge systems expectings SpellSO[]
    public SpellSO[] ToSpellArray()
    {
        return spells.Select(s => s.baseData).ToArray();
    }
}