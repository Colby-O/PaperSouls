using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Enemy
{
    public abstract class MeleeEnemy : Enemy
    {
        protected MeleeEnemyData _meleeEnemyData;

        protected override void Awake()
        {
            base.Awake();
            _meleeEnemyData = (MeleeEnemyData)base.Data;
            _timeSinceLastAttack = _meleeEnemyData.timeBetweenAttacks;
        }
    }
}
