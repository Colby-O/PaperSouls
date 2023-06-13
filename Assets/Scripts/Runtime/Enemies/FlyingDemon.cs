using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : RangedEnemy
{
    protected override void ActiveMovement()
    {
        Vector3 newPosition = enemyData.enemySpeed * Time.deltaTime * directionToPlayer;
        gameObject.transform.Translate(newPosition);
    }

    protected override void PassiveMovement()
    {
        
    }

    protected override void Attack(IDamageable damageable, float dmg)
    {
        if (rangedEnemyData.fireRate < timeSinceLastAttack)
        {
            timeSinceLastAttack = 0.0f;
            SpawnProjectile(visonRay);
        }
    }
}
