using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.MonoSystems.UI;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class MainMenuView : View
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        /// <summary>
        /// Starts the game
        /// </summary>
        private void StartGame()
        {
            //GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Playing));
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<ProfileSelectView>();
        }

        /// <summary>
        /// Quits the game
        /// </summary>
        private void QuitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Open the Settings Menu
        /// </summary>
        private void OpenSettings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SettingsView>();
        }

        public override void Init()
        {
            _playButton.onClick.AddListener(StartGame);
            _settingsButton.onClick.AddListener(OpenSettings);
            _quitButton.onClick.AddListener(QuitGame);
        }

        public override void Show()
        {
            base.Show();
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
