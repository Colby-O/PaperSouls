using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangedEnemy : Enemy
{
    protected RangedEnemyData rangedEnemyData;

    protected virtual void SpawnProjectile(Ray path)
    {
        GameObject projectileObj = GameObject.Instantiate(rangedEnemyData.projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.damage = rangedEnemyData.attackDamage;
        projectile.speed = rangedEnemyData.projectileSpeed;
        projectile.AddTagsToIgnore(rangedEnemyData.tagToIgnore);
        projectile.AddTagsToIgnore(transform.tag);
        projectile.SetRay(path);
    }

    protected override void Awake()
    {
        base.Awake();
        rangedEnemyData = (RangedEnemyData)base.enemyData;
        timeSinceLastAttack = rangedEnemyData.fireRate;
    }
}
