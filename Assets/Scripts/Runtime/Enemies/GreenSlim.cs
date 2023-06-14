using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;

namespace PaperSouls.Runtime.Enemy
{
    public class GreenSlim : MeleeEnemy
    {
        protected override void ActiveMovement()
        {
            Vector3 newPosition = Data.enemySpeed * Time.deltaTime * _directionToPlayer;
            gameObject.transform.Translate(new Vector3(newPosition.x, 0.0f, newPosition.z));
        }

        protected override void PassiveMovement()
        {

        }

        protected override void Attack(IDamageable damageable, float dmg)
        {
            if (_meleeEnemyData.timeBetweenAttacks < _timeSinceLastAttack)
            {
                damageable.Damage(dmg);
                _timeSinceLastAttack = 0.0f;
            }
        }
    }
}
