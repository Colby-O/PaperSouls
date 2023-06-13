using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : ScriptableObject
{

    [Range(0, 99)] public int baseHealth;
    [Range(0, 99)] public int baseArmor;
    [Range(0, 99)] public int baseDamage;
    [Range(0, 99)] public int baseAgility;

}
