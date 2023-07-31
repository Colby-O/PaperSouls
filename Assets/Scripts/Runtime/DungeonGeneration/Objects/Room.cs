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

        public List<DungeonObject> Decorations;

        public GameObject GameObject { get; set; }
        public Vector3 Size { get; protected set; }
        public Vector3 Position { get; set; }
        public RoomZone[,] Grid {get; set;}
        public Vector2Int GridSize { get; set; } 

        public Room(Vector3 position, Vector3 size, Random.State state, int id)
        {
            Position = position;
            Size = size;
            State = state;
            ID = id;
        }

        /// <summary>
        /// Check if the room is reachable by the player
        /// </summary>
        public bool CheckIfReachable(TileType[,] grid, Vector3 tileSize)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / tileSize.x), Mathf.RoundToInt(Position.z / tileSize.z));
            Vector2Int size = new Vector2Int(Mathf.RoundToInt(Size.x / tileSize.x), Mathf.RoundToInt(Size.z / tileSize.z));

            for (int i = pos.x - Mathf.CeilToInt(size.x / 2); i < pos.x + Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = pos.y - Mathf.CeilToInt(size.y / 2); j < pos.y + Mathf.CeilToInt(size.y / 2); j++)
                {
                    if (grid[i, j] == TileType.HallwayAndRoom) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes a room from the grid
        /// </summary>
        public void RemoveFromGrid(ref TileType[,] grid, Vector3 tileSize)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / tileSize.x), Mathf.RoundToInt(Position.z / tileSize.z));
            Vector2Int size = new Vector2Int(Mathf.RoundToInt(Size.x / tileSize.x), Mathf.RoundToInt(Size.z / tileSize.z));

            for (int i = pos.x - Mathf.CeilToInt(size.x / 2); i < pos.x + Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = pos.y - Mathf.CeilToInt(size.y / 2); j < pos.y + Mathf.CeilToInt(size.y / 2); j++)
                {
                    grid[i, j] = TileType.Empty;
                }
            }
        }

        /// <summary>
        /// Find the locations of all exits i.e. room/room and room/hallway
        /// intersetions.
        /// </summary>
        public void FindExits(TileType[,] grid, Vector3 tileSize)
        {
            Vector2Int pos = new Vector2Int(Mathf.RoundToInt(Position.x / tileSize.x), Mathf.RoundToInt(Position.z / tileSize.z));
            Vector2Int size = new Vector2Int(Mathf.RoundToInt(Size.x / tileSize.x), Mathf.RoundToInt(Size.z / tileSize.z));
            Vector2Int sizeHalf = new Vector2Int(Mathf.RoundToInt(size.x / 2), Mathf.RoundToInt(size.y / 2));

            LeftExits = new();
            RightExits = new();
            TopExits = new();
            BottomExits = new();

            // TODO: Abstract this into function
            // Check Left Wall
            for (int i = pos.x - sizeHalf.x; i < pos.x + sizeHalf.x; i++)
            {
                if (
                    grid[i, pos.y - sizeHalf.y] == TileType.HallwayAndRoom &&
                    grid[i, pos.y - sizeHalf.y - 1] == TileType.Hallway
                ) LeftExits.Add(i - pos.x + sizeHalf.x);
                else if (
                    grid[i, pos.y - sizeHalf.y - 1] == TileType.HallwayAndRoom ||
                    grid[i, pos.y - sizeHalf.y - 1] == TileType.Room
                ) LeftExits.Add(-(i - pos.x + sizeHalf.x));
            }

            // Check Right Wall
            for (int i = pos.x - sizeHalf.x; i < pos.x + sizeHalf.x; i++)
            {
                if (
                    grid[i, pos.y + sizeHalf.y - 1] == TileType.HallwayAndRoom &&
                    grid[i, pos.y + sizeHalf.y] == TileType.Hallway
                ) RightExits.Add(i - pos.x + sizeHalf.x);
                else if (
                    grid[i, pos.y + sizeHalf.y] == TileType.HallwayAndRoom ||
                    grid[i, pos.y + sizeHalf.y] == TileType.Room
                ) RightExits.Add(-(i - pos.x + sizeHalf.x));
            }

            // Check Top Wall
            for (int i = pos.y - sizeHalf.y; i < pos.y + sizeHalf.y; i++)
            {
                if (
                    grid[pos.x - sizeHalf.x, i] == TileType.HallwayAndRoom &&
                    grid[pos.x - sizeHalf.x - 1, i] == TileType.Hallway
                ) TopExits.Add(i - pos.y + sizeHalf.y);
                else if (
                    grid[pos.x - sizeHalf.x - 1, i] == TileType.HallwayAndRoom ||
                    grid[pos.x - sizeHalf.x - 1, i] == TileType.Room
                ) TopExits.Add(-(i - pos.y + sizeHalf.y));
            }

            // Check Bottom Wall
            for (int i = pos.y - sizeHalf.y; i < pos.y + sizeHalf.y; i++)
            {
                if (
                    grid[pos.x + sizeHalf.x - 1, i] == TileType.HallwayAndRoom &&
                    grid[pos.x + sizeHalf.x, i] == TileType.Hallway
                ) BottomExits.Add(i - pos.y + sizeHalf.y);
                else if (
                    grid[pos.x + sizeHalf.x, i] == TileType.HallwayAndRoom ||
                    grid[pos.x + sizeHalf.x, i] == TileType.Room
                ) BottomExits.Add(-(i - pos.y + sizeHalf.y));
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
