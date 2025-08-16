using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeEnemy
{
    public EnemySO baseData;

    public int maxHealth;
    public int impactScore;
    public int reels;

    public List<List<RuntimeSpell>> runtimeReelSpellPools;

    public Sprite sprite;
    public EnemyType enemyType;
    public string enemyName;

    public EnemyTargeting allyTargeting;

    public RuntimeEnemy(EnemySO source)
    {
        baseData = source;

        maxHealth = source.maxHealth;
        impactScore = source.impactScore;
        reels = Mathf.Max(1, source.reels);

        runtimeReelSpellPools = new List<List<RuntimeSpell>>();

        if (source.reelSpellPools != null && source.reelSpellPools.Count > 0)
        {
            foreach (var pool in source.reelSpellPools)
            {
                var runtimeSpells = pool.spells.Select(s => new RuntimeSpell(s)).ToList();
                runtimeReelSpellPools.Add(runtimeSpells);
            }
        }

        sprite = source.sprite;
        enemyType = source.enemyType;
        enemyName = source.enemyName;
        allyTargeting = source.allyTargeting;
    }

    public void OverrideSpellPool(List<RuntimeSpell> newSpells)
    {
        runtimeReelSpellPools = new List<List<RuntimeSpell>> { newSpells };
    }

    public void ChangePhase(EnemySO newBase)
    {
        baseData = newBase;

        maxHealth = newBase.maxHealth;
        impactScore = newBase.impactScore;
        reels = Mathf.Max(1, newBase.reels);

        runtimeReelSpellPools.Clear();
        foreach (var pool in newBase.reelSpellPools)
        {
            var runtimeSpells = pool.spells.Select(s => new RuntimeSpell(s)).ToList();
            runtimeReelSpellPools.Add(runtimeSpells);
        }

        sprite = newBase.sprite;
        enemyType = newBase.enemyType;
        enemyName = newBase.enemyName;
        allyTargeting = newBase.allyTargeting;
    }
}
