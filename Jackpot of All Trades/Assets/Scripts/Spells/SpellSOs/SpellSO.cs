using System.Collections.Generic;
using UnityEngine;

public abstract class SpellSO : ScriptableObject
{
    [Header("Visuals")]
    [TextArea] public string description;
    public Sprite icon;

    [Header("Classification")]
    public ColorType colorType;
    public List<SpellTag> tags;
    public string spellName;

    [Header("Stats")]
    public int charge;
    public TargetingMode targetingMode;
}