using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Stats")]
    public int maxHealth;
    public int impactScore; //for weighting certain enemies to spawn in certain positions later

    [Header("Reel Settings")]
    [Min(1)]
    public int reels = 1;

    [Header("Spells")]
    public List<SpellSO> spellPool;

    [Header("Classification")]
    public Sprite sprite;
    public EnemyType enemyType;
    public string enemyName;

    [Header("AI Targeting")]
    public EnemyTargeting allyTargeting = EnemyTargeting.Self;

    private void OnValidate()
    {
        if (reels < 1) reels = 1;
    }
}
