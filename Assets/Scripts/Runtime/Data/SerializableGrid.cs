using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PaperSouls.Runtime.DungeonGeneration;

namespace PaperSouls.Runtime.Data
{
    [System.Serializable]
    internal sealed class SerializableGrid
    {
        [SerializeField] private List<TileType> _tiles;
        [SerializeField] private int _gridSize = 0;

        public SerializableGrid()
        {
            _tiles = null;
            _gridSize = 0;
        }

        public SerializableGrid(TileType[,] grid, int gridSize)
        {
            _gridSize = gridSize;
            _tiles = grid.Cast<TileType>().ToList();
        }

        public TileType[,] Deserialize()
        {
            if (_gridSize == 0) return null;

            TileType[,] grid = new TileType[_gridSize, _gridSize];

            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    grid[y, x] = _tiles[x + y * _gridSize];
                }
            }

            return grid;
        }

        public bool Empty() => _tiles == null || _gridSize == 0;
    }
}

