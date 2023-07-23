using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using PaperSouls.Core;
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
        [SerializeField] private string _currentProfileName = "Development";

        private GameData _gameData;
        public GameData Data
        {
            get
            {
                return _gameData;
            }
            private set
            {
                _gameData = value;
            }
        }

        private List<IDataPersistence> _dataPersistencesObjects;
        private JSONHandler _jsonHandler;

        public int ProfileID {get; private set;}

        public void NewGame()
        {
            _gameData = new GameData();
        }

        public void LoadGame()
        {
            _gameData = _jsonHandler.Load(_currentProfileName);

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
            _gameData.ProfileID = ProfileID;

            if (_gameData == null) Debug.LogError("GameData is null. Please call NewGame() before SaveGame().");
            
            foreach (var obj in _dataPersistencesObjects) obj.SaveData(ref _gameData);

            _jsonHandler.Save(_gameData, _currentProfileName);
        }

        /// <summary>
        /// Return all GameObjects in the Scene with a IDataPersistence component.
        /// </summary>
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            //TODO: Find a way to get all objects with IDataPersistence without using FindObjectsOfType
            //      This is a very expensive operation.
            return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteGame(DeleteProfileMessage msg)
        {
            string path = Path.Combine(Application.dataPath, Application.persistentDataPath, msg.ProfileName);
            if (Directory.Exists(path)) Directory.Delete(path, true);
            else Debug.LogWarning($"Trying to delete path that does not exist:\n{path}"); ;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetProfileID(SetProfileIDMessage msg)
        {
            ProfileID = msg.ProfileID;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ChangeProfile(ChangeProfileMessage msg)
        {
            _currentProfileName = msg.ProfileName;
            LoadGame();
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, GameData> FetchAllProfiles()
        {
            return _jsonHandler.LoadAndGetAllProfiles();
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
        /// Add listeners
        /// </summary>
        private void AddListeners()
        {
            GameManager.AddListener<SetProfileIDMessage>(SetProfileID);
            GameManager.AddListener<ChangeProfileMessage>(ChangeProfile);
            GameManager.AddListener<DeleteProfileMessage>(DeleteGame);
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /// <summary>
        /// Removes the listeners
        /// </summary>
        private void RemoveListeners()
        {
            GameManager.RemoveListener<SetProfileIDMessage>(SetProfileID);
            GameManager.RemoveListener<ChangeProfileMessage>(ChangeProfile);
            GameManager.RemoveListener<DeleteProfileMessage>(DeleteGame);
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
