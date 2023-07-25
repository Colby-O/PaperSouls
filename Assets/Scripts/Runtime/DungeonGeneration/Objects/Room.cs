using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal class Room : DungeonObject
    {
        public int NumberOfExits;
        public Random.State State;

        public List<Vector3> Exits;
        private Stack<Vector3> _availableExits;

        //public List<DungeonObject> Decorations;
        //public int RoomZone[,] Grid {get; private set;}

        public int ExitsUsed { get; set; }

        public Vector3 GetAvailableExit() => _availableExits.Pop();

        public Room(Vector3 position, Vector3 size, List<Vector3> exits, Random.State state, int id)
        {
            Exits = exits;
            _availableExits = new(Exits);
            Position = position;
            Size = size;
            State = state;
            ID = id;
        }

        public SerializableRoom ToSerializableRoom()
        {
            return new(Position, Size, NumberOfExits, ID, State);
        }

        public override string ToString()
        {
            return $"RoomID: {ID} Position: {Position} Size: {Size}";
        }
    }
}
