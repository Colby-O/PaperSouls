using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Core
{

    /// <summary>
    /// List of GameStates
    /// </summary>
    public enum GameState
    {
        InMainMenu,
        Playing,
        InMenu,
        PlayerDead
    }

    public class GameManger : MonoBehaviour
    {
        private static GameManger _instance;
        private static readonly object Padlock = new();

        public static GameManger Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new();
                    }

                    return _instance;
                }
            }
        }

        public GameObject Player { get; private set; }
        [SerializeField] private ItemDatabase _itemDatabase;
        private GameState State { get; set; }

        public static bool AccpetPlayerInput { get; private set; }

        /// <summary>
        /// Updates the GameState and run any corrspounding logic. 
        /// </summary>
        public static void UpdateGameState(GameState newState)
        {
            _instance.State = newState;

            switch (_instance.State)
            {
                case GameState.InMainMenu:
                    break;
                case GameState.Playing:
                    AccpetPlayerInput = true;
                    break;
                case GameState.InMenu:
                    AccpetPlayerInput = false;
                    break;
                case GameState.PlayerDead:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    UpdateGameState(GameState.Playing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_instance.State), _instance.State, null);
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
            _itemDatabase.SetItemIDs();
            State = GameState.Playing;
            AccpetPlayerInput = true;
        }

        private void Update()
        {
            if (_instance.Player == null) _instance.Player = GameObject.Find("Player");
        }
    }
}
