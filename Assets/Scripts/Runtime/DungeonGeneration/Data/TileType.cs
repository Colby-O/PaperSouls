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
        /// Main Room Tile
        /// </summary>
        MainRoom,
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
        HallwaySpacing,
        /// <summary>
        /// Invaild Tile. Used as a proxy is 
        /// avoid main room being too close
        /// </summary>
        Invaild
    }

    [System.Serializable]
    internal sealed class TileWeights
    {
        public float EMPTY = 10;
        public float MAIN_ROOM = 5;
        public float ROOM = 5;
        public float HALLWAY = 5;
        public float HALLWAY_AND_ROOM = 5;
        public float ROOM_SPACING = 20;
        public float TURN_PENAILITY = 1;
        public float HALLWAY_SPACING = 7;
    }
}
