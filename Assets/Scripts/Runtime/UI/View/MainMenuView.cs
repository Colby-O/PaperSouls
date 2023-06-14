using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PaperSouls.Core;

namespace PaperSouls.Runtime.UI.View
{
    public class MainMenuView : View
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _quitButton;

        /// <summary>
        /// Starts the game
        /// </summary>
        private void StartGame()
        {
            GameManger.UpdateGameState(GameState.Playing);
            SceneManager.LoadScene("TestScene");
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
            ViewManger.Show<SettingsView>();
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
            GameManger.UpdateGameState(GameState.InMainMenu);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }
}
