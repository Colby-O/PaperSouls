using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    public class RoomGenerator
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

        public RoomGenerator(RoomData roomData, int seed)
        {
            _seed = seed;
            _roomData = roomData;
            Random.InitState(_seed);
        }

        /// <summary>
        /// Fetches the generate the locations of the room exits
        /// </summary>
        List<int> GetExterenceLocations(Vector2 _wallCount)
        {
            List<int> enterenceIndices = new();

            if (_numEnterences > (int)(2 * _wallCount.x + 2 * _wallCount.y - 8)) _numEnterences = (int)(2 * _wallCount.x + 2 * _wallCount.y - 8);

            for (int i = 0; i < _numEnterences; i++)
            {
                int rand = Random.Range(-2 * (int)_wallCount.x, 2 * (int)_wallCount.y + 1);

                while (rand == 0 || rand == -1 || rand == -(int)_wallCount.x - 1 || rand == 1 || rand == _wallCount.y + 1 ||rand == -2 * (int)_wallCount.x || rand == -(int)_wallCount.x || rand == 2 * (int)_wallCount.y  || rand == (int)_wallCount.y || enterenceIndices.Contains(rand)) rand = Random.Range(-2 * (int)_wallCount.x, 2 * (int)_wallCount.y + 1);

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
            float xSplit = Random.Range(1, room.size.x);

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
            float ySplit = Random.Range(1, room.size.y);

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
            }

            for (int i = subRooms.Count - 1; i > -1; i--)
            {
                if (Random.value >= _roomData.proabilityForSplit) subRooms.RemoveAt(i);
            }

            return subRooms;
        }

        /*
        /// <summary>
        /// Adds a Wall to the Room Grid
        /// </summary>
        List<Vector2Int> AddWallToGrid(int start, int end)
        {
            List<Vector2Int> wallSegments = new();
            int cursor = -1;
            for (int i = start; i < end; i++)
            {
                if (_grid[i, i + 1] == RoomTileType.Empty && cursor == -1) cursor = i;
                else if (_grid[i, i + 1] == RoomTileType.Wall && cursor != -1)
                {
                    wallSegments.Add(new Vector2Int(cursor, i));
                    cursor = -1;
                }
                else if (i == end - 1 && _grid[i, i + 1] == RoomTileType.Empty && cursor != -1) wallSegments.Add(new Vector2Int(cursor, end));
            }

            return wallSegments;
        }
        */

        /// <summary>
        /// Generate sub rooms
        /// </summary>
        void CreateSubRooms(List<Bounds> subRooms)
        {
            foreach (Bounds room in subRooms)
            {
                Vector3 min = new(room.min.x, 0, room.min.y);
                Vector3 max = new(room.max.x, 0, room.max.y);

                Debug.DrawLine(max - new Vector3(0, 0, room.size.y), min, Color.green, 1);
                Debug.DrawLine(max - new Vector3(room.size.x, 0, 0), min, Color.green, 1);
                Debug.DrawLine(max, min + new Vector3(0, 0, room.size.y), Color.green, 1);
                Debug.DrawLine(max, min + new Vector3(room.size.x, 0, 0), Color.green, 1);

                _wallCount = new(Mathf.Max(1, Mathf.FloorToInt(room.size.x / _wallSize)), Mathf.Max(1, Mathf.FloorToInt(room.size.y / _wallSize)));
                Vector2 scale = new((room.size.x / _wallCount.x) / _wallSize, (room.size.y / _wallCount.y) / _wallSize);

                List<DungeonObject> subRoomObjects = new(_roomData.wallObjects);
                subRoomObjects.AddRange(_roomData.enterenceObjects);

                for (int i = 0; i < _wallCount.x; i++)
                {
                    Vector3 positionRight = new Vector3(room.center.x, 0, room.center.y) + new Vector3(-room.size.x / 2f + _wallSize * scale.x / 2 + i * scale.x * _wallSize, _parnet.transform.position.y, room.size.y / 2);
                    Vector3 positionLeft = new Vector3(room.center.x, 0, room.center.y) + new Vector3(-room.size.x / 2f + _wallSize * scale.x / 2 + i * scale.x * _wallSize, _parnet.transform.position.y, -room.size.y / 2);

                    Quaternion rotation = _parnet.transform.rotation;
                    Vector3 scaleVector = new(scale.x, 1, 1);

                    DungeonObject leftWallAsset = GetRandomAsset(subRoomObjects);
                    DungeonObject rightWallAsset = GetRandomAsset(subRoomObjects);

                    GameObject wallRight = GameObject.Instantiate(leftWallAsset.Prefab, positionRight, rotation * Quaternion.Euler(0f, 180f, 0f), _mesh.transform);
                    GameObject wallLeft = GameObject.Instantiate(rightWallAsset.Prefab, positionLeft, rotation * Quaternion.Euler(0f, 0f, 0f), _mesh.transform);

                    wallRight.transform.localScale = scaleVector;
                    wallLeft.transform.localScale = scaleVector;

                    if (Mathf.Abs(room.size.y / 2 + room.center.y - _roomSize.y / 2) >= 1) wallRight.transform.parent = _mesh.transform;
                    else GameObject.Destroy(wallRight);
                    if (Mathf.Abs(-room.size.y / 2 + room.center.y + _roomSize.y / 2) >= 1) wallLeft.transform.parent = _mesh.transform;
                    else GameObject.Destroy(wallLeft);
                }

                for (int i = 0; i < _wallCount.y; i++)
                {
                    Vector3 positionDown = new Vector3(room.center.x, 0, room.center.y) + new Vector3(room.size.x / 2f, _parnet.transform.position.y, -room.size.y / 2f + _wallSize * scale.y / 2f + i * scale.y * _wallSize);
                    Vector3 positionUp = new Vector3(room.center.x, 0, room.center.y) + new Vector3(-room.size.x / 2f, _parnet.transform.position.y, -room.size.y / 2f + _wallSize * scale.y / 2f + i * scale.y * _wallSize);

                    Quaternion rotation = _parnet.transform.rotation;
                    Vector3 scaleVector = new(scale.y, 1, 1);

                    DungeonObject upWallAsset = GetRandomAsset(subRoomObjects);
                    DungeonObject downWallAsset = GetRandomAsset(subRoomObjects);

                    GameObject wallDown = GameObject.Instantiate(upWallAsset.Prefab, positionDown, rotation * Quaternion.Euler(0f, 270f, 0f), _mesh.transform);
                    GameObject wallUp = GameObject.Instantiate(downWallAsset.Prefab, positionUp, rotation * Quaternion.Euler(0f, 90f, 0f), _mesh.transform);

                    wallDown.transform.localScale = scaleVector;
                    wallUp.transform.localScale = scaleVector;

                    if (Mathf.Abs(room.size.x / 2 + room.center.x - _roomSize.x / 2) >= 1) wallDown.transform.parent = _mesh.transform;
                    else GameObject.Destroy(wallDown);
                    if (Mathf.Abs(-room.size.x / 2 + room.center.x + _roomSize.x / 2) >= 1) wallUp.transform.parent = _mesh.transform;
                    else GameObject.Destroy(wallUp);
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
                _roomData.minSubRoomSize.x,
                _roomData.minSubRoomSize.y
                );
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void GenerateRoom()
        {
            CreateWalls();
            CreateFloors();
            CreateSubRooms(_subRooms);
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

            return room;
        }
    }
}
