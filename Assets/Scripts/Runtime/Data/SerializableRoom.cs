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
        public List<int> LeftExits;
        public List<int> RightExits;
        public List<int> TopExits;
        public List<int> BottomExits;


        public SerializableRoom(
            Vector3 roomPosition, 
            Vector3 roomSize, 
            int numberOfExits, 
            int roomID, 
            Random.State state,
            List<int> leftExits,
            List<int> rightExits,
            List<int> topExits,
            List<int> bottomExits
            )
        {
            Position = roomPosition;
            Size = roomSize;
            NumberOfExits = numberOfExits;
            ID = roomID;
            State = state;
            LeftExits = leftExits;
            RightExits = rightExits;
            TopExits = topExits;
            BottomExits = bottomExits;
        }

        public override string ToString()
        {
            return $"RoomID: {ID} Position: {Position} Size: {Size}";
        }
    }
}

