using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal sealed class DungeonLoader
    {
        private DungeonProperties _dungeonProperties;
        private RoomGenerator _roomGenerator;

        private Vector2Int _tileSize;
        private TileType[,] _grid;
        private int _gridSize;
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

        public DungeonLoader(DungeonProperties dungeonProperties, Vector3 tileSize)
        {
            _dungeonProperties = dungeonProperties;
            _tileSize = new((int)tileSize.x, (int)tileSize.z);
        }

        /// <summary>
        /// Creates a copy of a Hallway with rotation y.
        /// </summary>
        private GameObject InstantiateHallwayPrefab(GameObject obj, float y)
        {
            return Object.Instantiate(obj, Vector3.zero, Quaternion.Euler(0f, y, 0f));
        }

        /// <summary>
        /// Creates Different Hallway vairents from a single rotation
        /// </summary>
        private void GenerateHallwayVarients()
        {
            _hallwayVarients = new()
            {
                null,
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.EnterenceHallway, 180f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.EnterenceHallway, 90f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.CurvedHallway, 90f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.EnterenceHallway, 0f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.StrightHallway, 0f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.CurvedHallway, 180f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.ThreeWayHallway, 0f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.EnterenceHallway, 270f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.CurvedHallway, 0f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.StrightHallway, 90f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.ThreeWayHallway, 270f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.CurvedHallway, 270f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.ThreeWayHallway, 180f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.ThreeWayHallway, 90f),
                InstantiateHallwayPrefab(_dungeonProperties.GenerationProperties.FourWayHallway, 0f)
            };

            // Hides each of the hallway vairents in the scene
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
                if (
                    tiles[dir.x + 1, dir.y + 1] == TileType.Hallway || 
                    tiles[dir.x + 1, dir.y + 1] == TileType.HallwayAndRoom ||
                    tiles[dir.x + 1, dir.y + 1] == TileType.Room ||
                    tiles[dir.x + 1, dir.y + 1] == TileType.MainRoom
                    ) key |= 1 << i;
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
                    if (pos.x + i < _gridSize && pos.y + j < _gridSize && pos.x + i >= 0 && pos.y + j >= 0) tiles[i + 1, j + 1] = _grid[pos.x + i, pos.y + j];
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

        /// <summary>
        /// Places the rooms
        /// </summary>
        private void GenerateRooms()
        {
            foreach (SerializableRoom room in _roomList)
            {
                GameObject roomObj = _roomGenerator.GenerateFromRoom(room).GameObject;
                roomObj.transform.parent = _dungeonHolder.transform;
                _dungeonObjects.Add(roomObj);
            }
        }

        /// <summary>
        /// Places the hallways
        /// </summary>
        private void GenerateHallways()
        {
            GenerateHallwayVarients();

            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    Vector2Int gridPos = new Vector2Int(i, j);
                    TileType[,] tiles = GetSurroundingTiles(gridPos);

                    int key = GetHallwayOrention(tiles);

                    if (_grid[i, j] == TileType.Hallway) CreateHallwayMesh(gridPos, key, i * _gridSize + j);
                }
            }
        }

        /// <summary>
        /// Unloads the current dungeon
        /// </summary>
        public void Unload()
        {
            foreach (GameObject obj in _dungeonObjects) Object.Destroy(obj);
        }

        /// <summary>
        /// Draw grid for debugging
        /// </summary>
        private void DrawGrid()
        {
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(i * _tileSize.x * _dungeonProperties.GenerationProperties.Scale.x, 0, j * _tileSize.y * _dungeonProperties.GenerationProperties.Scale.z);
                    cube.transform.localScale = new Vector3(_tileSize.x * _dungeonProperties.GenerationProperties.Scale.x, _dungeonProperties.GenerationProperties.Scale.z, _tileSize.y * _dungeonProperties.GenerationProperties.Scale.z);

                    if (_grid[i, j] == TileType.Room) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
                    else if (_grid[i, j] == TileType.RoomSpacing) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.yellow);
                    else if (_grid[i, j] == TileType.Empty) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
                    else if (_grid[i, j] == TileType.Hallway) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
                    else if (_grid[i, j] == TileType.HallwayAndRoom) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
                    else if (_grid[i, j] == TileType.MainRoom) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.magenta);
                    else if (_grid[i, j] == TileType.Invaild) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
                    else cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
                }
            }
        }

        /// <summary>
        /// Loads a dungeon given dungeon object
        /// </summary>
        public List<GameObject> Load(Dungeon dungeon)
        {
            _roomList = dungeon.RoomList;
            _grid = dungeon.Grid.Deserialize();
            _gridSize = dungeon.GridSize;
            _roomGenerator = new(_dungeonProperties.RoomData, dungeon.Seed, _tileSize);
            _dungeonHolder = new("Dungeon");
            _dungeonObjects = new();
            Random.InitState(dungeon.Seed);
            GenerateRooms();
            GenerateHallways();
            return _dungeonObjects;
        }
    }
}
