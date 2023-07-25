using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PaperSouls.Runtime.Data
{
    [System.Serializable]
    internal sealed class SerializableRoom
    {
        public Vector3 Position;
        public Vector3 Size;
        public int NumberOfExits;
        public int ID;
        public Random.State State;

        public SerializableRoom(Vector3 roomPosition, Vector3 roomSize, int numberOfExits, int roomID, Random.State state)
        {
            Position = roomPosition;
            Size = roomSize;
            NumberOfExits = numberOfExits;
            ID = roomID;
            State = state;
        }
    }
}

