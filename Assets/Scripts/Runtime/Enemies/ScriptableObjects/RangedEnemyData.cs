using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangedEnemyData", menuName = "Enemies/Ranged Enemy", order = 2)]
public class RangedEnemyData : EnemyData
{
    [Header("Projectile Stats")]
    public GameObject projectilePrefab;
    public float fireRate;
    public float projectileSpeed;
    public float projectileLifetime;
    public List<string> tagToIgnore;
}
