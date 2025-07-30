using System.Collections.Generic;
using System.Linq;

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