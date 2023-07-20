using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;

namespace PaperSouls.Runtime.UI.View {
    internal sealed class SettingsView : View
    {
        [SerializeField] private Button _graphicSettingsButton;
        [SerializeField] private Button _controlSettingsButton;
        [SerializeField] private Button _soundSettingsButton;
        [SerializeField] private Button _backButton;


        /// <summary>
        /// Opens the Sounds Settings Menu 
        /// </summary>
        private void OpenSoundSettings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<SoundSettingsView>();
        }

        /// <summary>
        /// Opens the Control Settings Menu 
        /// </summary>
        private void OpenControlSettings()
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<ControlSettingsView>();
        }

        public override void Init()
        {
            _controlSettingsButton.onClick.AddListener(OpenControlSettings);
            _soundSettingsButton.onClick.AddListener(OpenSoundSettings);
            _backButton.onClick.AddListener(GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast);
        }
    }
}
