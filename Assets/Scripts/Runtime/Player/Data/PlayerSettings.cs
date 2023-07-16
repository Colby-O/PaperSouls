using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Player
{
    /// <summary>
    /// Defines the Player Settings
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Settings", order = 1)]
    internal class PlayerSettings : ScriptableObject
    {
        public float playerSpeedWalking = 0.3f;
        public float playerSpeedRunning = 0.6f;
        public float lookSpeed = 10.0f;
        public float health = 100.0f;
        public float baseXPToLevelUp = 100.0f;
        public float xpIncreasePerLevel = 50.0f;
        public GameObject healthBarPrefab;
    }
}
