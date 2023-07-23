using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Data
{
    [System.Serializable]
    internal class GameData
    {
        // Profile ID
        public int ProfileID = -1;
        // Seed for the procedual generation
        public int Seed;

        // Player Infomation
        public Vector3 Position;
        public int CurrentLevel = -1;
        public int AmmoCount = -1;
        public float CurrentHealth = -1;
        public float CurrentXP = -1;
    }
}
