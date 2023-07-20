using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems.GameState;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class PauseMenuView : View
    {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _mainMenuButton;
        
        /// <summary>
        /// 
        /// </summary>
        private void GotoMainMenu()
        {
            GameManager.Emit<GotoMainMenuMessage>(new());
        }

        /// <summary>
        /// Open the Settings Menu
        /// </summary>
        private void OpenSettings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ResumeGame()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
            GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Playing));
        }

        public override void Init()
        {

            _mainMenuButton.onClick.AddListener(GotoMainMenu);
            _settingsButton.onClick.AddListener(OpenSettings);
            _resumeButton.onClick.AddListener(ResumeGame);
        }
        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
