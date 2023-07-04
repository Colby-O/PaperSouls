using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [CreateAssetMenu(fileName = "DungeonProperties", menuName = "Dungeon/Properties")]
    public class DungeonProperties : ScriptableObject
    {
        [Header("Dungeon Properties")]
        public Vector3 Scale = Vector3.one;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public int GridSize;
        public float LoopProabilty = 0.2f;

        [Header("Room Properties")]
        public Vector2Int RoomSize;
        public Vector2Int NumberOfRooms;
        public Vector2Int NumberOfExits;
        public int RoomSpacing;

        [Header("Hallway Prefabs")]
        public GameObject StrightHallway;
        public GameObject CurvedHallway;
        public GameObject ThreeWayHallway;
        public GameObject FourWayHallway;
        public GameObject EnterenceHallway;

        [Header("Generation Properties")]
        public TileWeights Weights;
        public bool UseShortestPath = true;
        public bool UseRandomRoomSizes = false;
        public bool AllowGridExtensions = true;
        public int GridExtension;
        public int MaxNumberOfRoomPlacementTries;
    }
}