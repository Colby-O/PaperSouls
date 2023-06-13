using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSlim : MeleeEnemy
{
    protected override void ActiveMovement()
    {
        Vector3 newPosition = enemyData.enemySpeed * Time.deltaTime * directionToPlayer;
        gameObject.transform.Translate(new Vector3(newPosition.x, 0.0f, newPosition.z));
    }

    protected override void Attack(IDamageable damageable, float dmg)
    {
        if (meleeEnemyData.timeBetweenAttacks < timeSinceLastAttack)
        {
            damageable.Damage(dmg);
            timeSinceLastAttack = 0.0f;
        }
    }

    protected override void PassiveMovement()
    {
        
    }
}
