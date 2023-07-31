using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;
using PaperSouls.Runtime.Helpers;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal sealed class DungeonGenerator
    {
        private const int MaxNumberOfMainRoomPlacementTries = 100;
        private const int MaxNumberOfRoomPlacementTries = 1000;
        private const int GridExtensionAmount = 10;

        // Generation seed
        public int Seed { get; set; }
        // Dungeon generations properties
        public DungeonProperties DungeonProperties { get; set; }
        // The size of a single tile
        public Vector3 TileSize { get; set; }

        // List of room in the dungeon
        private List<Room> _primaryRoomList;
        private List<Room> _secondaryRoomList;
        // A grid that store what each tile type is at each position
        private TileType[,] _grid;
        // Size of the map
        private int _gridSize;

        private Dictionary<int, List<int>> _edgeList;
        private List<Dictionary<Vector2Int, Vector2Int>> _paths;
        private List<Vector2Int> _pathStarts;
        private List<Vector2Int> _pathEnds;

        // Helper Classes
        private DelaunayTriangulation _triangulation;
        private MST _mst;

        // Generators
        private RoomGenerator _roomGenerator;

        public DungeonGenerator(int seed, DungeonProperties properties, Vector3 tileSize)
        {
            Random.InitState(seed);

            Seed = seed;
            DungeonProperties = properties;
            TileSize = tileSize;

            _primaryRoomList = new List<Room>();
            _secondaryRoomList = new List<Room>();
            _paths = new List<Dictionary<Vector2Int, Vector2Int>>();
            _pathStarts = new List<Vector2Int>();
            _pathEnds = new List<Vector2Int>();
            _roomGenerator = new RoomGenerator(
                properties.RoomData, 
                seed, 
                new Vector2Int(Mathf.RoundToInt(tileSize.x), Mathf.RoundToInt(tileSize.z))
            );

            _gridSize = DungeonProperties.GenerationProperties.GridSize;
            _grid = ArrayHelper.CreateAndFill(_gridSize, _gridSize, TileType.Empty);
        }

        /// <summary>
        /// Gets a random room position in the gird.
        /// </summary>
        private Vector2Int GetVaildRoomPosition()
        {
            if (DungeonProperties.GenerationProperties.BorderSpacing >= _gridSize)
            {
                Debug.LogError(
                    $"Border spacing cannot be greater than or equal to the grid size!\n" +
                    $"{DungeonProperties.GenerationProperties.BorderSpacing} >= {_gridSize}"
                );
                return new Vector2Int();
            }

            return new Vector2Int(
                 Random.Range(
                     DungeonProperties.GenerationProperties.BorderSpacing,
                     _gridSize - DungeonProperties.GenerationProperties.BorderSpacing
                 ),
                 Random.Range(
                     DungeonProperties.GenerationProperties.BorderSpacing,
                     _gridSize - DungeonProperties.GenerationProperties.BorderSpacing
                 )
             );
        }

        /// <summary>
        /// Gets a random room size that is vaild. Only odd room sizes are vaild.
        /// </summary>
        private Vector2Int GetVaildRoomSize(Vector2Int roomSizeLimits)
        {
            // Fetches a random size bias toward smaller values
            // TODO: In the future is might be benfical to add
            //       a skewness parameter to control this bias
            Vector2Int roomSize = new Vector2Int(
                RandomGenerator.GetRandomSkewed(
                    roomSizeLimits.x,
                    roomSizeLimits.y
                ),
                RandomGenerator.GetRandomSkewed(
                    roomSizeLimits.x, 
                    roomSizeLimits.y
                )
            );

            // Ensures the roomSize is an odd number (I don't know why this matters but it does)
            // We'll need to fix this before we add custom rooms the erorr is likely in the
            // RoomGenerator script
            if (roomSize.x % 2 == 0) roomSize.x += 1;
            if (roomSize.y % 2 == 0) roomSize.y += 1;

            return roomSize;
        }

        /// <summary>
        /// Extends the size of the grid
        /// </summary>
        void ExtendGridSize()
        {
            TileType[,] oldGrid = _grid.Clone() as TileType[,];
            int oldGridSize = _gridSize;
            _gridSize += GridExtensionAmount;
            _grid = ArrayHelper.CreateAndFill(_gridSize, _gridSize, TileType.Empty);

            for (int i = 0; i < oldGridSize; i++)
            {
                for (int j = 0; j < oldGridSize; j++)
                {
                    _grid[i, j] = oldGrid[i, j];
                }
            }
        }

        /// <summary>
        /// Adds a room to the grid.
        /// </summary>
        bool UpdateRoomGrid(
            Vector2Int roomSize, 
            Vector2Int roomPosition, 
            int minRoomSpacing,
            int minRoomDst,
            int borderSpacing,
            TileType tileType = TileType.Room
        )
        {
            TileType[,] gridTemp = _grid.Clone() as TileType[,];

            bool vaildRoom = true;
            for (int j = -(roomSize.x / 2 + minRoomSpacing + minRoomDst); j < (roomSize.x / 2 + minRoomSpacing + minRoomDst); j++)
            {
                for (int k = -(roomSize.y / 2 + minRoomSpacing + minRoomDst); k < (roomSize.y / 2 + minRoomSpacing + minRoomDst); k++)
                {
                    if (
                        roomPosition.x + j >= _gridSize - borderSpacing ||
                        roomPosition.x + j < borderSpacing ||
                        roomPosition.y + k >= _gridSize - borderSpacing ||
                        roomPosition.y + k < borderSpacing
                    )
                    {
                        vaildRoom = false;
                        break;
                    }
                    else if (
                           (
                            j < -(roomSize.x / 2 + minRoomSpacing) || 
                            j >= (roomSize.x / 2 + minRoomSpacing) || 
                            k < -(roomSize.y / 2 + minRoomSpacing) || 
                            k >= (roomSize.y / 2 + minRoomSpacing)
                           )
                           && _grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty
                       )
                       {
                        gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.Invaild;
                       }
                    else if (
                        (j < -roomSize.x / 2 || j >= roomSize.x / 2 || k < -roomSize.y / 2 || k >= roomSize.y / 2)
                        && _grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty
                    )
                    {
                        gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.RoomSpacing;
                    }
                    else if (_grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty)
                    {
                        gridTemp[roomPosition.x + j, roomPosition.y + k] = tileType;
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
        /// Generate vaild random position in the grid for a main room
        /// </summary>
        private bool GenerateRandomMainRoomLocation(
            out Vector2Int roomPosition, 
            out Vector2Int roomSize,
            Vector2Int roomSizeRange,
            int minRoomSpacing,
            int minRoomDst,
            TileType tileType = TileType.Room
        )
        {
            int numberOfPlacementTries = 0;
            bool foundVaildRoom = false;
            do
            {
                roomPosition = GetVaildRoomPosition();
                roomSize = GetVaildRoomSize(roomSizeRange);

                numberOfPlacementTries++;
                if (MaxNumberOfMainRoomPlacementTries < numberOfPlacementTries)
                {
                    if (!DungeonProperties.GenerationProperties.AllowGridExtensions) break;
                    ExtendGridSize();
                    numberOfPlacementTries = 0;
                }

                foundVaildRoom = UpdateRoomGrid(
                    roomSize, 
                    roomPosition, 
                    minRoomSpacing,
                    minRoomDst,
                    DungeonProperties.GenerationProperties.BorderSpacing,
                    tileType
                );
            } while (!foundVaildRoom);

            return foundVaildRoom;
        }

        /// <summary>
        /// Create a new room given a prefab, position, scale, and id
        /// </summary>
        void CreateNewRoom(
            Vector2Int roomPosition, 
            Vector2Int roomSize, 
            Vector2Int numberOfExits, 
            int roomID
        )
        {
            Vector3 position = new(roomPosition.x * TileSize.x, 0.0f, roomPosition.y * TileSize.z);
            Vector3 size = new(roomSize.x * TileSize.x, 0, roomSize.y * TileSize.z);
            int numExits = Random.Range(numberOfExits.x, numberOfExits.y + 1);
            _primaryRoomList.Add(_roomGenerator.GenerateRoomObject(position, size, numExits, roomID));
        }

        private void PlaceMainRooms()
        {
            // TODO: Add the option for this function to place remade rooms.
            int numberOfRooms = DungeonProperties.GenerationProperties.NumberOfMainRooms;
            
            for (int i = 0; i < numberOfRooms; i++)
            {
                if (
                    GenerateRandomMainRoomLocation(
                        out Vector2Int roomPosition, 
                        out Vector2Int roomSize, 
                        DungeonProperties.GenerationProperties.MainRoomSize,
                        DungeonProperties.GenerationProperties.MainRoomSpacing,
                        DungeonProperties.GenerationProperties.MinDistacneBetweebMainRooms,
                        TileType.MainRoom
                    )
                )
                {
                    CreateNewRoom(
                        roomPosition,
                        roomSize,
                        DungeonProperties.GenerationProperties.NumberOfExits,
                        _primaryRoomList.Count
                    );
                }
            }

            // Replace all invaild tiles with empty
            ArrayHelper.Replace(_grid, TileType.Invaild, TileType.Empty);
        }

        /// <summary>
        /// Fill the entire grid with room at a specific density
        /// </summary>
        private void FillGridWithRooms()
        {
            int totalNumberOfAvailableTiles = ArrayHelper.Count(_grid, TileType.Empty, DungeonProperties.GenerationProperties.BorderSpacing);
            float fillPercentage = 0;
            int numberOfPlacementTries = 0;
            while (fillPercentage < DungeonProperties.GenerationProperties.RoomDensity)
            {
                Vector2Int roomPosition = GetVaildRoomPosition();
                Vector2Int roomSize = GetVaildRoomSize(DungeonProperties.GenerationProperties.RoomSize);

                if (
                    UpdateRoomGrid(
                        roomSize,
                        roomPosition,
                        DungeonProperties.GenerationProperties.RoomSpacing,
                        0,
                        DungeonProperties.GenerationProperties.BorderSpacing,
                        TileType.Room
                    )
                )
                {
                    _secondaryRoomList.Add(
                        new Room(new(roomPosition.x * TileSize.x, 0.0f, roomPosition.y * TileSize.z),
                        new(roomSize.x * TileSize.x, 0.0f, roomSize.y * TileSize.z),
                        new(),
                        Random.state,
                        _secondaryRoomList.Count + _primaryRoomList.Count));
                    numberOfPlacementTries = 0;
                }
                else numberOfPlacementTries++;

                if (MaxNumberOfRoomPlacementTries < numberOfPlacementTries) break;

                fillPercentage = 1.0f - ArrayHelper.Count(_grid, TileType.Empty, DungeonProperties.GenerationProperties.BorderSpacing) / (float)totalNumberOfAvailableTiles;
            }
        }

        /// <summary>
        /// Places "dummy" room or in other word rooms that are triangulated but are not
        /// "main" rooms. This make the layout more intersting than just going from main room
        /// to main room.
        /// </summary>
        private void PlaceDummyRooms()
        {
            int numberOfRooms = Random.Range(
                DungeonProperties.GenerationProperties.NumberOfDummyRooms.x, 
                DungeonProperties.GenerationProperties.NumberOfDummyRooms.y + 1
            );

            for (int i = 0; i < numberOfRooms; i++)
            {
                if (
                    GenerateRandomMainRoomLocation(
                        out Vector2Int roomPosition,
                        out Vector2Int roomSize,
                        DungeonProperties.GenerationProperties.RoomSize,
                        DungeonProperties.GenerationProperties.MainRoomSpacing,
                        0,
                        TileType.MainRoom
                    )
                )
                {
                    CreateNewRoom(
                        roomPosition,
                        roomSize,
                        DungeonProperties.GenerationProperties.NumberOfExits,
                        _primaryRoomList.Count
                    );
                }
            }
        }

        /// <summary>
        /// Get A list of room center coordinates
        /// </summary>
        private List<Vector2> GetVerticeList()
        {
            List<Vector2> vertices = new();

            foreach (Room room in _primaryRoomList)
            {
                vertices.Add(new Vector2(room.Position.x, room.Position.z));
            }

            return vertices;
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
                    _primaryRoomList[vertex].ExitsUsed += 1;
                    _primaryRoomList[adjVertex].ExitsUsed += 1;
                }
            }
        }

        /// <summary>
        /// Replace a room.
        /// Called when the original room has too little exits
        /// </summary>
        private void ReplaceRoom(int roomID, int numberOfExitsNeeded)
        {
            int usedExits = _primaryRoomList[roomID].ExitsUsed;
            Room newRoom = _roomGenerator.GenerateRoomObject(_primaryRoomList[roomID].Position, _primaryRoomList[roomID].Size, numberOfExitsNeeded, _primaryRoomList[roomID].ID);
            newRoom.ExitsUsed = usedExits;
            _primaryRoomList[roomID] = newRoom;
        }

        /// <summary>
        /// Makes sure all room have enough exits 
        /// </summary>
        private void CheckForRoomsWithInvaildNumberOfEdages()
        {
            for (int v = 0; v < _primaryRoomList.Count; v++)
            {
                if (_primaryRoomList[v].ExitsUsed > _primaryRoomList[v].Exits.Count) ReplaceRoom(v, _primaryRoomList[v].ExitsUsed + 1);
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
                        rand <= DungeonProperties.GenerationProperties.LoopProabilty &&
                        adjacencyMatrix[i, v] != 0 &&
                        !(_edgeList[i].Contains(v) || _edgeList[v].Contains(i)) &&
                        _primaryRoomList[v].ExitsUsed < _primaryRoomList[v].Exits.Count &&
                        _primaryRoomList[i].ExitsUsed < _primaryRoomList[i].Exits.Count
                    )
                    {
                        _edgeList[i].Add(v);
                        _primaryRoomList[i].ExitsUsed += 1;
                        _primaryRoomList[v].ExitsUsed += 1;
                    }
                }
            }
        }

        /// <summary>
        /// Converts world coordinates into grid space cooridnates 
        /// </summary>
        private Vector2Int GetRoomdPosition(Vector3 start)
        {
            return new Vector2Int(Mathf.RoundToInt((start.x / TileSize.x)), Mathf.RoundToInt((start.z / TileSize.z)));
        }

        /// <summary>
        /// Add hallway to the grid
        /// </summary>
        private void AddHallwayTileToGrid(Vector2Int gridPos)
        {
            if (_grid[gridPos.x, gridPos.y] == TileType.Room || _grid[gridPos.x, gridPos.y] == TileType.HallwayAndRoom) _grid[gridPos.x, gridPos.y] = TileType.HallwayAndRoom;
            else if (_grid[gridPos.x, gridPos.y] == TileType.MainRoom) _grid[gridPos.x, gridPos.y] = TileType.MainRoom;
            else _grid[gridPos.x, gridPos.y] = TileType.Hallway;

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
            Vector3 startExit = _primaryRoomList[roomIDA].GetAvailableExit();
            Vector3 endExit = _primaryRoomList[roomIDB].GetAvailableExit();

            Vector2Int start = GetRoomdPosition(startExit);
            Vector2Int end = GetRoomdPosition(endExit);

            PathFinder pathFinder = new(_grid, DungeonProperties.GenerationProperties.Weights);
            Dictionary<Vector2Int, Vector2Int> path = pathFinder.FindOptimalPath(start, end);

            _pathStarts.Add(start);
            _pathEnds.Add(end);
            _paths.Add(path);

            AddHallwayToGrid(path, end);
        }

        /// <summary>
        /// Find path the hallways will take between connecting rooms
        /// </summary>
        private void ConstructHallways()
        {
            int numberOfRooms = _primaryRoomList.Count;

            for (int i = 0; i < numberOfRooms; i++)
            {
                foreach (int roomID in _edgeList[i])
                {
                    ConstructHallwayBetween(i, roomID);
                }
            }
        }

        /// <summary>
        /// Remove all rooms from the grid and roomList that were not used in the final 
        /// dungeon.
        /// </summary>
        private void RemoveUnusedRooms()
        {
            List<Room> newRoomList = new();
            foreach(Room room in _secondaryRoomList)
            {
                if (!room.CheckIfReachable(_grid, new(Mathf.RoundToInt(TileSize.x), Mathf.RoundToInt(TileSize.z))))
                {
                    room.RemoveFromGrid(ref _grid, new(Mathf.RoundToInt(TileSize.x), Mathf.RoundToInt(TileSize.z)));
                }
                else newRoomList.Add(room);
            }
            _secondaryRoomList = new(newRoomList);
        }

        /// <summary>
        /// Places a terminal room
        /// </summary>
        private bool AddTerminalRoom(int roomID)
        {
            if (
                GenerateRandomMainRoomLocation(
                        out Vector2Int roomPosition,
                        out Vector2Int roomSize,
                        DungeonProperties.GenerationProperties.RoomSize,
                        DungeonProperties.GenerationProperties.MainRoomSpacing,
                        0,
                        TileType.MainRoom
                )
            )
            {
                int newRoomID = _primaryRoomList.Count;

                CreateNewRoom(roomPosition, roomSize, new(1, 1), newRoomID);
                _edgeList[roomID].Add(newRoomID);
                _edgeList[newRoomID] = new();
                _primaryRoomList[newRoomID].ExitsUsed += 1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add terminal rooms to cap off any open exits
        /// </summary>
        private void PlaceTerminalRooms()
        {
            List<Room> roomCopy = new(_primaryRoomList);

            foreach (Room room in roomCopy)
            {
                int numberOfTries = 0;
                while (room.ExitsUsed != room.Exits.Count)
                {
                    if (numberOfTries > MaxNumberOfRoomPlacementTries)
                    {
                        if (!DungeonProperties.GenerationProperties.AllowGridExtensions) break;
                        numberOfTries = 0;
                        ExtendGridSize();
                    }

                    if (AddTerminalRoom(room.ID)) _primaryRoomList[room.ID].ExitsUsed += 1;
                    numberOfTries += 1;
                }
            }
        }


        /// <summary>
        /// Find the locations of all exits i.e. room/room and room/hallway
        /// intersetions.
        /// </summary>
        private void FindSecondaryRoomExits()
        {
            foreach (Room room in _secondaryRoomList) room.FindExits(_grid, TileSize);
        }

        /// <summary>
        /// Handles the main dungeon generation code for the generator. 
        /// </summary>
        private void GenerateDungeon()
        {
            // Step 1: Places "main" rooms through out the level the player
            //         must vist to progress i.e. boss rooms 
            PlaceMainRooms();

            // Step 2: Add "dummy" room is the primary roomList to be used in the trigulation
            PlaceDummyRooms();

            // Step 3: Construct the connections between main rooms.
            //         This insures all main rooms will be included
            //         in the final graph and will create a pesudo
            //         level progression.
            ConstructLayout();

            // Step 4: Caps off any unused exits on the main or dummy rooms
            PlaceTerminalRooms();

            // Step 5: Fills the grid with "normal" rooms with a specified density.
            //         Lower room density will result in longer hallways and vice versa
            FillGridWithRooms();

            // Step 6: Construct hallway between connecting main rooms
            ConstructHallways();

            // Step 7: Removes all rooms that were not used i.e. remove all leafs from the graph
            RemoveUnusedRooms();

            // Step 8: Deduce where the exits should be placed on secondary rooms
            FindSecondaryRoomExits();
        }

        /// <summary>
        /// Takes the result of the GenerateDungeon function and stores the 
        /// result in a serlaizable format. 
        /// </summary>
        private Dungeon ConstructSeralizableDungeon()
        {
            SerializableGrid serializableGrid = new(_grid, _gridSize);
            List<SerializableRoom> rooms = new();
            foreach (Room room in _primaryRoomList) rooms.Add(room.ToSerializableRoom());
            foreach (Room room in _secondaryRoomList) rooms.Add(room.ToSerializableRoom());

            return new(rooms, serializableGrid, _gridSize, Seed);
        }

        /// <summary>
        /// Generates a dungeon and return a dungeon object.
        /// </summary>
        public Dungeon Generate()
        {
            GenerateDungeon();
            return ConstructSeralizableDungeon();
        }
    }
}
