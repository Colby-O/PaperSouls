using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.Enemy
{
    public abstract class EnemyData : ScriptableObject
    {
        [Header("Health Stats")]
        public float health;
        public GameObject healthBarPrefab;
        [Header("Leveling")]
        public float xpGain = 50;
        [Header("Damage Stats")]
        public float attackDamage;
        [Header("Movement")]
        public float enemySpeed;
        public float lookSpeed;
        public float minDistToObject;
        [Header("Material/Sprites")]
        public Material defaultMat;
        public Material deathMat;
        [Header("Loot")]
        public bool dropLoot = true;
        public LootTable lootTable;
        public int maxNumberOfDrops = 3;
        public float dropRadius = 1f;
        [Header("Other")]
        public float memoryTime;
    }
}
