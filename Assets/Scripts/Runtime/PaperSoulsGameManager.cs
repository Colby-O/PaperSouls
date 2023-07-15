using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.Audio;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems;
using PaperSouls.Runtime.Items;

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

        // Should be some where else but didn't want to think about this right now
        public static bool AccpetPlayerInput { get; set; }
        public static GameObject Player { get; private set; }

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
            // Again shouldn't be here...
            AccpetPlayerInput = true;
            _gameStateMonoSystem.ChangeToPlayingState();
        }
 
        private void Update()
        {
            // This also shouldn't be here
            if (Player == null) Player = GameObject.Find("Player");
        }
    }
}
