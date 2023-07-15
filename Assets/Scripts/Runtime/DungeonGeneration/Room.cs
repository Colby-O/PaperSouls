using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    /// <summary>
    /// Defines different areas within a room
    /// </summary>
    public enum RoomZone
    {
        Room,
        Edge,
        SubRoomWall,
        Invalid
    }

    public enum TileType : int
    {
        Empty,
        Room,
        Hallway,
        HallwayAndRoom,
        RoomSpacing,
        HallwaySpacing
    }

    [System.Serializable]
    public class TileWeights
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
    public class DungeonObject
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

        public DungeonObject() { }

        public DungeonObject(GameObject gameObject, float proability, int id)
        {
            ID = id;
            Size = GetSizeOfObject(gameObject);
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
        private Vector3 GetSizeOfObject(GameObject room)
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

        public void CalculateSize()
        {
            this.Size = GetSizeOfObject(Prefab);
        }
    }

    [System.Serializable]
    public class DecorationObject : DungeonObject
    {
        public RoomZone zone;
        public Vector3 Scale = Vector3.one;
        public int Padding;
        public List<DungeonObject> Surrounding;
        [Range(0, 1)] public float FillProbability = 1.0f;
        [Range(0, 1)] public float SurroundProbability = 1.0f;
    }

    [System.Serializable]
    public class Room : DungeonObject
    {
        public List<DungeonObject> Decorations;
        public int NumExits;
        public List<Transform> Exits;
        public Stack<Transform> AvailableExits;
        public int ExitsUsed;
        public RoomZone[,] Grid { get; private set; }

        public Room(GameObject gameObject, List<Transform> exits, int roomID) : base(gameObject, roomID)
        {
            this.Exits = new List<Transform>(exits);
            this.NumExits = exits.Count;
            this.AvailableExits = new Stack<Transform>(exits);
            ExitsUsed = 0;
        }

        public Room(int roomID) : this(null, new List<Transform>(), roomID) { }

        public void SetGrid(RoomZone[,] grid) => Grid = grid.Clone() as RoomZone[,];

        /// <summary>
        /// Sets the rooms exits
        /// </summary>
        public void SetExits(List<Transform> exits)
        {
            this.Exits = new List<Transform>(exits);
            this.NumExits = exits.Count;
            this.AvailableExits = new Stack<Transform>(exits);
        }

        public void DrawZones()
        {
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.z; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = Prefab.transform.position + new Vector3(-Size.x / 2f + 0.5f + i, 0, -Size.z / 2f + 0.5f + j);
                    if (Grid[i, j] == RoomZone.Room) cube.GetComponent<Renderer>().material.color = Color.blue;
                    else if (Grid[i, j] == RoomZone.Edge) cube.GetComponent<Renderer>().material.color = Color.red;
                    else if (Grid[i, j] == RoomZone.SubRoomWall) cube.GetComponent<Renderer>().material.color = Color.magenta;
                    else if (Grid[i, j] == RoomZone.Invalid) cube.GetComponent<Renderer>().material.color = Color.black;
                    cube.transform.parent = Prefab.transform;
                }
            }
        }
    }
    
    [System.Serializable]
    public class Hallway : DungeonObject
    {
        // NOTE: Delete if no additional infomation is need...
        public Hallway(GameObject gameObject, int id) : base(gameObject, id) { }
    }
}
