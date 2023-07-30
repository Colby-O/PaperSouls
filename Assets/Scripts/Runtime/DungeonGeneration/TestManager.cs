using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.DungeonGeneration
{
    public class TestManager : MonoBehaviour
    {
        [SerializeField] private int _defaultSeed;
        [SerializeField] private DungeonProperties _data;
        [SerializeField] private Vector3 _tileSize = Vector3.zero;
        [SerializeField] private bool _regenerate = false;

        private DungeonGenerator _generator;
        private DungeonLoader _loader;

        private List<GameObject> cubes = new();

        /// <summary>
        /// Draw grid for debugging
        /// </summary>
        private void DrawGrid()
        {
            for (int i = 0; i < _generator._gridSize; i++)
            {
                for (int j = 0; j < _generator._gridSize; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(i * _tileSize.x * _data.GenerationProperties.Scale.x, 0, j * _tileSize.z * _data.GenerationProperties.Scale.z);
                    cube.transform.localScale = new Vector3(_tileSize.x * _data.GenerationProperties.Scale.x, _data.GenerationProperties.Scale.z, _tileSize.z * _data.GenerationProperties.Scale.z);

                    if (_generator._grid[i, j] == TileType.Room) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
                    else if (_generator._grid[i, j] == TileType.RoomSpacing) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.yellow);
                    else if (_generator._grid[i, j] == TileType.Empty) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);
                    else if (_generator._grid[i, j] == TileType.Hallway) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
                    else if (_generator._grid[i, j] == TileType.HallwayAndRoom) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.white);
                    else if (_generator._grid[i, j] == TileType.MainRoom) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.magenta);
                    else if (_generator._grid[i, j] == TileType.Invaild) cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
                    else cube.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.black);

                    cubes.Add(cube);
                }
            }
        }

        void Start()
        {
            _generator = new(_defaultSeed, _data, _tileSize);
            _loader = new(_data, _tileSize);

            Dungeon d = _generator.Generate();
            DrawGrid();
            _loader.Load(d);

            //Vector3 pos = d.RoomList[Random.Range(0, d.RoomList.Count)].Position;
            //pos.y += 0.5f;
            //GameObject.Find("Player").GetComponent<PlayerController>().TeleportTo(pos);
        }

        private void Update()
        {
            if(_regenerate)
            {
                foreach (var cube in cubes) Destroy(cube);

                _generator = new(_defaultSeed, _data, _tileSize);

                Dungeon d = _generator.Generate();
                //DrawGrid();
                _regenerate = false;
            }
        }
    }
}
