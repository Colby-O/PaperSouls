using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Player {
    /// <summary>
    /// Player Data
    /// </summary>
    internal class PlayerData : ScriptableObject
    {

        [Range(0, 99)] public int baseHealth;
        [Range(0, 99)] public int baseArmor;
        [Range(0, 99)] public int baseDamage;
        [Range(0, 99)] public int baseAgility;

    }
}
