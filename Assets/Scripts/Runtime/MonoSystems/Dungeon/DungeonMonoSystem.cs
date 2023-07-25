using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.DungeonGeneration;
using PaperSouls.Runtime.MonoSystems.DataPersistence;
using System.Xml.Serialization;
using PaperSouls.Runtime.Data;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.MonoSystems.DungeonGeneration
{
    internal sealed class DungeonMonoSystem : MonoBehaviour, IDungeonMonoSystem, IDataPersistence
    {
        [SerializeField] private int _defaultSeed;
        [SerializeField] private DungeonData _data;
        [SerializeField] private Vector3 _tileSize = Vector3.zero;
        [SerializeField] private int _rendererDistance;

        private DungeonGenerator _generator;
        private DungeonLoader _loader;

        private Dungeon _dungeon;
        private List<GameObject> _dungeonObjects;

        private void GenerateDungeon(GenerateDungeonMessage msg)
        {
            _generator = new(msg.Seed, _data, _tileSize);
            _dungeon = _generator.Generate();
        }

        private void LoadDungeon(LoadDungeonMessage msg)
        {
            _loader = new(_data, _tileSize);

            if (_dungeon.Grid.Deserialize() == null || _dungeon.RoomList.Count == 0)
            {
                Debug.LogWarning($"No Dungeon Saved! Generating new dungeon with default seed {_defaultSeed}.");
                _generator = new(_defaultSeed, _data, _tileSize);
                _dungeon = _generator.Generate();
            }
            _dungeonObjects = _loader.Load(_dungeon);
        }

        private void DestoryDungeon(DestoryDungeonMessage msg)
        {
            if (_loader != null) _loader.Unload();
        }

        private void StartChunkLoader(StartChunkLoadingMessage msg) => StartCoroutine(nameof(CheckActivation));
        private void StopChunkLoader(StopChunkLoadingMessage msg)
        {
            StopCoroutine(nameof(CheckActivation));
            foreach (GameObject obj in _dungeonObjects) obj.SetActive(true);
        }

        private void AddListeners()
        {
            GameManager.AddListener<GenerateDungeonMessage>(GenerateDungeon);
            GameManager.AddListener<LoadDungeonMessage>(LoadDungeon);
            GameManager.AddListener<DestoryDungeonMessage>(DestoryDungeon);
            GameManager.AddListener<StartChunkLoadingMessage>(StartChunkLoader);
            GameManager.AddListener<StopChunkLoadingMessage>(StopChunkLoader);
        }

        private void RemoveListeners()
        {
            GameManager.RemoveListener<GenerateDungeonMessage>(GenerateDungeon);
            GameManager.RemoveListener<LoadDungeonMessage>(LoadDungeon);
            GameManager.RemoveListener<DestoryDungeonMessage>(DestoryDungeon);
            GameManager.RemoveListener<StartChunkLoadingMessage>(StartChunkLoader);
            GameManager.RemoveListener<StopChunkLoadingMessage>(StopChunkLoader);
        }

        public bool TeleportTo(int id)
        {
            if (_dungeon == null)
            {
                Debug.LogError("Dungeon is null!");
            }
            else if (PaperSoulsGameManager.Player == null)
            {
                Debug.LogError("Player is null!");
                return false;
            }
            else
            {
                PlayerController player = PaperSoulsGameManager.Player.GetComponent<PlayerController>();
                if (player != null)
                {
                    SerializableRoom room = _dungeon.RoomList.Find(room => room.ID == id);
                    if (room == null) return false;
                    Vector3 roomPosition = _dungeon.RoomList.Find(room => room.ID == id).Position;
                    roomPosition.y += 0.5f;
                    player.TeleportTo(roomPosition);
                    return true;
                }
            }

            return false;
        }

        public string DungeonToString()
        {
            if (_dungeon == null)
            {
                Debug.LogError("Dungeon is null!");
                return string.Empty;
            }

            string res = string.Empty;
            res += " Dungeon Room List\n";

            foreach (SerializableRoom room in _dungeon.RoomList)
            {
                res += " |---------------------------------------------------------------------------------------------------------|\n";
                res += " " + room.ToString() + "\n";
            }
            res += " |---------------------------------------------------------------------------------------------------------|";
            return res;
        }

        public bool SaveData(ref GameData data)
        {
            data.Dungeon = _dungeon;
            return true;
        }

        public bool LoadData(GameData data)
        {
            if (data.Dungeon != null) _dungeon = data.Dungeon;
            return true;
        }

        IEnumerator CheckActivation()
        {
            List<GameObject> objectToRemove = new();

            // Deactivates the object far away from the player
            if (PaperSoulsGameManager.Player != null && _dungeonObjects != null && _dungeonObjects.Count > 0)
            {
                foreach (GameObject obj in _dungeonObjects)
                {
                    if (Vector3.Distance(PaperSoulsGameManager.Player.transform.position, obj.transform.position) > _rendererDistance)
                    {
                        if (obj == null) objectToRemove.Add(obj);
                        else obj.SetActive(false);
                    }
                    else
                    {
                        if (obj == null) objectToRemove.Add(obj);
                        else obj.SetActive(true);
                    }
                }
            }

            yield return new WaitForSeconds(0.01f);

            // Check if any dungeon objects were deleted for whatever reason
            if (objectToRemove.Count > 0)
            {
                foreach (GameObject obj in objectToRemove)
                {
                    _dungeonObjects.Remove(obj);
                }
            }

            yield return new WaitForSeconds(0.01f);

            StartCoroutine(nameof(CheckActivation));
        }


        private void OnDestroy()
        {
            RemoveListeners();
        }
        private void Awake()
        {
            AddListeners();
        }
    }
}
