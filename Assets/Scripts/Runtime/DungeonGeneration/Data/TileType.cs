using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal enum TileType : int
    {
        /// <summary>
        /// Empty Tile Slot
        /// </summary>
        Empty,
        /// <summary>
        /// Room Tile
        /// </summary>
        Room,
        /// <summary>
        /// Hallway Tile
        /// </summary>
        Hallway,
        /// <summary>
        /// Hallway and Room Tile Overlap
        /// </summary>
        HallwayAndRoom,
        /// <summary>
        /// Tile Next To Room
        /// </summary>
        RoomSpacing,
        /// <summary>
        /// Tile Next To Hallway
        /// </summary>
        HallwaySpacing
    }

    [System.Serializable]
    internal sealed class TileWeights
    {
        private const int INF = 1000000;
        public float EMPTY = 10;
        public float ROOM = INF;
        public float HALLWAY = 5;
        public float HALLWAY_AND_ROOM = INF;
        public float ROOM_SPACING = 20;
        public float TURN_PENAILITY = 3;
        public float HALLWAY_SPACING = 7;
    }
}
