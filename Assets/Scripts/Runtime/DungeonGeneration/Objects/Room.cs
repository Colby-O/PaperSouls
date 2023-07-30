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
        public List<int> LeftExits;
        public List<int> RightExits;
        public List<int> TopExits;
        public List<int> BottomExits;

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
        public bool AreExitsLeft() => _availableExits.Count != 0;

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

        public bool CheckIfReachable(TileType[,] grid, Vector2Int tileSize)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / tileSize.x), Mathf.RoundToInt(Position.z / tileSize.y));
            Vector2Int size = new Vector2Int(Mathf.RoundToInt(Size.x / tileSize.x), Mathf.RoundToInt(Size.z / tileSize.y));

            for (int i = pos.x - Mathf.CeilToInt(size.x / 2); i < pos.x + Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = pos.y - Mathf.CeilToInt(size.y / 2); j < pos.y + Mathf.CeilToInt(size.y / 2); j++)
                {
                    if (grid[i, j] == TileType.HallwayAndRoom) return true;
                }
            }

            return false;
        }

        public void RemoveFromGrid(ref TileType[,] grid, Vector2Int tileSize)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / tileSize.x), Mathf.RoundToInt(Position.z / tileSize.y));
            Vector2Int size = new Vector2Int(Mathf.RoundToInt(Size.x / tileSize.x), Mathf.RoundToInt(Size.z / tileSize.y));

            for (int i = pos.x - Mathf.CeilToInt(size.x / 2); i < pos.x + Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = pos.y - Mathf.CeilToInt(size.y / 2); j < pos.y + Mathf.CeilToInt(size.y / 2); j++)
                {
                    grid[i, j] = TileType.Empty;
                }
            }
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
            return new(Position, Size, NumberOfExits, ID, State, LeftExits, RightExits, TopExits, BottomExits);
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
