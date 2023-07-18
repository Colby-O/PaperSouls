using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    /// <summary>
    /// Defines room Tile Types
    /// </summary>
    internal enum RoomTileType
    {
        Empty,
        Wall
    }

    internal sealed class RoomGenerator
    {
        // Room Data
        private RoomData _roomData;
        private int _seed;
        private Vector2Int _roomSize;
        private int _numEnterences;
        private List<Bounds> _subRooms;

        // Room Object Wrappers
        private GameObject _parnet;
        private GameObject _mesh;
        private GameObject _exits;

        // Interal Variables
        private RoomZone[,] _grid;
        private Vector2 _wallCount;
        private float _wallSize;
        private Vector2 _floorSize;
        private Vector2 _floorCount;
        
        private List<Vector2> _subroomStart;
        private List<Vector2> _subroomEnd;

        public int test;
        public RoomGenerator(RoomData roomData, int seed)
        {
            _seed = seed;
            _roomData = roomData;
            Random.InitState(_seed);
        }

        /// <summary>
        /// Fetches the generate the locations of the room exits
        /// </summary>
        List<int> GetExterenceLocations(Vector2 wallCount)
        {
            List<int> enterenceIndices = new();

            if (_numEnterences > (int)(2 * wallCount.x + 2 * wallCount.y - 8)) _numEnterences = (int)(2 * wallCount.x + 2 * wallCount.y - 8);

            for (int i = 0; i < _numEnterences; i++)
            {
                int rand = Random.Range(-2 * (int)_wallCount.x, 2 * (int)_wallCount.y + 1);

                while (
                    rand == 0 || 
                    rand == -1 ||
                    rand == -(int)wallCount.x - 1 || 
                    rand == 1 || rand == wallCount.y + 1 ||
                    rand == -2 * (int)wallCount.x || 
                    rand == -(int)wallCount.x || 
                    rand == 2 * (int)wallCount.y  || 
                    rand == (int)wallCount.y || 
                    enterenceIndices.Contains(rand)
                    ) rand = Random.Range(-2 * (int)_wallCount.x, 2 * (int)_wallCount.y + 1);

                enterenceIndices.Add(rand);
            }

            return enterenceIndices;
        }

        /// <summary>
        /// Pick a random asset from a list
        /// </summary>
        DungeonObject GetRandomAsset(List<DungeonObject> objs)
        {
            bool assetFound = false;
            DungeonObject obj = null;
            do
            {
                int rand = Random.Range(0, objs.Count);
                float prob = Random.value;

                if (prob <= objs[rand].Proability)
                {
                    assetFound = true;
                    obj = objs[rand];
                }

            } while (!assetFound);

            return obj;
        }

        void AddEnterenceToGrid(Vector2Int pos, Vector2Int wallTileSize)
        {
            for (int i = 0; i < wallTileSize.x; i++)
            {
                for (int j = 0; j < wallTileSize.y; j++)
                {
                    _grid[pos.x + i, pos.y + j] = RoomZone.Invalid;
                }
            }
        }

        /// <summary>
        /// Create dungeon walls
        /// </summary>
        void CreateWalls()
        {
            _wallCount = new(Mathf.Max(1, Mathf.FloorToInt(_roomSize.x / _wallSize)), Mathf.Max(1, Mathf.FloorToInt(_roomSize.y / _wallSize)));
            Vector2 scale = new((_roomSize.x / _wallCount.x) / _wallSize, (_roomSize.y / _wallCount.y) / _wallSize);

            List<int> enterenceIndices = GetExterenceLocations(_wallCount);

            for (int i = 0; i < _wallCount.x; i++)
            {
                Vector3 positionRight = _parnet.transform.position + new Vector3(-_roomSize.x / 2f + _wallSize * scale.x / 2f + i * scale.x * _wallSize, 0, _roomSize.y / 2f);
                Vector3 positionLeft = _parnet.transform.position + new Vector3(-_roomSize.x / 2f + _wallSize * scale.x / 2f + i * scale.x * _wallSize, 0, -_roomSize.y / 2f);

                Quaternion rotation = _parnet.transform.rotation;
                Vector3 scaleVector = new(scale.x, 1, 1);

                DungeonObject leftWallAsset = (enterenceIndices.Contains(-i - 1)) ? GetRandomAsset(_roomData.enterenceObjects) : GetRandomAsset(_roomData.wallObjects);
                DungeonObject rightWallAsset = (enterenceIndices.Contains(-(int)_wallCount.x - i - 1)) ? GetRandomAsset(_roomData.enterenceObjects) : GetRandomAsset(_roomData.wallObjects);

                if (enterenceIndices.Contains(-i - 1))
                {
                    GameObject exit = new("exit" + (_exits.transform.childCount + 1));
                    exit.transform.position = positionRight;
                    exit.transform.parent = _exits.transform;
                    AddEnterenceToGrid(new(i * Mathf.RoundToInt(scale.x * _wallSize), _roomSize.y - 1), new((int)(scale.x * _wallSize), 1));
                }

                if (enterenceIndices.Contains(-(int)_wallCount.x - i - 1))
                {
                    GameObject exit = new("exit" + (_exits.transform.childCount + 1));
                    exit.transform.position = positionLeft;
                    exit.transform.parent = _exits.transform;
                    AddEnterenceToGrid(new(i * Mathf.RoundToInt(scale.x * _wallSize), 0), new((int)(scale.x * _wallSize), 1));
                }

                GameObject wallRight = GameObject.Instantiate(leftWallAsset.Prefab, positionRight, rotation * Quaternion.Euler(0f, 180f, 0f), _mesh.transform);
                GameObject wallLeft = GameObject.Instantiate(rightWallAsset.Prefab, positionLeft, rotation * Quaternion.Euler(0f, 0f, 0f), _mesh.transform);

                wallRight.transform.localScale = scaleVector;
                wallLeft.transform.localScale = scaleVector;

                wallRight.transform.parent = _mesh.transform;
                wallLeft.transform.parent = _mesh.transform;
            }

            for (int i = 0; i < _wallCount.y; i++)
            {
                Vector3 positionDown = _parnet.transform.position + new Vector3(_roomSize.x / 2f, 0, -_roomSize.y / 2f + _wallSize * scale.y / 2f + i * scale.y * _wallSize);
                Vector3 positionUp = _parnet.transform.position + new Vector3(-_roomSize.x / 2f, 0, -_roomSize.y / 2f + _wallSize * scale.y / 2f + i * scale.y * _wallSize);

                Quaternion rotation = _parnet.transform.rotation;
                Vector3 scaleVector = new(scale.y, 1, 1);

                DungeonObject upWallAsset = (enterenceIndices.Contains(i + 1)) ? GetRandomAsset(_roomData.enterenceObjects) : GetRandomAsset(_roomData.wallObjects);
                DungeonObject downWallAsset = (enterenceIndices.Contains((int)_wallCount.y + i + 1)) ? GetRandomAsset(_roomData.enterenceObjects) : GetRandomAsset(_roomData.wallObjects);

                if (enterenceIndices.Contains(i + 1))
                {
                    GameObject exit = new("exit" + (_exits.transform.childCount + 1));
                    exit.transform.position = positionDown;
                    exit.transform.parent = _exits.transform;
                    AddEnterenceToGrid(new(_roomSize.x - 1, i * Mathf.RoundToInt(scale.y * _wallSize)), new(1, (int)(scale.y * _wallSize)));
                }

                if (enterenceIndices.Contains((int)_wallCount.y + i + 1))
                {
                    GameObject exit = new("exit" + (_exits.transform.childCount + 1));
                    exit.transform.position = positionUp;
                    exit.transform.parent = _exits.transform;
                    AddEnterenceToGrid(new(0, i * Mathf.RoundToInt(scale.y * _wallSize)), new(1, (int)(scale.y * _wallSize)));
                }

                GameObject wallDown = GameObject.Instantiate(upWallAsset.Prefab, positionDown, rotation * Quaternion.Euler(0f, 270f, 0f), _mesh.transform);
                GameObject wallUp = GameObject.Instantiate(downWallAsset.Prefab, positionUp, rotation * Quaternion.Euler(0f, 90f, 0f), _mesh.transform);

                wallDown.transform.localScale = scaleVector;
                wallUp.transform.localScale = scaleVector;

                wallDown.transform.parent = _mesh.transform;
                wallUp.transform.parent = _mesh.transform;
            }
        }

        /// <summary>
        /// Create dungeon floors
        /// </summary>
        void CreateFloors()
        {
            _floorCount = new(Mathf.Max(1, Mathf.FloorToInt(_roomSize.x / _floorSize.x)), Mathf.Max(1, Mathf.FloorToInt(_roomSize.y / _floorSize.y)));
            Vector2 scale = new((_roomSize.x / _floorCount.x) / _floorSize.x, (_roomSize.y / _floorCount.y) / _floorSize.y);

            for (int i = 0; i < _floorCount.x; i++)
            {
                for (int j = 0; j < _floorCount.y; j++)
                {
                    Vector3 position = _parnet.transform.position + new Vector3(-_roomSize.x / 2f + _floorSize.x * scale.x / 2 + i * scale.x * _floorSize.x, 0, -_roomSize.y / 2f + _floorSize.y * scale.y / 2f + j * scale.y * _floorSize.y);
                    Quaternion rotation = _parnet.transform.rotation;

                    int rand = Random.Range(0, 4);
                    float rotate = 0;

                    if (!_roomData.useRandomFloorRotation) rotate = 0;
                    else if (rand == 0) rotate = 0;
                    else if (rand == 1) rotate = 90;
                    else if (rand == 2) rotate = 180;
                    else rotate = 270;

                    Vector3 scaleVector;
                    if (rand == 0 || rand == 2) scaleVector = new(scale.x, 1, scale.y);
                    else scaleVector = new(scale.y, 1, scale.x);

                    DungeonObject floorObject = GetRandomAsset(_roomData.floorObjects);
                    
                    GameObject floor = GameObject.Instantiate(floorObject.Prefab, position, rotation * Quaternion.Euler(0f, rotate, 0f), _mesh.transform);
                    floor.transform.localScale = scaleVector;
                    floor.transform.parent = _mesh.transform;
                }
            }
        }

        /// <summary>
        /// Create dungeon pillar
        /// </summary>
        void CreatePillar()
        {
            if (_roomData.pillarObjects.Count <= 0) return;

            DungeonObject pillar = GetRandomAsset(_roomData.pillarObjects);
            GameObject.Instantiate(pillar.Prefab, _parnet.transform.position + new Vector3(-_roomSize.x / 2, 0, -_roomSize.y / 2), _parnet.transform.rotation, _mesh.transform);
            GameObject.Instantiate(pillar.Prefab, _parnet.transform.position + new Vector3(-_roomSize.x / 2, 0, _roomSize.y / 2), _parnet.transform.rotation, _mesh.transform);
            GameObject.Instantiate(pillar.Prefab, _parnet.transform.position + new Vector3(_roomSize.x / 2, 0, -_roomSize.y / 2), _parnet.transform.rotation, _mesh.transform);
            GameObject.Instantiate(pillar.Prefab, _parnet.transform.position + new Vector3(_roomSize.x / 2, 0, _roomSize.y / 2), _parnet.transform.rotation, _mesh.transform);
        }

        /// <summary>
        /// Vertival split for BSP
        /// </summary>
        private void SplitRoomVertically(Bounds room, Queue<Bounds> roomsQueue, int minWidth)
        {
            //int xSplit = Mathf.Max(minWidth, room.size.x - minWidth);
            float xSplit = Random.Range(minWidth, room.size.x - minWidth);

            float centerOffsetRoom1 = room.center.x - (room.size.x - xSplit) / 2f;
            float centerOffsetRoom2 = room.center.x + xSplit / 2f;

            Bounds room1 = new(new Vector3(centerOffsetRoom1, room.center.y, room.center.z), new Vector3(xSplit, room.size.y, room.size.z));
            Bounds room2 = new(new Vector3(centerOffsetRoom2, room.center.y, room.center.z), new Vector3(room.size.x - xSplit, room.size.y, room.size.z));

            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }

        /// <summary>
        /// Horizontally split for BSP
        /// </summary>
        private void SplitRoomHorizontally(Bounds room, Queue<Bounds> roomsQueue, int minHeight)
        {
            //int ySplit = Mathf.Max(minHeight, room.size.y - minHeight);
            float ySplit = Random.Range(minHeight, room.size.y - minHeight);

            float centerOffsetRoom1 = room.center.y - (room.size.y - ySplit) / 2f;
            float centerOffsetRoom2 = room.center.y + ySplit / 2f;

            Bounds room1 = new(new Vector3(room.center.x, centerOffsetRoom1, room.center.z), new Vector3(room.size.x, ySplit, room.size.z));
            Bounds room2 = new(new Vector3(room.center.x, centerOffsetRoom2, room.center.z), new Vector3(room.size.x, room.size.y - ySplit, room.size.z));

            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }

        /// <summary>
        /// Uses Binary Space Partitioning (BSP) to partition a room
        /// </summary>
        private List<Bounds> BinarySpacePartitioning(Bounds mainRoom, int minWidth, int minHeight)
        {
            Queue<Bounds> roomsQueue = new();
            List<Bounds> subRooms = new();

            roomsQueue.Enqueue(mainRoom);

            while (roomsQueue.Count > 0)
            {
                if (Random.value > _roomData.proabilityForSplit) break;

                Bounds room = roomsQueue.Dequeue();

                if (room.size.x < minWidth || room.size.y < minHeight) continue;

                if (Random.value < 0.5f)
                {
                    if (room.size.y >= 2 * minHeight) SplitRoomHorizontally(room, roomsQueue, minHeight);
                    else if (room.size.x >= 2 * minWidth) SplitRoomVertically(room, roomsQueue, minWidth);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) subRooms.Add(room);
                }
                else
                {
                    if (room.size.x >= 2 * minWidth) SplitRoomVertically(room, roomsQueue, minWidth);
                    else if (room.size.y >= 2 * minHeight) SplitRoomHorizontally(room, roomsQueue, minHeight);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) subRooms.Add(room);
                }

                subRooms.Add(room);
            }

            //for (int i = subRooms.Count - 1; i > -1; i--)
            //{
            //    if (Random.value >= _roomData.proabilityForSplit) subRooms.RemoveAt(i);
            //}

            return subRooms;
        }

        
        /// <summary>
        /// Adds a Wall to the Room Grid
        /// </summary>
        private void AddWallToGridX(int start, int end, int y)
        {
            if (_grid[Mathf.Clamp(start - 1, 0, _roomSize.x - 1), y] == RoomZone.Invalid || _grid[start, y] == RoomZone.Invalid) return;
            if (_grid[Mathf.Clamp(end, 0, _roomSize.x - 1), y] == RoomZone.Invalid || _grid[end, y] == RoomZone.Invalid) return;

            _subroomStart.Add(new(start - 1, y));

            // Add Main Wall Tiles To Grid
            for (int i = start; i <= end; i++)
            {
                if (y > 0 && _grid[i, y - 1] != RoomZone.Room)
                {
                    _subroomEnd.Add(new(i, y));
                    _subroomStart.Add(new(i + 1, y));
                }
                else if (y < _roomSize.y - 1 && _grid[i, y + 1] != RoomZone.Room)
                {
                    _subroomEnd.Add(new(i, y));
                    _subroomStart.Add(new(i + 1, y));
                }
                else if (_grid[i, y] == RoomZone.Room) _grid[i, y] = RoomZone.SubRoomWall;
             
            }

            _subroomEnd.Add(new(end, y));

            // Add Tile To Edge
            if (start == 1 && _grid[start, y - 1] == RoomZone.Room && _grid[start, y + 1] == RoomZone.Room) _grid[start - 1, y] = RoomZone.SubRoomWall;
            if (end == _roomSize.x - 1 && _grid[end - 1, y - 1] == RoomZone.Room && _grid[end - 1, y + 1] == RoomZone.Room) _grid[end, y] = RoomZone.SubRoomWall;
        }

        /// <summary>
        /// Adds a Wall to the Room Grid
        /// </summary>
        private void AddWallToGridY(int start, int end, int x)
        {
            if (_grid[x, Mathf.Clamp(start - 1, 0, _roomSize.y - 1)] == RoomZone.Invalid || _grid[x, start - 1] == RoomZone.Invalid) return;
            if (_grid[x, Mathf.Clamp(end + 1, 0, _roomSize.y - 1)] == RoomZone.Invalid || _grid[x, end] == RoomZone.Invalid) return;

            _subroomStart.Add(new(x, start - 1));

            // Add Main Wall Tiles To Grid
            for (int i = start; i <= end; i++)
            {
                if (x > 0 && _grid[x - 1, i] != RoomZone.Room)
                {
                    _subroomEnd.Add(new(x, i));
                    _subroomStart.Add(new(x, i + 1));
                }
                else if (x < _roomSize.x - 1 && _grid[x + 1, i] != RoomZone.Room)
                {
                    _subroomEnd.Add(new(x, i));
                    _subroomStart.Add(new(x, i + 1));
                }
                else if (_grid[x, i] == RoomZone.Room) _grid[x, i] = RoomZone.SubRoomWall; 
            }

            _subroomEnd.Add(new(x, end));

            // Add Tile To Edge
            if (start == 1 && _grid[x - 1, start] == RoomZone.Room && _grid[x + 1, start] == RoomZone.Room) _grid[x, start - 1] = RoomZone.SubRoomWall;
            if (end == _roomSize.y - 1 && _grid[x - 1, end - 1] == RoomZone.Room && _grid[x + 1, end - 1] == RoomZone.Room) _grid[x, end] = RoomZone.SubRoomWall;
        }

        private void AddSubroomsToGrid()
        {
            foreach (Bounds room in _subRooms)
            {
                if (room.size.y == _roomSize.y && room.size.x == _roomSize.x) continue;
                Vector3 min = new(room.min.x - _parnet.transform.position.x, 0, room.min.y - _parnet.transform.position.z);
                Vector3 max = new(room.max.x - _parnet.transform.position.x, 0, room.max.y - _parnet.transform.position.z);


                if (Mathf.FloorToInt(max.z + _roomSize.y / 2.0f) > 0) AddWallToGridX(Mathf.FloorToInt(min.x + _roomSize.x / 2.0f + 1), Mathf.FloorToInt(max.x + _roomSize.x / 2.0f - 1), Mathf.FloorToInt(min.z + _roomSize.y / 2.0f + 1));
                if (Mathf.FloorToInt(_roomSize.y) > Mathf.FloorToInt(max.z + _roomSize.y / 2.0f)) AddWallToGridX(Mathf.FloorToInt(min.x + _roomSize.x / 2.0f + 1), Mathf.FloorToInt(max.x + _roomSize.x / 2.0f - 1), Mathf.FloorToInt(max.z + _roomSize.y / 2.0f));
                if (Mathf.FloorToInt(min.x + _roomSize.x / 2.0f) > 0) AddWallToGridY(Mathf.FloorToInt(min.z + _roomSize.y / 2.0f + 1), Mathf.FloorToInt(max.z + _roomSize.y / 2.0f - 1), Mathf.FloorToInt(min.x + _roomSize.x / 2.0f + 1));
                if (Mathf.FloorToInt(_roomSize.x) > Mathf.FloorToInt(max.x + _roomSize.x / 2.0f)) AddWallToGridY(Mathf.FloorToInt(min.z + _roomSize.y / 2.0f + 1), Mathf.FloorToInt(max.z + _roomSize.y / 2.0f - 1), Mathf.FloorToInt(max.x + _roomSize.x / 2.0f));
            }
        }

        /// <summary>
        /// Generate sub rooms
        /// </summary>
        void CreateSubRooms()
        {
            //if (subRooms.Count <= 1) return;
            for (int j = 0; j < _subroomStart.Count; j++)
            {
                bool sideWall = _subroomStart[j].y == _subroomEnd[j].y;
                Vector2 size = _subroomEnd[j] - _subroomStart[j];

                if (size.x <= 1  && size.y <= 1) continue;

                Debug.Log("Start: " + _subroomStart[j] + " " + test++);
                Debug.Log("End: " + _subroomEnd[j] + " " + test++);

                _wallCount = new(Mathf.Max(1, Mathf.FloorToInt((size.x + 1) / _wallSize)), Mathf.Max(1, Mathf.FloorToInt((size.y + 1) / _wallSize)));
                Vector2 scale = new(((size.x + 1) / _wallCount.x) / _wallSize, ((size.y + 1) / _wallCount.y) / _wallSize);

                List<DungeonObject> subRoomObjects = new(_roomData.SubRoomWallObjects);
                subRoomObjects.AddRange(_roomData.SubRoomEnterenceObjects);

                for (int i = 0; i < _wallCount.x; i++)
                {
                    if (!sideWall) break;

                    Vector3 position = _parnet.transform.position + new Vector3(-_roomSize.x / 2.0f + _subroomStart[j].x + _wallSize * scale.x / 2 + i * scale.x * _wallSize, _parnet.transform.position.y, -_roomSize.y / 2.0f + _subroomStart[j].y);

                    Quaternion rotation = _parnet.transform.rotation;
                    Vector3 scaleVector = new(scale.x, 1, 1);

                    DungeonObject wallAsset = GetRandomAsset(subRoomObjects);

                    GameObject wall = GameObject.Instantiate(wallAsset.Prefab, position, rotation * Quaternion.Euler(0f, 180f, 0f), _mesh.transform);

                    wall.transform.localScale = scaleVector;

                    wall.transform.parent = _mesh.transform;
                }

                for (int i = 0; i < _wallCount.y; i++)
                {
                    if (sideWall) break;

                    Vector3 position = _parnet.transform.position + new Vector3(-_roomSize.x / 2.0f + _subroomStart[j].x, _parnet.transform.position.y, -_roomSize.y / 2.0f + _subroomStart[j].y + _wallSize * scale.y / 2f + i * scale.y * _wallSize);

                    Quaternion rotation = _parnet.transform.rotation;
                    Vector3 scaleVector = new(scale.y, 1, 1);

                    DungeonObject upWallAsset = GetRandomAsset(subRoomObjects);
                    DungeonObject downWallAsset = GetRandomAsset(subRoomObjects);

                    GameObject wall = GameObject.Instantiate(upWallAsset.Prefab, position, rotation * Quaternion.Euler(0f, 270f, 0f), _mesh.transform);

                    wall.transform.localScale = scaleVector;

                    wall.transform.parent = _mesh.transform;
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitRoom(GameObject parnet, Vector2Int roomSize, int numEnterences)
        {
            _roomSize = roomSize;
            _numEnterences = numEnterences;
            _parnet = parnet;
            _subroomStart = new();
            _subroomEnd = new();

            _grid = new RoomZone[_roomSize.x, _roomSize.y];

            for (int i = 0; i < _roomSize.x; i++)
            {
                for (int j = 0; j < _roomSize.y; j++)
                {
                    if (i == 0 || i == _roomSize.x - 1 || j == 0 || j == _roomSize.y - 1) _grid[i, j] = RoomZone.Edge;
                    else _grid[i, j] = RoomZone.Room;
                }
            }

            _mesh = new("mesh");
            _exits = new("exits");

            _mesh.transform.parent = _parnet.transform;
            _exits.transform.parent = _parnet.transform;

            _subRooms = BinarySpacePartitioning(
                new(
                    new Vector3(
                        _parnet.transform.position.x,
                        _parnet.transform.position.z,
                        0
                        ),
                    new Vector3(
                        _roomSize.x,
                        _roomSize.y, 
                        0
                        )
                    ),
                _roomData.MinSubRoomSize.x,
                _roomData.MinSubRoomSize.y
                );
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void GenerateRoom()
        {
            CreateWalls();
            CreateFloors();
            //AddSubroomsToGrid();
            //CreateSubRooms();
            CreatePillar();
        }
        
        /// <summary>
        /// 
        /// </summary>
        private List<Transform> GetRoomExits()
        {
            List<Transform> exits = new();
            foreach (Transform exit in _exits.transform) exits.Add(exit);
            return exits;
        }

        /// <summary>
        /// Creates a room
        /// </summary>
        public Room CreateRoom(GameObject parnet, Vector2Int roomSize, Vector3 tileSize, int numEnterences, int roomID)
        {
            Decorator decorator = new(Random.Range(-10000, 10000), _roomData.recipes[Random.Range(0, _roomData.recipes.Count)]);

            _wallSize = Mathf.Max
            (
                tileSize.x,
                tileSize.z
            );

            _floorSize = new Vector2
            (
                tileSize.x,
                tileSize.z
            );

            InitRoom(parnet, roomSize, numEnterences);

            GenerateRoom();

            Room room = new(_parnet, GetRoomExits(), roomID);
            room.SetSize(new(roomSize.x, 1, roomSize.y));
            room.SetGrid(_grid);

            decorator.DecorateRoom(ref room);

            //room.DrawZones();

            return room;
        }
    }
}
