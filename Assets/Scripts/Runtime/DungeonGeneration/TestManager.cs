using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    internal sealed class TestManager : MonoBehaviour
    {
        private DungeonGenerator _dungeonGenerator;
        private DungeonLoader _dungeonLoader;

        public int _seed;
        public DungeonData _dungeonData;
        public Vector3 _tileSize;
        private TileType[,] _grid;

        void Start()
        {
            _dungeonGenerator = new(_seed, _dungeonData, _tileSize);
            _dungeonLoader = new(_dungeonData, _tileSize);
            Dungeon dungeon = _dungeonGenerator.Generate();
            _dungeonLoader.Load(dungeon);
        }
    }
}
