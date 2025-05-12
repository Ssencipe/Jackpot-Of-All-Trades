using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class ClickableEnemy : MonoBehaviour
{
    private BaseEnemy enemyData;

    public void SetData(BaseEnemy data)
    {
        enemyData = data;
    }

    void OnMouseDown()
    {
        if (!BattleDirector.Instance.IsPlayerTurn || enemyData == null || enemyData.IsDead)
            return;

        TargetingOverride.SetOverrideTarget(enemyData);
        Debug.Log($"Player selected {enemyData.baseData.enemyName} as target.");
    }
}

