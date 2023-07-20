using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.Audio;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems.DataPersistence;
using PaperSouls.Runtime.MonoSystems;
using PaperSouls.Runtime.UI.View;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime
{
    internal sealed class PaperSoulsGameManager : GameManager
    {
        [Header("Holders")]
        [SerializeField] private GameObject _monoSystemParnet;

        [Header("MonoSystems")]
        [SerializeField] private AudioMonoSystem _audioMonoSystem;
        [SerializeField] public UIMonoSystem _uiMonoSystem;
        [SerializeField] private GameStateMonoSystem _gameStateMonoSystem;
        [SerializeField] private DataPersistenceMonoSystem _dataPersistenceMonoSystem;

        [Header("Databases")]
        [SerializeField] private ItemDatabase _itemDatabase;

        [Header("Global Variables")]
        [SerializeField] private GameStates _intialState = GameStates.MainMenu;
        private static GameObject _player = null;
        public static bool AccpetPlayerInput { get; set; }
        public static int Seed { get; set; }
        public static GameObject Player { 
            get 
            { 
                return _player; 
            } 
            private set 
            { 
                _player = value; 
            } 
        }
        private bool _firstTimeRunning = false;

        private void StartGame(StartGameMessage msg) => StartCoroutine(StartGame());

        private void RestartGame(RestartGameMessage msg) => StartCoroutine(ResetGame());

        private void GotoMainMenu(GotoMainMenuMessage msg) => StartCoroutine(GotoMainMenu());

        /// <summary>
        /// 
        /// </summary>
        private IEnumerator StartGame()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("DungeonGenerationScene");

            _uiMonoSystem.Show<LoadingScreenView>();
            LoadingScreenView loadingScreen = _uiMonoSystem.GetCurrentView<LoadingScreenView>();
            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                if (loadingScreen != null) loadingScreen.IncreaseProgressBar(progress);
                yield return null;
            }
            _dataPersistenceMonoSystem.Data.Seed = Seed;
            if (_player == null) _player = GameObject.Find("Player");
            Emit<ResetViewMessage>(new());
            Emit<ChangeGameStateMessage>(new(GameStates.Playing));
        }

        /// <summary>
        /// Function to be ran once a game is reset i.e. the player dies.
        /// </summary>
        private IEnumerator ResetGame()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (_player == null) _player = GameObject.Find("Player");
            Emit<ResetViewMessage>(new());
            Emit<ChangeGameStateMessage>(new(GameStates.Playing));
        }

        private IEnumerator GotoMainMenu()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Emit<ResetViewMessage>(new());
            Emit<ChangeGameStateMessage>(new(GameStates.MainMenu));
        }

        private void AddListeners()
        {
            AddListener<StartGameMessage>(StartGame);
            AddListener<RestartGameMessage>(RestartGame);
            AddListener<GotoMainMenuMessage>(GotoMainMenu);
        }

        private void RemoveListeners()
        {
            RemoveListener<StartGameMessage>(StartGame);
            RemoveListener<RestartGameMessage>(RestartGame);
            RemoveListener<GotoMainMenuMessage>(GotoMainMenu);
        }

        /// <summary>
        /// Attaches all MonoSystems to the GameManager
        /// </summary>
        private void AttachMonoSystems()
        {
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioMonoSystem);
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiMonoSystem);
            AddMonoSystem<GameStateMonoSystem, IGameStateMonoSystem>(_gameStateMonoSystem);
            AddMonoSystem<DataPersistenceMonoSystem, IDataPersistenceMonoSystem>(_dataPersistenceMonoSystem);
        }

        protected override string GetApplicationName()
        {
            return nameof(PaperSoulsGameManager);
        }

        protected override void OnInitalized()
        {
            // Ataches all MonoSystems to the GameManager
            AttachMonoSystems();

            // Initalzie all Databases
            _itemDatabase.SetItemIDs();
            
            // Adds Event Listeners
            AddListeners();

            // Ensures all MonoSystems call Awake at the same time.
            _monoSystemParnet.SetActive(true);
        }

        private void OnDestroy() => RemoveListeners();

        private void Start()
        {
            if (_player == null) _player = GameObject.Find("Player");
        }

        private void Update()
        {
            // This is scuffed
            // The issue is that GameManager Start is ran before the MonoSystem's
            // Since the View Hides all Views on the start Menu is deactiavted.
            // TODO: Find a better solution to this problem. Might need to refactor the 
            // code base to be more carefull about Awake/Start order. 
            if (!_firstTimeRunning)
            {
                _firstTimeRunning = true;
                Emit<ChangeGameStateMessage>(new(_intialState));
            }
        }
    }
}
