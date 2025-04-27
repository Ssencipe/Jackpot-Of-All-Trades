using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public List<BaseEnemy> currentEnemies = new List<BaseEnemy>();

    public BaseEnemy GetLeftmostEnemy()
    {
        return currentEnemies.Count > 0 ? currentEnemies[0] : null;
    }

    public void DealDamage(BaseEnemy target, int amount)
    {
        target?.TakeDamage(amount);
    }

    public void SpawnEnemies(List<EnemySO> encounterPool)
    {
        currentEnemies.Clear();

        for (int i = 0; i < encounterPool.Count; i++)
        {
            var enemy = new BaseEnemy(encounterPool[i], i);
            currentEnemies.Add(enemy);
        }
    }
}
