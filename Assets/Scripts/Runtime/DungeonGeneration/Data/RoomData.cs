using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    internal sealed class RoomData
    {
        [Min(1)] public int minSubRoomSize;
        [Min(1)] public int maxSubRoomSize;
        public Vector2Int MinSubRoomSize { 
            get 
            { 
                return new Vector2Int(minSubRoomSize, maxSubRoomSize); 
            }
            set 
            { 
                minSubRoomSize = value.x; 
                maxSubRoomSize = value.y; 
            } 
        }

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
