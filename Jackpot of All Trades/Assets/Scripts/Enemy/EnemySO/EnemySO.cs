using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Enemies/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    public int maxHealth;
    public Sprite sprite;

    public EnemyType enemyType;
    public int impactScore; // Relative "threat" value

    public List<SpellSO> spellPool; // Spells the enemy can cast (can include duplicates)
    public int reels = 1;
}
