using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeEnemy : Enemy
{
    protected MeleeEnemyData meleeEnemyData;

    protected override void Awake()
    {
        base.Awake();
        meleeEnemyData = (MeleeEnemyData)base.enemyData;
        timeSinceLastAttack = meleeEnemyData.timeBetweenAttacks;
    }
}
