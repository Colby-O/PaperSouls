using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.Audio;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime
{
    internal sealed class PaperSoulsGameManager : GameManager
    {
        [Header("Holders")]
        [SerializeField] private GameObject _monoSystemParnet;
        [SerializeField] private GameObject _controllerParnet;

        [Header("MonoSystems")]
        [SerializeField] private AudioMonoSystem _audioMonoSystem;
        [SerializeField] private SceneMonoSystem _sceneMonoSystem;
        [SerializeField] public UIMonoSystem _uiMonoSystem;
        [SerializeField] private GameStateMonoSystem _gameStateMonoSystem;

        [Header("Databases")]
        [SerializeField] private ItemDatabase _itemDatabase;

        [Header("Global Variables")]
        [SerializeField] private GameStates IntialState = GameStates.MainMenu;
        private static GameObject _player = null;
        public static Vector3 StartPosition = Vector3.zero;

        public static bool AccpetPlayerInput { get; set; }
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

        public static void ResetGame()
        {
            Player.transform.position = PaperSoulsGameManager.StartPosition;
            GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Playing));
        }

        /// <summary>
        /// Attaches all MonoSystems to the GameManager
        /// </summary>
        private void AttachMonoSystems()
        {
            AddMonoSystem<AudioMonoSystem, IAudioMonoSystem>(_audioMonoSystem);
            //AddMonoSystem<SceneMonoSystem, ISceneMonoSystem>(_sceneMonoSystem);
            AddMonoSystem<UIMonoSystem, IUIMonoSystem>(_uiMonoSystem);
            AddMonoSystem<GameStateMonoSystem, IGameStateMonoSystem>(_gameStateMonoSystem);
        }

        protected override string GetApplicationName()
        {
            return nameof(PaperSoulsGameManager);
        }

        protected override void OnInitalized()
        {
            if (_player == null) _player = GameObject.Find("Player");

            StartPosition = _player.transform.position;

            // Ataches all MonoSystems to the GameManager
            AttachMonoSystems();

            // Initalzie all Databases
            _itemDatabase.SetItemIDs();

            // Ensures all MonoSystems call Awake at the same time.
            _monoSystemParnet.SetActive(true);
            _controllerParnet.SetActive(true);
        }

        private void Start()
        {
            AccpetPlayerInput = false;
            GameManager.Emit<ChangeGameStateMessage>(new(IntialState));
        }
    }
}
