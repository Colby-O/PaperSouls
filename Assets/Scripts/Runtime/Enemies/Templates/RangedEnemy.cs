using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Weapons;

namespace PaperSouls.Runtime.Enemy
{
    public abstract class RangedEnemy : Enemy
    {
        protected RangedEnemyData _rangedEnemyData;

        protected virtual void SpawnProjectile(Ray path)
        {
            GameObject projectileObj = GameObject.Instantiate(_rangedEnemyData.projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Damage = _rangedEnemyData.attackDamage;
            projectile.Speed = _rangedEnemyData.projectileSpeed;
            projectile.AddTagsToIgnore(_rangedEnemyData.tagToIgnore);
            projectile.AddTagsToIgnore(transform.tag);
            projectile.SetRay(path);
        }

        protected override void Awake()
        {
            base.Awake();
            _rangedEnemyData = (RangedEnemyData)base.Data;
            _timeSinceLastAttack = _rangedEnemyData.fireRate;
        }
    }
}
