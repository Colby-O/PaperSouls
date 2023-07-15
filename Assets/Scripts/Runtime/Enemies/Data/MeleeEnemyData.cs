using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Enemy
{

    [CreateAssetMenu(fileName = "MeleeEnemyDat", menuName = "Enemies/Melee Enemy", order = 1)]
    internal class MeleeEnemyData : EnemyData
    {
        [Header("Damage Stats")]
        public float timeBetweenAttacks;
    }
}
