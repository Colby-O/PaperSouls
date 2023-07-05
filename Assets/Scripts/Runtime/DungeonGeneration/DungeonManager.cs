using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Helpers;

namespace PaperSouls.Runtime.DungeonGeneration
{
    public class DungeonManager : MonoBehaviour
    {
        public bool RegenerateDungeon = false;
        [SerializeField] private int _seed;
        [SerializeField] private RoomData _roomData;
        [SerializeField] private DungeonProperties _properties;

        private RoomGenerator _roomGenerator;
        private List<Room> _roomList;
        [SerializeField] private Vector3 _tileSize = Vector3.zero;

        private TileType[,] _grid;
        private List<DungeonObject> _dungeonObjects;
        private List<Hallway> _hallwayVarients;
        private GameObject _dungeonHolder;

        private Dictionary<int, List<int>> _edgeList;
        private List<Dictionary<Vector2Int, Vector2Int>> _paths;
        private List<Vector2Int> _pathStarts;
        private List<Vector2Int> _pathEnds;

        private static readonly Vector2Int[] DIRECTIONS = new[]
        {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
        };

        private DelaunayTriangulation _triangulation;
        private MST _mst;

        [Header("Debug")]
        [SerializeField] private bool _drawDelaunayTriangulation = false;
        [SerializeField] private bool _drawMST = false;
        [SerializeField] private bool _drawFinalGraph = false;
        [SerializeField] private bool _drawHallwayPaths = false;
        [SerializeField] private bool _drawGridTiles = false;
        private int _numberOfMainRooms;

