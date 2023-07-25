using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    internal class DecorationObject : DungeonObject
    {
        public RoomZone zone;
        public Vector3 Scale = Vector3.one;
        public float edgeOffset = 0.0f;
        public int Padding;
        public List<DungeonObject> Surrounding;
        [Range(0, 1)] public float FillProbability = 1.0f;
        [Range(0, 1)] public float SurroundProbability = 1.0f;
    }
}
