using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal enum Side : int
    {
        /// <summary>
        /// Right side of the room
        /// </summary>
        Right,
        /// <summary>
        /// Left side of the room
        /// </summary>
        Left,
        /// <summary>
        /// Top side of the room
        /// </summary>
        Top,
        /// <summary>
        /// Bottom side of the room
        /// </summary>
        Bottom
    }

    internal sealed class RoomGenerator
    {
        private int _seed;
        private int _numExits;
        private Vector3 _roomPosition;
        private Vector2Int _tileSize;
        private Vector2Int _roomSize;

        private Vector2Int _wallCount;
        private Vector2 _scale;

        private List<int> _exitsRight;
        private List<int> _exitsLeft;
        private List<int> _exitsTop;
        private List<int> _exitsBottom;

        public RoomGenerator(int seed, Vector2Int tileSize)
        {
            Random.InitState(seed);
            _seed = seed;
            _tileSize = tileSize;
        }

        /// <summary>
        /// Setup for the room generator to be call on a per room basis
        /// </summary>
        private void Initialization(Vector3 positon, Vector3 size, int numEnterences)
        {
            // Variable initialization
            _exitsRight = new();
            _exitsLeft = new();
            _exitsTop = new();
            _exitsBottom = new();

            _roomSize = new(Mathf.RoundToInt(size.x), Mathf.RoundToInt(size.z));
            _roomPosition = positon;

            // Number of wall tiles needed per side
            _wallCount = new(
                Mathf.Max(1, Mathf.RoundToInt(_roomSize.x / _tileSize.x)),
                Mathf.Max(1, Mathf.RoundToInt(_roomSize.y / _tileSize.y))
            );

            // Exits cannot be placed on cornors to avoid issues with the path finder
            int maxNumberOfExnterencesPossible = 2 * _wallCount.x + 2 * _wallCount.y - 8;
            _numExits = (numEnterences > maxNumberOfExnterencesPossible) ? maxNumberOfExnterencesPossible : numEnterences;

            // Computes the scale need for a wall tile
           _scale = new((_roomSize.x / _wallCount.x) / _tileSize.x, (_roomSize.y / _wallCount.y) / _tileSize.y);
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
        /// Computes the middle point for wall <paramref name="wallIndex"/> on <paramref name="side"/>.
        /// Returns room center position on fail.
        /// </summary>
        private Vector3 GetPositionOfWall(int wallIndex, Side side)
        {
            Vector3? relativePosition = side switch
            {
                Side.Right => new Vector3(
                                -_roomSize.x / 2f + (wallIndex + 0.5f) * _scale.x * _tileSize.x,
                                0,
                                _roomSize.y / 2f - _tileSize.y
                            ),
                Side.Left => new Vector3(
                                -_roomSize.x / 2f + (wallIndex + 0.5f) * _scale.x * _tileSize.x,
                                0,
                                -_roomSize.y / 2f
                            ),
                Side.Top => new Vector3(
                                -_roomSize.x / 2f,
                                0,
                                -_roomSize.y / 2f + (wallIndex + 0.5f) * _scale.y * _tileSize.y
                            ),
                Side.Bottom => new Vector3(
                                _roomSize.x / 2f - _tileSize.x,
                                0,
                                -_roomSize.y / 2f + (wallIndex + 0.5f) * _scale.y * _tileSize.y
                            ),
                _ => null
            };

            return _roomPosition + relativePosition ?? Vector3.zero;
        }

        /// <summary>
        /// Fetches the position of a random exit location on a random side.
        /// </summary>
        private bool GetExitLocation(out Vector3 position)
        {
            float val = Random.value;

            int location;
            if (val < 0.25f)
            {
                location = GetVaildExitLocation(1, _wallCount.x - 2, ref _exitsRight);
                position = GetPositionOfWall(location, Side.Right);
            }
            else if (val < 0.5f)
            {
                location = GetVaildExitLocation(1, _wallCount.x - 2, ref _exitsLeft);
                position = GetPositionOfWall(location, Side.Left);
            }
            else if (val < 0.75f)
            {
                location = GetVaildExitLocation(1, _wallCount.y - 2, ref _exitsTop);
                position = GetPositionOfWall(location, Side.Top);
            }
            else
            {
                location = GetVaildExitLocation(1, _wallCount.y - 2, ref _exitsBottom);
                position = GetPositionOfWall(location, Side.Bottom);
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
        /// Constructs a room object that can latter be used to generate a room.
        /// </summary>
        public Room GenerateRoomObject(Vector3 positon, Vector3 size, int numEnterences, int roomID)
        {
            Random.State state = Random.state;
            Initialization(positon, size, numEnterences);
            List<Vector3> exits = GenerateExitLocations();
            return new Room(positon, size, exits, state, roomID);
        }

        /// <summary>
        /// Generates a room from a Room object.
        /// </summary>
        public GameObject GenerateFromRoom(Room room) => GenerateFromRoom(room.ToSerializableRoom());

        /// <summary>
        /// Generates a room from a SerializableRoom object.
        /// </summary>
        public GameObject GenerateFromRoom(SerializableRoom room)
        {
            Random.state = room.State;
            return Generate(room.Position, room.Size, room.NumberOfExits, room.ID);
        }

        /// <summary>
        /// Generates a room from scratch.
        /// </summary>
        public GameObject Generate(Vector3 positon, Vector3 size, int numEnterences, int roomID)
        {
            Initialization(positon, size, numEnterences);
            List<Vector3> exits = GenerateExitLocations();
            return null;
        }
    }
}