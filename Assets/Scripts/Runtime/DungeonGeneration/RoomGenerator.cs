using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;
using Unity.Transforms;
using Unity.Entities.UniversalDelegates;
using UnityEngine.UIElements;
using System.Drawing;
using Unity.Entities;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal sealed class RoomGenerator
    {
        private RoomData _roomData;
        private RoomZone[,] _grid;
        private List<Bounds> _subRooms;

        private int _numExits;
        private Vector3 _roomPosition;
        private Vector2Int _tileSize;
        private Vector2Int _roomSize;

        private GameObject _roomHolder;
        private GameObject _mesh;

        private Vector2Int _tileCount;
        private Vector2 _scale;

        private List<int> _exitsRight;
        private List<int> _exitsLeft;
        private List<int> _exitsTop;
        private List<int> _exitsBottom;

        public RoomGenerator(RoomData roomData, int seed, Vector2Int tileSize)
        {
            Random.InitState(seed);
            _tileSize = tileSize;
            _roomData = roomData;
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

            return subRooms;
        }

        /// <summary>
        /// Setup for the room generator to be call on a per room basis
        /// </summary>
        private void Initialization(Vector3 positon, Vector3 size, int numEnterences = -1)
        {
            _roomSize = new(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.z));
            _roomPosition = positon;

            _grid = new RoomZone[_roomSize.x - _tileSize.x, _roomSize.y - _tileSize.y];

            for (int i = 0; i < _roomSize.x - _tileSize.x; i++)
            {
                for (int j = 0; j < _roomSize.y - _tileSize.y; j++)
                {
                    if (i == 0 || i == _roomSize.x - _tileSize.x - 1 || j == 0 || j == _roomSize.y - _tileSize.y - 1) _grid[i, j] = RoomZone.Edge;
                    else _grid[i, j] = RoomZone.Room;
                }
            }

            // Number of wall tiles needed per RoomSide
            _tileCount = new(
                Mathf.Max(1, Mathf.RoundToInt(_roomSize.x / _tileSize.x)),
                Mathf.Max(1, Mathf.RoundToInt(_roomSize.y / _tileSize.y))
            );

            // Computes the scale need for a wall tile
            _scale = new((_roomSize.x / _tileCount.x) / _tileSize.x, (_roomSize.y / _tileCount.y) / _tileSize.y);

            // Exits cannot be placed on cornors to avoid issues with the path finder
            int maxNumberOfExnterencesPossible = 2 * _tileCount.x + 2 * _tileCount.y - 8;
            _numExits = (numEnterences > maxNumberOfExnterencesPossible) ? maxNumberOfExnterencesPossible : numEnterences;

            _subRooms = BinarySpacePartitioning(
                new(
                    new Vector3(
                        _roomPosition.x,
                        _roomPosition.z,
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

        private void InitializeHolders(int id)
        {
            _roomHolder = new($"Room_{id}");
            _mesh = new("Mesh");
            _mesh.transform.parent = _roomHolder.transform;
        }

        /// <summary>
        /// Find an exit location that was not already used if possible. Otherwise returns -1.
        /// </summary>
        private int GetVaildExitLocation(int min, int max, ref List<int> used)
        {
            if (max - min <= used.Count) return -1;

            int exitLocation;
            do
            {
                exitLocation = Random.Range(min, max);

            } while (used.Contains(exitLocation));

            used.Add(exitLocation);

            return exitLocation;
        }

        /// <summary>
        /// Computes the middle point for wall <paramref name="wallIndex"/> on <paramref name="RoomSide"/>.
        /// Returns room center position on fail.
        /// </summary>
        private Vector3 GetPositionOfWall(int wallIndex, RoomSide RoomSide)
        {
            Vector3? relativePosition = RoomSide switch
            {
                RoomSide.Right => new Vector3(
                                -_roomSize.x / 2f + (wallIndex + 0.5f) * _scale.x * _tileSize.x,
                                0,
                                _roomSize.y / 2f - _tileSize.y
                            ),
                RoomSide.Left => new Vector3(
                                -_roomSize.x / 2f + (wallIndex + 0.5f) * _scale.x * _tileSize.x,
                                0,
                                -_roomSize.y / 2f
                            ),
                RoomSide.Top => new Vector3(
                                -_roomSize.x / 2f,
                                0,
                                -_roomSize.y / 2f + (wallIndex + 0.5f) * _scale.y * _tileSize.y
                            ),
                RoomSide.Bottom => new Vector3(
                                _roomSize.x / 2f - _tileSize.x,
                                0,
                                -_roomSize.y / 2f + (wallIndex + 0.5f) * _scale.y * _tileSize.y
                            ),
                _ => null
            };

            return relativePosition ?? Vector3.zero;
        }

        /// <summary>
        /// Fetches the position of a random exit location on a random RoomSide.
        /// </summary>
        private bool GetExitLocation(out Vector3 position)
        {
            float val = Random.value;

            int location;
            if (val < 0.25f)
            {
                location = GetVaildExitLocation(1, _tileCount.x - 2, ref _exitsRight);
                position = GetPositionOfWall(location, RoomSide.Right);
            }
            else if (val < 0.5f)
            {
                location = GetVaildExitLocation(1, _tileCount.x - 2, ref _exitsLeft);
                position = GetPositionOfWall(location, RoomSide.Left);
            }
            else if (val < 0.75f)
            {
                location = GetVaildExitLocation(1, _tileCount.y - 2, ref _exitsTop);
                position = GetPositionOfWall(location, RoomSide.Top);
            }
            else
            {
                location = GetVaildExitLocation(1, _tileCount.y - 2, ref _exitsBottom);
                position = GetPositionOfWall(location, RoomSide.Bottom);
            }

            return location != -1;
        }

        /// <summary>
        /// Generates all exits for a room.
        /// </summary>
        private List<Vector3> GenerateExitLocations()
        {
            List<Vector3> exits = new();

            while (exits.Count < _numExits) if (GetExitLocation(out Vector3 position)) exits.Add(position);

            return exits;
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

        private float GetRoomSideRotation(RoomSide RoomSide)
        {
            return RoomSide switch
            {
                RoomSide.Left => 0.0f,
                RoomSide.Top => 90.0f,
                RoomSide.Right => 180.0f,
                RoomSide.Bottom => 270.0f,
                _ => 0.0f
            };
        }

        private Vector3 GetRoomSideScale(RoomSide RoomSide)
        {
            if (RoomSide == RoomSide.Left || RoomSide == RoomSide.Right) return new(_scale.x, 1, 1);
            else return new(_scale.y, 1, 1);
        }

        private List<int> GetExitsForRoomSide(RoomSide RoomSide)
        {
            return RoomSide switch
            {
                RoomSide.Left => _exitsLeft,
                RoomSide.Top => _exitsTop,
                RoomSide.Right => _exitsRight,
                RoomSide.Bottom => _exitsBottom,
                _ => null
            };
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

        private void AddEnterenceToGrid(int wallIndex, RoomSide RoomSide)
        {
            switch (RoomSide)
            {
                case RoomSide.Right:
                    AddEnterenceToGrid(
                        new Vector2Int(wallIndex * Mathf.RoundToInt(_scale.x * _tileSize.x), _roomSize.y - _tileSize.y - 1),
                        new Vector2Int((int)(_scale.x * _tileSize.x), 1)
                    );
                    break;
                case RoomSide.Bottom:
                    AddEnterenceToGrid(
                        new Vector2Int(_roomSize.x - _tileSize.x - 1, wallIndex * Mathf.RoundToInt(_scale.y * _tileSize.y)),
                        new Vector2Int(1, (int)(_scale.y * _tileSize.y))
                    );
                    break;
                case RoomSide.Left:
                    AddEnterenceToGrid(new Vector2Int(
                        wallIndex * Mathf.RoundToInt(_scale.x * _tileSize.x), 0),
                        new Vector2Int((int)(_scale.x * _tileSize.x),
                        1)
                    );
                    break;
                case RoomSide.Top:
                    AddEnterenceToGrid(
                        new Vector2Int(0, wallIndex * Mathf.RoundToInt(_scale.y * _tileSize.y)),
                        new Vector2Int(1, (int)(_scale.y * _tileSize.y))
                    );
                    break;
            }
        }

        private void PlaceWallOnRoomSide(int wallIndex, RoomSide RoomSide)
        {
            Vector3 position = GetPositionOfWall(wallIndex, RoomSide);
            float rot = GetRoomSideRotation(RoomSide);
            List<int> exitsLocations = GetExitsForRoomSide(RoomSide);

            bool isExit = exitsLocations.Contains(wallIndex);
            bool isOpen = exitsLocations.Contains(-wallIndex);

            if (isOpen)
            {
                AddEnterenceToGrid(wallIndex, RoomSide);
                return;
            }

            DungeonObject asset = (isExit) ? GetRandomAsset(_roomData.enterenceObjects) : GetRandomAsset(_roomData.wallObjects);

            if (isExit) AddEnterenceToGrid(wallIndex, RoomSide);

            GameObject wall = Object.Instantiate(asset.GameObject, position, Quaternion.Euler(0f, rot, 0f), _mesh.transform);

            wall.transform.localScale = GetRoomSideScale(RoomSide);
        }

        /// <summary>
        /// Create dungeon walls
        /// </summary>
        private void CreateWalls()
        {
            for (int i = 0; i < _tileCount.x - 1; i++)
            {
                PlaceWallOnRoomSide(i, RoomSide.Left);
                PlaceWallOnRoomSide(i, RoomSide.Right);
            }

            for (int i = 0; i < _tileCount.y - 1; i++)
            {
                PlaceWallOnRoomSide(i, RoomSide.Top);
                PlaceWallOnRoomSide(i, RoomSide.Bottom);
            }
        }

        private float GetRandomRotation()
        {
            float rotate;
            int rand = Random.Range(0, 4);
            if (rand == 0) rotate = 0;
            else if (rand == 1) rotate = 90;
            else if (rand == 2) rotate = 180;
            else rotate = 270;

            return rotate;
        }

        /// <summary>
        /// Create dungeon floors
        /// </summary>
        private void CreateFloors()
        {
            for (int i = 0; i < _tileCount.x - 1; i++)
            {
                for (int j = 0; j < _tileCount.y - 1; j++)
                {
                    Vector3 position = new Vector3(
                        -_roomSize.x / 2f + _tileSize.x * _scale.x / 2 + i * _scale.x * _tileSize.x,
                        0,
                        -_roomSize.y / 2f + _tileSize.y * _scale.y / 2f + j * _scale.y * _tileSize.y
                    );

                    int rand = Random.Range(0, 4);
                    float rotate;
                    if (!_roomData.useRandomFloorRotation) rotate = 0;
                    else rotate = GetRandomRotation();

                    Vector3 scaleVector;
                    if (rotate == 0 || rotate == 180) scaleVector = new(_scale.x, 1, _scale.y);
                    else scaleVector = new(_scale.y, 1, _scale.x);

                    DungeonObject floorObject = GetRandomAsset(_roomData.floorObjects);

                    GameObject floor = Object.Instantiate(floorObject.GameObject, position, Quaternion.Euler(0f, rotate, 0f), _mesh.transform);
                    floor.transform.localScale = scaleVector;
                    floor.transform.parent = _mesh.transform;
                }
            }
        }

        /// <summary>
        /// Generates the GameObject for a room
        /// </summary>
        private void GenerateRoom()
        {
            CreateWalls();
            CreateFloors();
            // Not Working
            //AddSubroomsToGrid();
            //GenerateSubRooms()
        }

        private Room GenerateFromRoom(Vector3 positon, Vector3 size, int id, bool drawGrid = false)
        {
            Decorator decorator = new(Random.Range(-10000, 10000), _roomData.Recipes[Random.Range(0, _roomData.Recipes.Count)]);
            Initialization(positon, size);
            InitializeHolders(id);
            GenerateRoom();

            _roomHolder.transform.position = _roomPosition;
            Room room = new Room(positon, size, new(), Random.state, id);
            room.GameObject = _roomHolder;
            room.Position = _roomPosition - new Vector3(_tileSize.x / 2.0f, 0, _tileSize.y / 2.0f);
            room.Grid = _grid;
            room.GridSize = new(_roomSize.x - _tileSize.x, _roomSize.y - _tileSize.y);
            room.LeftExits = _exitsLeft;
            room.RightExits = _exitsRight;
            room.TopExits = _exitsTop;
            room.BottomExits = _exitsBottom;

            decorator.DecorateRoom(ref room);

            if (drawGrid) room.DrawGrid();

            return room;
        }

        /// <summary>
        /// Constructs a room object that can latter be used to generate a room.
        /// </summary>
        public Room GenerateRoomObject(Vector3 positon, Vector3 size, int numEnterences, int id)
        {
            _exitsRight = new();
            _exitsLeft = new();
            _exitsTop = new();
            _exitsBottom = new();
            Random.State state = Random.state;
            Initialization(positon, size, numEnterences);
            List<Vector3> exits = GenerateExitLocations();
            for (int i = 0; i < exits.Count; i++) exits[i] += _roomPosition;
            Room room = new Room(positon, size, exits, state, id);
            room.LeftExits = _exitsLeft;
            room.RightExits = _exitsRight;
            room.TopExits = _exitsTop;
            room.BottomExits = _exitsBottom;
            return room;
        }

        /// <summary>
        /// Generates a room from a Room object.
        /// </summary>
        public Room GenerateFromRoom(Room room, bool drawGrid = false) => GenerateFromRoom(room.ToSerializableRoom(), drawGrid);

        /// <summary>
        /// Generates a room from a SerializableRoom object.
        /// </summary>
        public Room GenerateFromRoom(SerializableRoom room, bool drawGrid = false)
        {
            Random.state = room.State;
            _exitsLeft = room.LeftExits;
            _exitsRight = room.RightExits;
            _exitsTop = room.TopExits;
            _exitsBottom = room.BottomExits;
            return GenerateFromRoom(room.Position, room.Size, room.ID, drawGrid);
        }

        /// <summary>
        /// Generates a room from scratch.
        /// </summary>
        public Room Generate(Vector3 positon, Vector3 size, int numEnterences, int id, bool drawGrid = false)
        {
            Room room = GenerateRoomObject(positon, size, numEnterences, id);

            Decorator decorator = new(Random.Range(-10000, 10000), _roomData.Recipes[Random.Range(0, _roomData.Recipes.Count)]);

            InitializeHolders(id);
            GenerateRoom();

            _roomHolder.transform.position = _roomPosition;
            room.GameObject = _roomHolder;
            room.Position = _roomPosition - new Vector3(_tileSize.x / 2.0f, 0, _tileSize.y / 2.0f);
            room.Grid = _grid;
            room.GridSize = new(_roomSize.x - _tileSize.x, _roomSize.y - _tileSize.y);
            room.LeftExits = _exitsLeft;
            room.RightExits = _exitsRight;
            room.TopExits = _exitsTop;
            room.BottomExits = _exitsBottom;

            decorator.DecorateRoom(ref room);

            if (drawGrid) room.DrawGrid();

            return room;
        }
    }
}