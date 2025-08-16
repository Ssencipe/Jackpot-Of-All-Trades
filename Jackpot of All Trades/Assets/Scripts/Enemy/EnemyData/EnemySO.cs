using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpellPool
{
    public List<SpellSO> spells;
}

[CreateAssetMenu(menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Stats")]
    public int maxHealth;
    public int impactScore; //for weighing enemy spawn positions

    [Header("Reel Settings")]
    [Min(1)]
    public int reels = 1;

    [Header("Expanded Spell Pools")]
    public List<EnemySpellPool> reelSpellPools = new List<EnemySpellPool>();

    [Header("Classification")]
    public Sprite sprite;
    public EnemyType enemyType;
    public string enemyName;

    [Header("AI Targeting")]
    public EnemyTargeting allyTargeting = EnemyTargeting.Self;

    private void OnValidate()
    {
        if (reels < 1) reels = 1;

        // Auto-fill reels count based on number of spell pools (optional)
        if (reelSpellPools != null && reelSpellPools.Count > 0)
            reels = reelSpellPools.Count;
    }
}
