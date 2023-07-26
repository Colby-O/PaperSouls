using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Helpers;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal sealed class DungeonGenerator
    {
        // Constants
        public const int GridExtensionAmount = 10;
        public const int MaxNumberOfRoomPlacementTries = 100;

        // Properties
        private int _seed;
        private DungeonData _dungeonData;
        private Vector3 _tileSize;

        // Dungeon Components
        private List<Room> _roomList;
        private TileType[,] _grid;
        private int _gridSize;

        // Helper Classes
        private DelaunayTriangulation _triangulation;
        private MST _mst;

        private Dictionary<int, List<int>> _edgeList;
        private List<Dictionary<Vector2Int, Vector2Int>> _paths;
        private List<Vector2Int> _pathStarts;
        private List<Vector2Int> _pathEnds;

        // Generators
        private RoomGenerator _roomGenerator;

        public DungeonGenerator(int seed, DungeonData properties, Vector3 tileSize)
        {
            _roomGenerator = new(properties.RoomData, seed, new((int)tileSize.x, (int)tileSize.z));
            _roomList = new();
            _paths = new();
            _pathStarts = new();
            _pathEnds = new();

            _seed = seed;
            _dungeonData = properties;
            _tileSize = tileSize;

            Random.InitState(_seed);

            _gridSize = _dungeonData.DungeonProperties.GridSize;
            _grid = new TileType[_gridSize, _gridSize];

            for (int j = 0; j < _gridSize; j++)
            {
                for (int k = 0; k < _gridSize; k++)
                {
                    _grid[j, k] = TileType.Empty;
                }
            }
        }

        /// <summary>
        /// Create a new room given a prefab, position, scale, and id
        /// </summary>
        void CreateNewRoom(Vector2Int roomPosition, Vector2Int roomSize, Vector2Int numberOfExits, int roomID)
        {
            Vector3 position = new(roomPosition.x * _tileSize.x, 0.0f, roomPosition.y * _tileSize.z);
            Vector3 size = new(roomSize.x * _tileSize.x, 0, roomSize.y * _tileSize.z);
            int numExits = Random.Range(numberOfExits.x, numberOfExits.y + 1);
            _roomList.Add(_roomGenerator.GenerateRoomObject(position, size, numExits, roomID));
        }

        /// <summary>
        /// Adds a room to the grid.
        /// </summary>
        bool UpdateRoomGrid(Vector2Int roomSize, Vector2Int roomPosition, int minRoomSpacing)
        {
            TileType[,] gridTemp = _grid.Clone() as TileType[,];

            bool vaildRoom = true;
            for (int j = -(roomSize.x + minRoomSpacing) / 2; j < (roomSize.x + minRoomSpacing) / 2; j++)
            {
                for (int k = -(roomSize.y + minRoomSpacing) / 2; k < (roomSize.y + minRoomSpacing) / 2; k++)
                {
                    if (
                        roomPosition.x + j >= _gridSize || 
                        roomPosition.x + j < 0 || 
                        roomPosition.y + k >= _gridSize || 
                        roomPosition.y + k < 0
                    )
                    {
                        vaildRoom = false;
                        break;
                    }
                    if (
                        (j < -roomSize.x / 2 || j >= roomSize.x / 2 || k < -roomSize.y / 2 || k >= roomSize.y / 2)
                        && _grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty
                    )
                    {
                        gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.RoomSpacing;
                    }
                    else if (_grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty)
                    {
                        gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.Room;
                    }
                    else
                    {
                        vaildRoom = false;
                        break;
                    }
                }

                if (!vaildRoom) break;
            }

            if (vaildRoom) _grid = gridTemp.Clone() as TileType[,];

            return vaildRoom;
        }

        /// <summary>
        /// Extends the size of the grid
        /// </summary>
        void ExtendGridSize()
        {
            TileType[,] oldGrid = _grid.Clone() as TileType[,];
            int oldGridSize = _gridSize;
            _gridSize += GridExtensionAmount;
            _grid = new TileType[_gridSize, _gridSize];

            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < oldGridSize; j++)
                {
                    _grid[i, j] = TileType.Empty;
                }
            }

            for (int i = 0; i < oldGridSize; i++)
            {
                for (int j = 0; j < oldGridSize; j++)
                {
                    _grid[i, j] = oldGrid[i, j];
                }
            }
        }

        /// <summary>
        /// Gets a random room size that is vaild. Only odd room sizes are vaild.
        /// </summary>
        private Vector2Int GetVaildRoomSize()
        {
            Vector2Int roomSize = new(
                RandomGenerator.GetRandomSkewed(_dungeonData.DungeonProperties.RoomSize.x, _dungeonData.DungeonProperties.RoomSize.y),
                RandomGenerator.GetRandomSkewed(_dungeonData.DungeonProperties.RoomSize.x, _dungeonData.DungeonProperties.RoomSize.y)
                );
            
            // Ensures the roomSize is an odd number (I don't know why this matters but it does)
            if (roomSize.x % 2 == 0) roomSize.x += 1;
            if (roomSize.y % 2 == 0) roomSize.y += 1;

            return roomSize;
        }

        /// <summary>
        /// Generate vaild random position in the grid
        /// </summary>
        private bool GenerateRandomPosition(out Vector2Int roomPosition, out Vector2Int roomSize)
        {
            int numberOfPlacementTries = 0;
            bool foundVaildRoom = false;
            do
            {
                roomPosition = new(
                    Random.Range(10, _gridSize - 10), 
                    Random.Range(10, _gridSize - 10)
                    );

                roomSize = GetVaildRoomSize();

                numberOfPlacementTries += 1;
                if (MaxNumberOfRoomPlacementTries < numberOfPlacementTries)
                {
                    if (!_dungeonData.DungeonProperties.AllowGridExtensions) break;
                    ExtendGridSize();
                    numberOfPlacementTries = 0;
                }

                foundVaildRoom = UpdateRoomGrid(roomSize, roomPosition, _dungeonData.DungeonProperties.RoomSpacing);
            } while (!foundVaildRoom);

            return foundVaildRoom;
        }

        /// <summary>
        /// Get A list of room center coordinates
        /// </summary>
        private List<Vector2> GetVerticeList()
        {
            List<Vector2> vertices = new();

            foreach (Room room in _roomList)
            {
                vertices.Add(new Vector2(room.Position.x, room.Position.z));
            }

            return vertices;
        }

        /// <summary>
        /// Replace a room.
        /// Called when the original room has too little exits
        /// </summary>
        private void ReplaceRoom(int roomID, int numberOfExitsNeeded)
        {
            int usedExits = _roomList[roomID].ExitsUsed;
            Room newRoom = _roomGenerator.GenerateRoomObject(_roomList[roomID].Position, _roomList[roomID].Size, numberOfExitsNeeded, _roomList[roomID].ID);
            newRoom.ExitsUsed = usedExits;
            _roomList[roomID] = newRoom;
        }

        /// <summary>
        /// Finds the number of used exits on a room
        /// </summary>
        private void UpdateNumberOfUsedExits()
        {
            foreach (int vertex in _edgeList.Keys)
            {
                foreach (int adjVertex in _edgeList[vertex])
                {
                    _roomList[vertex].ExitsUsed += 1;
                    _roomList[adjVertex].ExitsUsed += 1;
                }
            }
        }

        /// <summary>
        /// Makes sure all room have enough exits 
        /// </summary>
        private void CheckForRoomsWithInvaildNumberOfEdages()
        {
            for (int v = 0; v < _roomList.Count; v++)
            {
                if (_roomList[v].ExitsUsed > _roomList[v].Exits.Count) ReplaceRoom(v, _roomList[v].ExitsUsed + 1);
            }
        }

        /// <summary>
        /// Converts world coordinates into grid space cooridnates 
        /// </summary>
        private Vector2Int GetRoomdPosition(Vector3 start)
        {
            return new Vector2Int(Mathf.RoundToInt((start.x / _tileSize.x)), Mathf.RoundToInt((start.z / _tileSize.z)));
        }

        /// <summary>
        /// Add hallway to the grid
        /// </summary>
        private void AddHallwayTileToGrid(Vector2Int gridPos)
        {
            _grid[gridPos.x, gridPos.y] = (_grid[gridPos.x, gridPos.y] == TileType.Room) ? TileType.HallwayAndRoom : TileType.Hallway;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0) continue;
                    else if (_grid[gridPos.x + i, gridPos.y + j] == TileType.Empty) _grid[gridPos.x + i, gridPos.y + j] = TileType.HallwaySpacing;
                }
            }
        }

        /// <summary>
        /// Place a allway along a path
        /// </summary>
        private void AddHallwayToGrid(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start)
        {
            Vector2Int gridPT = start;
            while (true)
            {
                AddHallwayTileToGrid(gridPT);

                if (!cameFrom.ContainsKey(gridPT)) break;
                else gridPT = cameFrom[gridPT];
            }
        }

        /// <summary>
        /// Generates a path between rooms using A*
        /// </summary>
        private void ConstructHallwayBetween(int roomIDA, int roomIDB)
        {
            Vector3 startExit = _roomList[roomIDA].GetAvailableExit();
            Vector3 endExit = _roomList[roomIDB].GetAvailableExit();

            Vector2Int start = GetRoomdPosition(startExit);
            Vector2Int end = GetRoomdPosition(endExit);

            PathFinder pathFinder = new(_grid, _dungeonData.DungeonProperties.Weights);
            Dictionary<Vector2Int, Vector2Int> path = pathFinder.FindOptimalPath(start, end);

            _pathStarts.Add(start);
            _pathEnds.Add(end);
            _paths.Add(path);

            AddHallwayToGrid(path, end);
        }

        /// <summary>
        /// Places a terminal room
        /// </summary>
        private bool AddTerminalRoom(int roomID)
        {
            if (GenerateRandomPosition(out Vector2Int roomPosition, out Vector2Int roomSize))
            {
                int newRoomID = _roomList.Count;

                CreateNewRoom(roomPosition, roomSize, new(1, 1), newRoomID);
                ConstructHallwayBetween(roomID, newRoomID);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Generates room data
        /// </summary>
        private void PlaceRooms()
        {
            int numberOfRooms = Random.Range(
                _dungeonData.DungeonProperties.NumberOfRooms.x, 
                _dungeonData.DungeonProperties.NumberOfRooms.y + 1
                );

            for (int i = 0; i < numberOfRooms; i++)
            {
                if (GenerateRandomPosition(out Vector2Int roomPosition, out Vector2Int roomSize))
                {
                    CreateNewRoom(roomPosition, roomSize, _dungeonData.DungeonProperties.NumberOfExits, i);
                }
            }
        }

        /// <summary>
        /// Determines which room are connected
        /// </summary>
        private void ConstructLayout()
        {
            List<Vector2> vertices = GetVerticeList();

            // Step 1: Computes the Delaunay Triangulation between the Rooms
            _triangulation = new(vertices);
            float[,] adjacencyMatrix = _triangulation.CalculateDelaunayTriangulation();

            // Step 2: Constructs a Minimum Spanning Tree 
            _mst = new(adjacencyMatrix, vertices);
            _edgeList = _mst.GetMST();
            UpdateNumberOfUsedExits();

            // Step 3: Ensure All Rooms Have Enough Exits
            CheckForRoomsWithInvaildNumberOfEdages();

            // Step 4: Add Back Random Edge To The Tree To Create Loops
            int numberOfVertices = vertices.Count;

            for (int i = 0; i < numberOfVertices; i++)
            {
                for (int v = 0; v < numberOfVertices; v++)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    if (
                        rand <= _dungeonData.DungeonProperties.LoopProabilty && 
                        adjacencyMatrix[i, v] != 0 && 
                        !(_edgeList[i].Contains(v) || _edgeList[v].Contains(i)) && 
                        _roomList[v].ExitsUsed < _roomList[v].Exits.Count && 
                        _roomList[i].ExitsUsed < _roomList[i].Exits.Count
                    )
                    {
                        _edgeList[i].Add(v);
                        _roomList[i].ExitsUsed += 1;
                        _roomList[v].ExitsUsed += 1;
                    }

                }
            }
        }

        /// <summary>
        /// Find path the hallways will take between connecting rooms
        /// </summary>
        private void ConstructHallways()
        {
            int numberOfRooms = _roomList.Count;

            for (int i = 0; i < numberOfRooms; i++)
            {
                foreach (int roomID in _edgeList[i])
                {
                    ConstructHallwayBetween(i, roomID);
                }
            }
        }

        /// <summary>
        /// Add terminal rooms to cap off any open exits
        /// </summary>
        private void PlaceTerminalRooms()
        {
            List<Room> roomCopy = new(_roomList);

            foreach (Room room in roomCopy)
            {
                int numberOfTries = 0;
                while (room.ExitsUsed != room.Exits.Count)
                {
                    if (numberOfTries > MaxNumberOfRoomPlacementTries)
                    {
                        if (!_dungeonData.DungeonProperties.AllowGridExtensions) break;
                        numberOfTries = 0;
                        ExtendGridSize();
                    }

                    if (AddTerminalRoom(room.ID)) _roomList[room.ID].ExitsUsed += 1;
                    numberOfTries += 1;
                }
            }
        }

        /// <summary>
        /// Generates the dungeon layout.
        /// </summary>
        private void GenerateDungeon()
        {
            PlaceRooms();
            ConstructLayout();
            ConstructHallways();
            PlaceTerminalRooms();
        }

        /// <summary>
        /// Constrcts a Seralizable Dungeon Object from the genereated dungeon
        /// </summary>
        private Dungeon ConstructSeralizableDungeon()
        {
            SerializableGrid serializableGrid = new(_grid, _gridSize);
            List<SerializableRoom> rooms = new();
            foreach (Room room in _roomList) rooms.Add(room.ToSerializableRoom());

            return new(rooms, serializableGrid, _gridSize, _seed);
        }

        /// <summary>
        /// Generate a dungeon and return a dungeon object.
        /// </summary>
        public Dungeon Generate()
        {
            GenerateDungeon();
            return ConstructSeralizableDungeon();
        }
    }
}
