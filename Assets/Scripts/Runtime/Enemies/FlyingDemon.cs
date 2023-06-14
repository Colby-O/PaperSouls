using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;

namespace PaperSouls.Runtime.Enemy
{
    public class FlyingDemon : RangedEnemy
    {
        protected override void ActiveMovement()
        {
            Vector3 newPosition = Data.enemySpeed * Time.deltaTime * _directionToPlayer;
            gameObject.transform.Translate(newPosition);
        }

        protected override void PassiveMovement()
        {

        }

        protected override void Attack(IDamageable damageable, float dmg)
        {
            if (_rangedEnemyData.fireRate < _timeSinceLastAttack)
            {
                _timeSinceLastAttack = 0.0f;
                SpawnProjectile(_visonRay);
            }
        }
    }
}
