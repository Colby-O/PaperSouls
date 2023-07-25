using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal sealed class DungeonLoader
    {
        private DungeonData _dungeonData;
        private RoomGenerator _roomGenerator;

        private Vector2Int _tileSize;
        private TileType[,] _grid;
        private List<SerializableRoom> _roomList;

        List<GameObject> _hallwayVarients;
        private GameObject _dungeonHolder;

        List<GameObject> _dungeonObjects;

        private static readonly Vector2Int[] DIRECTIONS = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        public DungeonLoader(DungeonData dungeonData, Vector3 tileSize)
        {
            _dungeonData = dungeonData;
            _tileSize = new((int)tileSize.x, (int)tileSize.z);
        }

        private GameObject InstantiateHallwayPrefab(GameObject obj, float y)
        {
            return Object.Instantiate(obj, Vector3.zero, Quaternion.Euler(0f, y, 0f));
        }

        private void GenerateHallwayVarients()
        {
            _hallwayVarients = new()
            {
                null,
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.EnterenceHallway, 180f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.EnterenceHallway, 90f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.CurvedHallway, 90f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.EnterenceHallway, 0f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.StrightHallway, 0f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.CurvedHallway, 180f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.ThreeWayHallway, 0f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.EnterenceHallway, 270f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.CurvedHallway, 0f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.StrightHallway, 90f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.ThreeWayHallway, 270f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.CurvedHallway, 270f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.ThreeWayHallway, 180f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.ThreeWayHallway, 90f),
                InstantiateHallwayPrefab(_dungeonData.DungeonProperties.FourWayHallway, 0f)
            };

            foreach (GameObject hallway in _hallwayVarients)
            {
                if (hallway != null)
                {
                    hallway.transform.parent = _dungeonHolder.transform;
                    hallway.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Finds the hallway orention needed to be placed
        /// </summary>
        private int GetHallwayOrention(TileType[,] tiles)
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
        private TileType[,] GetSurroundingTiles(Vector2Int pos)
        {
            TileType[,] tiles = new TileType[3, 3];

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (pos.x + i < _dungeonData.DungeonProperties.GridSize && pos.y + j < _dungeonData.DungeonProperties.GridSize && pos.x + i >= 0 && pos.y + j >= 0) tiles[i + 1, j + 1] = _grid[pos.x + i, pos.y + j];
                    else tiles[i + 1, j + 1] = TileType.Empty;
                }
            }

            return tiles;
        }

        /// <summary>
        /// Instantiates A Hallway Tile
        /// </summary>
        private GameObject InstantiateDungeonHallwayObject(GameObject prefab, Vector3 roomPosition, Vector3 roomSize, int hallwayID)
        {
            GameObject newObject = Object.Instantiate(prefab, roomPosition, prefab.transform.localRotation);
            newObject.transform.localScale = roomSize;
            newObject.name = "Hallway_" + hallwayID.ToString();
            newObject.transform.parent = _dungeonHolder.transform;

            return newObject;
        }

        /// <summary>
        /// Generates the hallway mesh
        /// </summary>
        private void CreateHallwayMesh(Vector2Int gridPos, int key, int hallwayID)
        {
            GameObject hallway = _hallwayVarients[key];

            Vector3 position = new Vector3(gridPos.x, 0, gridPos.y) * _tileSize.x;
            Vector3 scale = new Vector3(1, 1, 1);

            GameObject hallwayObject = InstantiateDungeonHallwayObject(hallway, position, scale, hallwayID);
            hallwayObject.SetActive(true);

            _dungeonObjects.Add(hallwayObject);
        }

        private void GenerateRooms()
        {
            foreach (SerializableRoom room in _roomList)
            {
                GameObject roomObj = _roomGenerator.GenerateFromRoom(room, false).GameObject;
                roomObj.transform.parent = _dungeonHolder.transform;
                _dungeonObjects.Add(roomObj);
            }
        }

        private void GenerateHallways()
        {
            GenerateHallwayVarients();

            for (int i = 0; i < _dungeonData.DungeonProperties.GridSize; i++)
            {
                for (int j = 0; j < _dungeonData.DungeonProperties.GridSize; j++)
                {
                    Vector2Int gridPos = new Vector2Int(i, j);
                    TileType[,] tiles = GetSurroundingTiles(gridPos);

                    int key = GetHallwayOrention(tiles);

                    if (_grid[i, j] == TileType.Hallway) CreateHallwayMesh(gridPos, key, i * _dungeonData.DungeonProperties.GridSize + j);
                }
            }
        }

        public void Unload()
        {
            foreach (GameObject obj in _dungeonObjects) Object.Destroy(obj);
        }

        public List<GameObject> Load(Dungeon dungeon)
        {
            _roomList = dungeon.RoomList;
            _grid = dungeon.Grid.Deserialize();
            _roomGenerator = new(_dungeonData.RoomData, dungeon.Seed, _tileSize);
            _dungeonHolder = new("Dungeon");
            _dungeonObjects = new();
            Random.InitState(dungeon.Seed);
            GenerateRooms();
            GenerateHallways();
            return _dungeonObjects;
        }
    }
}
