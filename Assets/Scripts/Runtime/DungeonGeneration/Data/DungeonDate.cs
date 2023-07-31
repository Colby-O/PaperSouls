using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    internal sealed class DungeonData
    {
        [Header("Dungeon Properties")]
        public Vector3 Scale = Vector3.one;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public int GridSize;
        [Range(0.0f, 1.0f)] public float LoopProbabilty = 0.2f;
        [Range(0.0f, 1.0f)] public float BranchingProbaility;
        [Range(0.0f, 1.0f)] public float RoomDensity;
        public bool AllowGridExtensions = true;

        [Header("Room Properties")]
        [Min(2)] public int NumberOfMainRooms;
        public Vector2Int NumberOfDummyRooms;
        public Vector2Int MainRoomSize;
        public Vector2Int RoomSize;
        [Min(0)] public int MinDistacneBetweebMainRooms;
        [Min(0)] public int MainRoomSpacing;
        [Min(0)] public int RoomSpacing;
        [Min(1)] public int BorderSpacing;

        [Header("Hallway Prefabs")]
        public GameObject StrightHallway;
        public GameObject CurvedHallway;
        public GameObject ThreeWayHallway;
        public GameObject FourWayHallway;
        public GameObject EnterenceHallway;

        [Header("Generation Properties")]
        public TileWeights Weights;
    }
}
