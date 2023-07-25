using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal class Room
    {
        public int ID;
        public int NumberOfExits;
        public Random.State State;
        public List<Vector3> Exits;
        public List<DungeonObject> Decorations;

        private Stack<Vector3> _availableExits;

        public GameObject GameObject { get; set; }
        public Vector3 Size { get; protected set; }
        public Vector3 Position { get; set; }
        public RoomZone[,] Grid {get; set;}
        public Vector2Int GridSize { get; set; } 
        public int ExitsUsed { get; set; }

        public Vector3 GetAvailableExit() => _availableExits.Pop();

        public Room(Vector3 position, Vector3 size, List<Vector3> exits, Random.State state, int id)
        {
            Exits = exits;
            NumberOfExits = Exits.Count;
            ExitsUsed = 0;
            _availableExits = new(Exits);
            Position = position;
            Size = size;
            State = state;
            ID = id;
        }

        public void DrawGrid()
        {
            for (int i = 0; i < GridSize.x; i++)
            {
                for (int j = 0; j < GridSize.y; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = Position + new Vector3(-GridSize.x / 2f + 0.5f + i, 0, -GridSize.y / 2f + 0.5f + j);
                    if (Grid[i, j] == RoomZone.Room) cube.GetComponent<Renderer>().material.color = UnityEngine.Color.blue;
                    else if (Grid[i, j] == RoomZone.Edge) cube.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
                    else if (Grid[i, j] == RoomZone.SubRoomWall) cube.GetComponent<Renderer>().material.color = UnityEngine.Color.magenta;
                    else if (Grid[i, j] == RoomZone.Invalid) cube.GetComponent<Renderer>().material.color = UnityEngine.Color.black;
                }
            }
        }

        public SerializableRoom ToSerializableRoom()
        {
            return new(Position, Size, NumberOfExits, ID, State);
        }

        public override string ToString()
        {
            return $"RoomID: {ID} Position: {Position} Size: {Size}";
        }

        /// <summary>
        /// Manually set the size of the object.
        /// </summary>
        public void SetSize(Vector3 size)
        {
            this.Size = size;
        }
    }
}
