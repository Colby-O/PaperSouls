using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal enum TileType : int
    {
        Empty,
        Room,
        Hallway,
        HallwayAndRoom,
        RoomSpacing,
        HallwaySpacing
    }

    [System.Serializable]
    internal sealed class TileWeights
    {
        const int INF = 1000000;
        public float EMPTY = 10;
        public float ROOM = INF;
        public float HALLWAY = 5;
        public float HALLWAY_AND_ROOM = INF;
        public float ROOM_SPACING = 20;
        public float TURN_PENAILITY = 3;
        public float HALLWAY_SPACING = 7;
    }

    [System.Serializable]
    internal class DungeonObject
    {
        public int ID;
        public float Proability = 1.0f;

        public Vector3 Size { get; protected set; }

        [SerializeField] private GameObject _prefab;

        public GameObject Prefab 
        { 
            get { return _prefab; } 
            protected set { _prefab = value; } 
        }

        public DungeonObject(GameObject gameObject, float proability, int id)
        {
            ID = id;
            Size = GetSizeOfRoom(gameObject);
            Prefab = gameObject;
            Proability = proability;
        }

        public DungeonObject(GameObject gameObject, int id) : this(gameObject, 1.0f, id) { }

        public DungeonObject(int id) : this(null, id) { }

        /// <summary>
        /// Sets the spawn proability 
        /// </summary>
        public void SetProability(float proability)
        {
            this.Proability = proability;
        }

        /// <summary>
        /// Gets the size of a room in world space 
        /// </summary>
        private Vector3 GetSizeOfRoom(GameObject room)
        {
            if (room == null) return Vector3.zero;

            Vector3 center = Vector3.zero;
            int childCount = 0;

            Renderer renderer = room.transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                center += renderer.bounds.center;
                childCount += 1;
            }

            foreach (Transform child in room.transform)
            {
                renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    center += renderer.bounds.center;
                    childCount += 1;
                }
            }

            center /= childCount;


            Bounds bounds = new(center, Vector3.zero);

            renderer = room.transform.GetComponent<Renderer>();
            if (renderer != null)
            {
                bounds.Encapsulate(renderer.bounds);
            }

            foreach (Transform child in room.transform)
            {
                renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return bounds.size;
        }

        public void SetGameObject(GameObject gameObject)
        {
            this.Prefab = gameObject;
        }

        /// <summary>
        /// Sets the size of the room
        /// </summary>
        public void SetSize(Vector3 size)
        {
            this.Size = size;
        }
    }
    
    [System.Serializable]
    internal class Room : DungeonObject
    {
        public int NumExits;
        public List<Transform> Exits;
        public Stack<Transform> AvailableExits;
        public int ExitsUsed;

        public Room(GameObject gameObject, List<Transform> exits, int roomID) : base(gameObject, roomID)
        {
            this.Exits = new List<Transform>(exits);
            this.NumExits = exits.Count;
            this.AvailableExits = new Stack<Transform>(exits);
            ExitsUsed = 0;
        }

        public Room(int roomID) : this(null, new List<Transform>(), roomID) { }

        /// <summary>
        /// Sets the rooms exits
        /// </summary>
        public void SetExits(List<Transform> exits)
        {
            this.Exits = new List<Transform>(exits);
            this.NumExits = exits.Count;
            this.AvailableExits = new Stack<Transform>(exits);
        }
    }
    
    [System.Serializable]
    internal class Hallway : DungeonObject
    {
        // NOTE: Delete if no additional infomation is need...
        public Hallway(GameObject gameObject, int id) : base(gameObject, id) { }
    }
}
