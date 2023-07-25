using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal enum RoomZone : int
    {
        /// <summary>
        /// Inside area of a room
        /// </summary>
        Room,
        /// <summary>
        /// Edges of a room
        /// </summary>
        Edge,
        /// <summary>
        /// Subroom Walls 
        /// </summary>
        SubRoomWall,
        /// <summary>
        /// Unavailable tile position
        /// </summary>
        Invalid
    }
}
