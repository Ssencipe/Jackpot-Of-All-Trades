using System.Collections.Generic;
using UnityEngine;

public abstract class SpellSO : ScriptableObject
{
    public string spellName;
    public string description;
    public ColorType colorType;
    public List<SpellTag> tags;
    public Sprite icon;
}