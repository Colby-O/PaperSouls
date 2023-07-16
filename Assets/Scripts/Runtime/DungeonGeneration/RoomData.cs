using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [CreateAssetMenu(fileName = "RoomData", menuName = "Dungeon/Room")]
    internal class RoomData : ScriptableObject
    {
        [Min(1)] public Vector2Int minSubRoomSize;
        [Range(0, 1)] public float proabilityForSplit = 0.5f;
        public bool useRandomFloorRotation = true;

        public List<Recipe> recipes;

        public List<DungeonObject> wallObjects;
        public List<DungeonObject> enterenceObjects;
        public List<DungeonObject> floorObjects;
        public List<DungeonObject> pillarObjects;

        public List<DungeonObject> SubRoomWallObjects;
        public List<DungeonObject> SubRoomEnterenceObjects;
    }
}
