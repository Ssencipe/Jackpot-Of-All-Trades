using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RuntimeEnemy
{
    public EnemySO baseData;

    public int maxHealth;
    public int impactScore;
    public int reels;
    public List<RuntimeSpell> spellPool;

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
        spellPool = source.spellPool.Select(s => new RuntimeSpell(s)).ToList();

        sprite = source.sprite;
        enemyType = source.enemyType;
        enemyName = source.enemyName;
        allyTargeting = source.allyTargeting;
    }

    public void OverrideSpellPool(List<RuntimeSpell> newSpells)
    {
        spellPool = newSpells;
    }

    public void ChangePhase(EnemySO newBase)
    {
        baseData = newBase;

        maxHealth = newBase.maxHealth;
        impactScore = newBase.impactScore;
        reels = Mathf.Max(1, newBase.reels);
        spellPool = newBase.spellPool.Select(s => new RuntimeSpell(s)).ToList();
        sprite = newBase.sprite;
        enemyType = newBase.enemyType;
        enemyName = newBase.enemyName;
        allyTargeting = newBase.allyTargeting;
    }
}