        /// <summary>
        /// Generate hallway varients for tilemap
        /// </summary>
        private void GenerateHallwayVarients()
        {
            _hallwayVarients = new();

            GameObject verticalHallway = Instantiate(_properties.StrightHallway, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            GameObject horzitionalHallway = Instantiate(_properties.StrightHallway, Vector3.zero, Quaternion.Euler(0f, 90f, 0f));

            GameObject enterenceHallwayLeft = Instantiate(_properties.EnterenceHallway, Vector3.zero, Quaternion.Euler(0f, 270f, 0f));
            GameObject enterenceHallwayRight = Instantiate(_properties.EnterenceHallway, Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
            GameObject enterenceHallwayUp = Instantiate(_properties.EnterenceHallway, Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
            GameObject enterenceHallwayDown = Instantiate(_properties.EnterenceHallway, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));


            GameObject curvedHallwayDownRight = Instantiate(_properties.CurvedHallway, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            GameObject curvedHallwayDownLeft = Instantiate(_properties.CurvedHallway, Vector3.zero, Quaternion.Euler(0f, 90f, 0f));
            GameObject curvedHallwayUpRight = Instantiate(_properties.CurvedHallway, Vector3.zero, Quaternion.Euler(0f, 270f, 0f));
            GameObject curvedHallwayUpLeft = Instantiate(_properties.CurvedHallway, Vector3.zero, Quaternion.Euler(0f, 180f, 0f));

            GameObject threeWayHallwayUp = Instantiate(_properties.ThreeWayHallway, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
            GameObject threeWayHallwayDown = Instantiate(_properties.ThreeWayHallway, Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
            GameObject threeWayHallwayLeft = Instantiate(_properties.ThreeWayHallway, Vector3.zero, Quaternion.Euler(0f, 270f, 0f));
            GameObject threeWayHallwayRight = Instantiate(_properties.ThreeWayHallway, Vector3.zero, Quaternion.Euler(0f, 90f, 0f));

            GameObject fourwayHallway = Instantiate(_properties.FourWayHallway, Vector3.zero, Quaternion.Euler(0f, 0f, 0f)); ;
            if (_tileSize == Vector3.zero) _tileSize = (new Hallway(fourwayHallway, 15)).Size;

            _hallwayVarients.Add(new Hallway(null, 0));
            _hallwayVarients.Add(new Hallway(enterenceHallwayUp, 1));
            _hallwayVarients.Add(new Hallway(enterenceHallwayRight, 2));
            _hallwayVarients.Add(new Hallway(curvedHallwayDownLeft, 3));
            _hallwayVarients.Add(new Hallway(enterenceHallwayDown, 4));
            _hallwayVarients.Add(new Hallway(verticalHallway, 5));
            _hallwayVarients.Add(new Hallway(curvedHallwayUpLeft, 6));
            _hallwayVarients.Add(new Hallway(threeWayHallwayUp, 7));
            _hallwayVarients.Add(new Hallway(enterenceHallwayLeft, 8));
            _hallwayVarients.Add(new Hallway(curvedHallwayDownRight, 9));
            _hallwayVarients.Add(new Hallway(horzitionalHallway, 10));
            _hallwayVarients.Add(new Hallway(threeWayHallwayLeft, 11));
            _hallwayVarients.Add(new Hallway(curvedHallwayUpRight, 12));
            _hallwayVarients.Add(new Hallway(threeWayHallwayDown, 13));
            _hallwayVarients.Add(new Hallway(threeWayHallwayRight, 14));
            _hallwayVarients.Add(new Hallway(fourwayHallway, 15));

            foreach (Hallway hallway in _hallwayVarients)
            {
                if (hallway.Prefab != null)
                {
                    hallway.Prefab.transform.parent = _dungeonHolder.transform;
                    hallway.Prefab.SetActive(false);
                }
            }
        }

        private void Initailzation()
        {
            Random.InitState(_seed);
            _roomGenerator = new(_roomData, _seed);
            _roomList = new();
            _dungeonObjects = new();
            _paths = new();
            _pathStarts = new();
            _pathEnds = new();
            if (_dungeonHolder == null) _dungeonHolder = new GameObject("DungeonHolder");

            _grid = new TileType[_properties.GridSize, _properties.GridSize];

            for (int j = 0; j < _properties.GridSize; j++)
            {
                for (int k = 0; k < _properties.GridSize; k++)
                {
                    _grid[j, k] = TileType.Empty;
                }
            }

            GenerateHallwayVarients();
        }

        /// <summary>
        /// Add a room to the grid
        /// </summary>
        bool UpdateRoomGrid(Vector2Int roomSize, Vector2Int roomPosition, int minRoomSpacing)
        {
            TileType[,] gridTemp = _grid.Clone() as TileType[,];

            bool vaildRoom = true;
            for (int j = -(roomSize.x + minRoomSpacing) / 2; j < (roomSize.x + minRoomSpacing) / 2; j++)
            {
                for (int k = -(roomSize.y + minRoomSpacing) / 2; k < (roomSize.y + minRoomSpacing) / 2; k++)
                {
                    if (roomPosition.x + j >= _properties.GridSize || roomPosition.x + j < 0 || roomPosition.y + k >= _properties.GridSize || roomPosition.y + k < 0)
                    {
                        vaildRoom = false;
                        break;
                    }
                    if ((j < -roomSize.x / 2 || j > roomSize.x / 2 || k < -roomSize.y / 2 || k > roomSize.y / 2) && _grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty) gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.RoomSpacing;
                    else if (_grid[roomPosition.x + j, roomPosition.y + k] == TileType.Empty) gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.Room;
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
            int oldGridSize = _properties.GridSize;
            _properties.GridSize += _properties.GridExtension;
            _grid = new TileType[_properties.GridSize, _properties.GridSize];

            for (int i = 0; i < _properties.GridSize; i++)
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
        /// Generate vaild random position in the grid
        /// </summary>
        bool GenerateRandomPosition(ref Vector2Int roomPosition, ref Vector2Int roomSize)
        {
            int numberOfPlacementTries = 0;
            bool foundVaildRoom;
            do
            {
                foundVaildRoom = UpdateRoomGrid(roomSize, roomPosition, _properties.RoomSpacing);

                if (!foundVaildRoom)
                {
                    roomPosition = new(Random.Range(10, _properties.GridSize - 10), Random.Range(10, _properties.GridSize - 10));
                    roomSize = new(Random.Range(_properties.RoomSize.x, _properties.RoomSize.y), Random.Range(_properties.RoomSize.x, _properties.RoomSize.y));

                    if (roomSize.x % 2 == 0) roomSize.x += 1;
                    if (roomSize.y % 2 == 0) roomSize.y += 1;

                    numberOfPlacementTries += 1;
                    if (_properties.MaxNumberOfRoomPlacementTries < numberOfPlacementTries)
                    {
                        if (!_properties.AllowGridExtensions) break;
                        ExtendGridSize();
                        numberOfPlacementTries = 0;
                    }
                }
            } while (!foundVaildRoom);

            return foundVaildRoom;
        }

        /// <summary>
        /// Create a new room given a prefab, position, scale, and id
        /// </summary>
        void CreateNewRoom(Vector2Int roomPosition, Vector2Int roomSize, Vector2Int numberOfExits, int roomID)
        {
            Vector3 position = new Vector3(roomPosition.x * _tileSize.x, 0.0f, roomPosition.y * _tileSize.z);

            GameObject parnet = new("Room" + roomID);
            parnet.transform.position = position;

            Vector2Int sizeScale = new((int)_tileSize.x, (int)_tileSize.z);
            Room room = _roomGenerator.CreateRoom(parnet, roomSize * sizeScale, _tileSize, Random.Range(numberOfExits.x, numberOfExits.y + 1), roomID);
            room.Prefab.transform.parent = _dungeonHolder.transform;
            _roomList.Add(room);
        }

        /// <summary>
        /// Generate a dungeon room
        /// </summary>
        void GenerateDungeonRooms()
        {
            int numberOfRooms = Random.Range(_properties.NumberOfRooms.x, _properties.NumberOfRooms.y + 1);

            for (int i = 0; i < numberOfRooms; i++)
            {
                Vector2Int roomPosition = new(Random.Range(10, _properties.GridSize - 10), Random.Range(10, _properties.GridSize - 10));

                Vector2Int roomSize = new(Random.Range(_properties.RoomSize.x, _properties.RoomSize.y), Random.Range(_properties.RoomSize.x, _properties.RoomSize.y));
                if (roomSize.x % 2 == 0) roomSize.x += 1;
                if (roomSize.y % 2 == 0) roomSize.y += 1;

                bool foundVaildRoom = GenerateRandomPosition(ref roomPosition, ref roomSize);

                if (foundVaildRoom) CreateNewRoom(roomPosition, roomSize, _properties.NumberOfExits, i);
            }
        }

        /// <summary>
        /// Get A list of room center coordinates
        /// </summary>
        private List<Vector2> GetVerticeList()
        {
            List<Vector2> vertices = new();

            foreach (Room room in _roomList)
            {
                vertices.Add(new Vector2(room.Prefab.transform.position.x, room.Prefab.transform.position.z));
            }

            return vertices;
        }

        /// <summary>
        /// Replace a room.
        /// Called when the original room has too little exits
        /// </summary>
        private void ReplaceRoom(int roomID, int numberOfExitsNeeded)
        {
            Vector3 roomPos = _roomList[roomID].Prefab.transform.position;

            GameObject newParent = new("Room" + _roomList[roomID].ID);
            newParent.transform.position = roomPos;
            newParent.transform.parent = _dungeonHolder.transform;
            Vector2Int roomSize = new((int)_roomList[roomID].Size.x, (int)_roomList[roomID].Size.z);
            if (roomSize.x % 2 == 0) roomSize.x += 1;
            if (roomSize.y % 2 == 0) roomSize.y += 1;
            int usedExits = _roomList[roomID].ExitsUsed;
            Room newRoom = _roomGenerator.CreateRoom(newParent, roomSize, _tileSize, numberOfExitsNeeded, _roomList[roomID].ID);
            newRoom.ExitsUsed = usedExits;

            GameObject.Destroy(_roomList[roomID].Prefab);
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
        /// 
        /// </summary>
        private void ConstructDungeonLayout()
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
                    if (rand <= _properties.LoopProabilty && adjacencyMatrix[i, v] != 0 && !(_edgeList[i].Contains(v) || _edgeList[v].Contains(i)) && _roomList[v].ExitsUsed < _roomList[v].Exits.Count && _roomList[i].ExitsUsed < _roomList[i].Exits.Count)
                    {
                        _edgeList[i].Add(v);
                        _roomList[i].ExitsUsed += 1;
                        _roomList[v].ExitsUsed += 1;
                    }

                }
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
        private void AddHallwayToGrid(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            Vector2Int gridPT = start;
            while (true)
            {
                AddHallwayTileToGrid(gridPT);

                if (!cameFrom.ContainsKey(gridPT)) break;
                else gridPT = cameFrom[gridPT];
            }
            //AddHallwayTileToGrid(end);
        }

        /// <summary>
        /// Generates a path between rooms using A*
        /// </summary>
        private void ConstructHallwayBetween(int roomIDA, int roomIDB)
        {
            Transform startExit = _roomList[roomIDA].AvailableExits.Pop();
            Transform endExit = _roomList[roomIDB].AvailableExits.Pop();

            Vector2Int start = GetRoomdPosition(startExit.position);
            Vector2Int end = GetRoomdPosition(endExit.position);

            PathFinder pathFinder = new(_grid, _properties.Weights);
            Dictionary<Vector2Int, Vector2Int> path = pathFinder.FindOptimalPath(start, end);

            _pathStarts.Add(start);
            _pathEnds.Add(end);
            _paths.Add(path);

            AddHallwayToGrid(path, end, start);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ConstructDungeonHallways()
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
        /// Places a terminal room
        /// </summary>
        private bool AddTerminalRoom(int roomID)
        {
            Vector2Int roomPosition = new(Random.Range(10, _properties.GridSize - 10), Random.Range(10, _properties.GridSize - 10));
            Vector2Int roomSize = new(Random.Range(_properties.RoomSize.x, _properties.RoomSize.y), Random.Range(_properties.RoomSize.x, _properties.RoomSize.y));
            if (roomSize.x % 2 == 0) roomSize.x += 1;
            if (roomSize.y % 2 == 0) roomSize.y += 1;

            bool success = GenerateRandomPosition(ref roomPosition, ref roomSize);

            if (success)
            {
                int newRoomID = _roomList.Count;

                CreateNewRoom(roomPosition, roomSize, new(1, 1), newRoomID);
                ConstructHallwayBetween(roomID, newRoomID);
            }

            return success;
        }
        

        /// <summary>
        /// Add terminal rooms to cap off any open exits
        /// </summary>
        private void CapOffOpenExits()
        {
            List<Room> roomCopy = new(_roomList);

            foreach (Room room in roomCopy)
            {
                int numberOfTries = 0;
                while (room.AvailableExits.Count != 0)
                {
                    if (numberOfTries > _properties.MaxNumberOfRoomPlacementTries)
                    {
                        if (!_properties.AllowGridExtensions) break;
                        numberOfTries = 0;
                        ExtendGridSize();
                    }

                    if (AddTerminalRoom(room.ID)) _roomList[room.ID].ExitsUsed += 1;
                    numberOfTries += 1;
                }
            }
        }

        /// <summary>
        /// Finds the hallway orention needed to be placed
        /// </summary>
        int GetHallwayOrention(TileType[,] tiles)
        {
            int key = 0;
            int i = 0;

            foreach (Vector2Int dir in DIRECTIONS)
            {
                if (tiles[dir.x + 1, dir.y + 1] == TileType.Hallway || tiles[dir.x + 1, dir.y + 1] == TileType.HallwayAndRoom || tiles[dir.x + 1, dir.y + 1] == TileType.Room) key |= 1 << i;
                i += 1;
            }

            return key;
        }

        /// <summary>
        /// Finds the all surrounding tiles
        /// </summary>
        TileType[,] GetSurroundingTiles(Vector2Int pos)
        {
            TileType[,] tiles = new TileType[3, 3];

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (pos.x + i < _properties.GridSize && pos.y + j < _properties.GridSize && pos.x + i >= 0 && pos.y + j >= 0) tiles[i + 1, j + 1] = _grid[pos.x + i, pos.y + j];
                    else tiles[i + 1, j + 1] = TileType.Empty;
                }
            }

            return tiles;
        }

        /// <summary>
        /// Instantiates A Hallway Tile
        /// </summary>
        GameObject InstantiateDungeonHallwayObject(GameObject prefab, Vector3 roomPosition, Vector3 roomSize, int hallwayID)
        {
            GameObject newObject = GameObject.Instantiate(prefab, roomPosition, prefab.transform.localRotation);
            newObject.transform.localScale = roomSize;
            newObject.name = "Hallway_" + hallwayID.ToString();
            newObject.transform.parent = _dungeonHolder.transform;

            return newObject;
        }

        /// <summary>
        /// Generates the hallway mesh
        /// </summary>
        void CreateHallwayMesh(Vector2Int gridPos, int key, int hallwayID)
        {
            Hallway hallway = _hallwayVarients[key];

            Vector3 position = new Vector3(gridPos.x, 0, gridPos.y) * _tileSize.x;
            Vector3 scale = new Vector3(1, 1, 1);

            GameObject hallwayObject = InstantiateDungeonHallwayObject(hallway.Prefab, position, scale, hallwayID);
            hallwayObject.transform.parent = _dungeonHolder.transform;
            hallwayObject.SetActive(true);

            Hallway hallwayInstance = new(hallwayObject, hallwayID);
            hallwayInstance.Prefab.name = "Hallway_" + hallwayID.ToString();
            _dungeonObjects.Add(hallwayInstance);
        }

        /// <summary>
        /// Renderer all hallways
        /// </summary>
        void RenderHallway()
        {
            for (int i = 0; i < _properties.GridSize; i++)
            {
                for (int j = 0; j < _properties.GridSize; j++)
                {
                    //if (grid[i, j] == TileType.ROOM) continue;

                    Vector2Int gridPos = new Vector2Int(i, j);
                    TileType[,] tiles = GetSurroundingTiles(gridPos);

                    int key = GetHallwayOrention(tiles);
                    //|| _grid[i, j] == TileType.HallwayAndRoom
                    if (_grid[i, j] == TileType.Hallway) CreateHallwayMesh(gridPos, key, i * _properties.GridSize + j);
                }
            }
        }

        private void GenerateDungeon()
        {
            Initailzation();
            GenerateDungeonRooms();
            ConstructDungeonLayout();
            ConstructDungeonHallways();
            _numberOfMainRooms = _roomList.Count;
            CapOffOpenExits();
            RenderHallway();
            _dungeonHolder.transform.localScale = _properties.Scale;
            _dungeonHolder.transform.position = _properties.Position;
            _dungeonHolder.transform.rotation = _properties.Rotation;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DestroyDungeon()
        {
            foreach (DungeonObject obj in _dungeonObjects) GameObject.Destroy(obj.Prefab);
            foreach (DungeonObject obj in _roomList) GameObject.Destroy(obj.Prefab);
        }

        /// <summary>
        /// Draw path for debugging
        /// </summary>
        void DrawPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            Vector2Int gridPT = start;
            while (true)
            {
                if (!cameFrom.ContainsKey(gridPT)) break;

                Vector3 worldPT = new Vector3(gridPT.x * _tileSize.x * _properties.Scale.x, 0, gridPT.y * _tileSize.z * _properties.Scale.z);
                Vector3 nextWorldPT = new Vector3(cameFrom[gridPT].x * _tileSize.x * _properties.Scale.x, 0, cameFrom[gridPT].y * _tileSize.z * _properties.Scale.z);

                if (_grid[cameFrom[gridPT].x, cameFrom[gridPT].y] == TileType.Room || _grid[cameFrom[gridPT].x, cameFrom[gridPT].y] == TileType.HallwayAndRoom) UnityEngine.Debug.DrawLine(worldPT, nextWorldPT, Color.red, Mathf.Infinity);
                else UnityEngine.Debug.DrawLine(worldPT, nextWorldPT, Color.magenta, Mathf.Infinity);

                gridPT = cameFrom[gridPT];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DrawGrid()
        {
            for (int i = 0; i < _properties.GridSize; i++)
            {
                for (int j = 0; j < _properties.GridSize; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(i * _tileSize.x *_properties.Scale.x, 0, j * _tileSize.z * _properties.Scale.z);
                    cube.transform.localScale = new Vector3(_tileSize.x * _properties.Scale.x, _properties.Scale.z, _tileSize.z * _properties.Scale.z);

                    if (_grid[i, j] == TileType.Room) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
                    else if (_grid[i, j] == TileType.RoomSpacing) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.yellow);
                    else if (_grid[i, j] == TileType.Empty) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
                    else if (_grid[i, j] == TileType.Hallway) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
                    else cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.grey);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Debug()
        {
            if (_drawDelaunayTriangulation)
            {
                _drawDelaunayTriangulation = false;
                
                _triangulation.DrawDelaunayTriangulation();
            }

            if (_drawMST)
            {
                _drawMST = false;
                // Debug
                _mst.DrawMST();
            }

            if (_drawFinalGraph)
            {
                _drawFinalGraph = false;
                // Debug
                for (int i = 0; i < _numberOfMainRooms; i++)
                {
                    if (_edgeList[i].Count == 0) continue;
                    for (int j = 0; j < _edgeList[i].Count; j++)
                    {
                        UnityEngine.Debug.DrawLine(_roomList[_edgeList[i][j]].Prefab.transform.position, _roomList[i].Prefab.transform.position, Color.blue, Mathf.Infinity);
                    }
                }
            }

            if (_drawHallwayPaths)
            {
                _drawHallwayPaths = false;

                for (int i = 0; i < _paths.Count; i++)
                {
                    DrawPath(_paths[i], _pathEnds[i], _pathStarts[i]);
                }
            }

            if (_drawGridTiles)
            {
                _drawGridTiles = false;
                DrawGrid();
            }
        }

        private void Awake()
        {
            GenerateDungeon();
        }

        private void Update()
        {
            if(RegenerateDungeon)
            {
                RegenerateDungeon = false;
                DestroyDungeon();
                GenerateDungeon();
            }
            Debug();
        }

        private void OnDestroy()
        {
            DestroyDungeon();
        }
    }
}
