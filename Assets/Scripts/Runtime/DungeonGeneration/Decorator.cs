using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    public class Decorator
    {
        private Recipe _recipe;
        private int _seed;
        private List<DungeonObject> _dectorations;
        private RoomZone[,] _grid;
        private GameObject _parent;
        private Vector3 _roomPosition;
        private Vector3 _roomSize;


        public Decorator(int seed, Recipe recipe)
        {
            _seed = seed;
            _recipe = recipe;
            Random.InitState(_seed);
        }

        private float ZonePlacementChance(RoomZone zone)
        {
            float proability = zone switch
            {
                RoomZone.Room => _recipe.PlacementProbability.Room,
                RoomZone.Edge => _recipe.PlacementProbability.Edge,
                _ => 0.0f,
            };
            return proability;
        }

        private bool PickRandomDecorationAsset(RoomZone zone, out DecorationObject obj)
        {
            List<DecorationObject> possibleObjects = _recipe.Objects.Where(obj => obj.zone == zone).ToList();

            if (possibleObjects.Count > 0)
            {
                do
                {
                    obj = possibleObjects[Random.Range(0, possibleObjects.Count)];
                } while (obj.Proability < Random.value);

                return true;
            }

            obj = null;
            return false;
        }

        private void AddObjectToGrid(Vector2Int pos, Vector2 size, Quaternion rot)
        {
            for (int i = -Mathf.FloorToInt(size.x / 2); i < Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = -Mathf.FloorToInt(size.y / 2); j < Mathf.CeilToInt(size.y / 2); j++)
                {
                    //Debug.Log(pos.x + i);
                    _grid[pos.x + i, pos.y + j] = RoomZone.Invalid;
                }
            }
        }

        private bool IsOverlap(Vector2Int pos, Vector2 size, Quaternion rot)
        {
            for (int i = -Mathf.FloorToInt(size.x / 2); i < Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = -Mathf.FloorToInt(size.y / 2); j < Mathf.CeilToInt(size.y / 2); j++)
                {
                    if (_grid[pos.x + i, pos.y + j] == RoomZone.Invalid) return true;
                }
            }

            return false;
        }

        private bool IsInsideRoom(Vector2Int pos, Vector2 size, Quaternion rot)
        {
            for (int i = -Mathf.FloorToInt(size.x / 2); i < Mathf.CeilToInt(size.x / 2); i++)
            {
                for (int j = -Mathf.FloorToInt(size.y / 2); j < Mathf.CeilToInt(size.y / 2); j++)
                {
                    if (pos.x + i < 0 || pos.y + j < 0 || pos.x + i >= _roomSize.x || pos.y + j >= _roomSize.z) return false;
                }
            }
            return true;
        }

        private Quaternion GetRotation(Vector2Int pos)
        {
            if (pos.y == 0) return Quaternion.Euler(0, 0, 0);
            else if (pos.y == (int)(_roomSize.z - 1)) return Quaternion.Euler(0, 180, 0);
            else if (pos.x == 0) return Quaternion.Euler(0, 90, 0);
            else if (pos.x == (int)(_roomSize.x - 1)) return Quaternion.Euler(0, 270, 0);
            else
            {
                int rand = Random.Range(0, 4);
                float rot;
                if (rand == 0) rot = 0;
                else if (rand == 1) rot = 90;
                else if (rand == 2) rot = 180;
                else rot = 270;

                return Quaternion.Euler(0, rot, 0);
            }
        }

        public void PlaceObject(Vector2Int pos, RoomZone zone)
        {
            if (PickRandomDecorationAsset(zone, out DecorationObject objectToPlace))
            {
                if (objectToPlace == null) return;
                Quaternion rotation = GetRotation(pos);

                Vector2 size;
                if (rotation.eulerAngles.y == 90 || rotation.eulerAngles.y == 270) size = new(objectToPlace.Size.z, objectToPlace.Size.x);
                else size = new(objectToPlace.Size.x, objectToPlace.Size.z);

                Vector3 position = new Vector3(-_roomSize.x / 2f + pos.x + (((int)size.x) % 2 == 1 ? 0.5f : 0), 0, -_roomSize.z / 2f + pos.y + (((int)size.y) % 2 == 1 ? 0.5f : 0));

                if (IsInsideRoom(pos, new(size.x, size.y), rotation) && 
                    !IsOverlap(pos, new(size.x, size.y), rotation)
                    )
                {
                    AddObjectToGrid(pos, new(size.x, size.y), rotation);
                    GameObject decorationObj = GameObject.Instantiate(objectToPlace.Prefab, position, rotation);
                    decorationObj.transform.parent = _parent.transform;
                    decorationObj.transform.localScale = new(1, 1, 1);
                    DungeonObject decoration = new(decorationObj, _dectorations.Count);
                    _dectorations.Add(decoration);
                }
            }
        }

        public void DecorateRoom(ref Room room)
        {
            _dectorations = new();
            _grid = room.Grid.Clone() as RoomZone[,];
            _roomSize = room.Size;
            _roomPosition = room.Prefab.transform.position;
            _parent = new GameObject("dectorations");

            for (int i = 0; i < _roomSize.x; i++)
            {
                for (int j = 0; j < _roomSize.z; j++)
                {
                    RoomZone zone = _grid[i, j];
                    float placementProability = ZonePlacementChance(zone);

                    if (placementProability >= Random.value)
                    {
                        PlaceObject(new(i, j), zone);
                    }
                }
            }

            _parent.transform.parent = room.Prefab.transform;

            room.Decorations = _dectorations;
            room.SetGrid(_grid);
            //room.DrawZones();
        }
    }
}
