using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.UI.View;

namespace PaperSouls.Runtime.MonoSystems.GameState
{
    internal sealed class GameStateMonoSystem : MonoBehaviour, IGameStateMonoSystem
    {
        public GameStates CurrentState;

        public void ChangeToMainMenuState()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<MainMenuView>();
            CurrentState = GameStates.MainMenu;
        }

        public void ChangeToPausedState()
        {
            PaperSoulsGameManager.AccpetPlayerInput = false;
            CurrentState = GameStates.Paused;
        }

        public void ChangeToPlayingState()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<PlayerHUDView>();
            PaperSoulsGameManager.AccpetPlayerInput = true;
            CurrentState = GameStates.Playing;
        }

        public void ChangeToDeadState()
        {
            CurrentState = GameStates.Dead;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            ChangeToPlayingState();
        }

        private void ChangeGameState(ChangeGameStateMessage msg)
        {
            if (msg.NextGameState == CurrentState) return;

            switch (msg.NextGameState)
            {
                case GameStates.MainMenu:
                    ChangeToMainMenuState();
                    break;
                case GameStates.Playing:
                    ChangeToPlayingState();
                    break;
                case GameStates.Paused:
                    ChangeToPausedState();
                    break;
                case GameStates.Dead:
                    ChangeToDeadState();
                    break;
                default:
                    Debug.LogWarning($"{msg.NextGameState} is not a vaild GameState.");
                    break;
            }
        }

        private void AddListeners()
        {
            GameManager.AddListener<ChangeGameStateMessage>(ChangeGameState);
        }

        private void Awake()
        {
            AddListeners();
        }
    }
}
