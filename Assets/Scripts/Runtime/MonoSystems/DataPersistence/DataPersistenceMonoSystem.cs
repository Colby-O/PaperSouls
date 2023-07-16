using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Helpers;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.MonoSystems.DataPersistence
{
    internal sealed class DataPersistenceMonoSystem : MonoBehaviour, IDataPersistenceMonoSystem
    {
        [Header("Config")]
        [SerializeField] private string _filename;
        [SerializeField] private bool _createNewGameIfNull = true;
        [SerializeField] private bool _useEncryption;
        [SerializeField] private string _currentProfileID = "Development";

        private GameData _gameData;

        private List<IDataPersistence> _dataPersistencesObjects;
        private JSONHandler _jsonHandler;

        public void NewGame()
        {
            _gameData = new GameData();
        }

        public void LoadGame()
        {
            _gameData = _jsonHandler.Load(_currentProfileID);

            if (_gameData == null && _createNewGameIfNull)
            {
                NewGame();
            }
            else if (_gameData == null)
            {
                return;
            }

            foreach (var obj in _dataPersistencesObjects) obj.LoadData(_gameData);
            
        }

        public void SaveGame()
        {
            if (_gameData == null) Debug.LogError("GameData is null. Please call NewGame() before SaveGame().");
            

            foreach (var obj in _dataPersistencesObjects) obj.SaveData(ref _gameData);

            _jsonHandler.Save(_gameData, _currentProfileID);
        }

        /// <summary>
        /// Return all GameObjects in the Scene with a IDataPersistence component.
        /// </summary>
        /// <returns></returns>
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            //TODO: Find a way to get all objects with IDataPersistence without using FindObjectsOfType
            //      This is a very expensive operation.
            return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        }

        /// <summary>
        /// Runs when a new Scene is loaded
        /// </summary>
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _dataPersistencesObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }

        /// <summary>
        /// Runs when a new Scene is unloaded
        /// </summary>
        public void OnSceneUnloaded(Scene scene)
        {
            SaveGame();
        }

        /// <summary>
        /// Add listeners to SceneMangaer
        /// </summary>
        private void AddListeners()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /// <summary>
        /// Removes the listeners to SceneMangaer
        /// </summary>
        private void RemoveListeners()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void Awake()
        {
            _jsonHandler = new JSONHandler(Application.persistentDataPath, _filename, _useEncryption);
            _dataPersistencesObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
    }
}
