using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    [System.Serializable]
    internal sealed class Dungeon 
    {
        public int Seed;
        public SerializableGrid Grid;
        public List<SerializableRoom> RoomList;

        public Dungeon()
        {
            Seed = -1;
            Grid = new SerializableGrid();
            RoomList = new List<SerializableRoom>();
        }

        public Dungeon(List<SerializableRoom> roomList, SerializableGrid grid, int seed)
        {
            Seed = seed;
            Grid = grid;
            RoomList = roomList;
        }
    }
}
