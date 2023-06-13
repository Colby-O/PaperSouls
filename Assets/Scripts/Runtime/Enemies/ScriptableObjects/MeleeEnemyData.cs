using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeEnemyDat", menuName = "Enemies/Melee Enemy", order = 1)]
public class MeleeEnemyData : EnemyData
{
    [Header("Damage Stats")]
    public float timeBetweenAttacks;
}
