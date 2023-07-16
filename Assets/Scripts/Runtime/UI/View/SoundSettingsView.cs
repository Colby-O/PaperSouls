using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.Audio;
using PaperSouls.Runtime.MonoSystems.UI;

namespace PaperSouls.Runtime.UI.View
{
    public class SoundSettingsView : View
    {

        [SerializeField] private Slider _overallSound;
        [SerializeField] private Slider _musicSound;
        [SerializeField] private Slider _sfxSound;
        [SerializeField] private Button _backButton;

        private IAudioMonoSystem _audioMonoSystem;

        /// <summary>
        /// Runs when the overall volume slider changes and Updates the value in the 
        /// Audio Manger. 
        /// </summary>
        private void ChangeOverallVolume()
        {
            _audioMonoSystem.SetOverallVolume(_overallSound.value);
        }

        /// <summary>
        /// Runs when the music volume slider changes and Updates the value in the 
        /// Audio Manger. 
        /// </summary>
        private void ChangeMusicVolume()
        {
            _audioMonoSystem.SetMusicVolume(_musicSound.value);
        }

        /// <summary>
        /// Runs when the SfX volume slider changes and Updates the value in the 
        /// Audio Manger. 
        /// </summary>
        private void ChangeSfxVolume()
        {
            _audioMonoSystem.SetSfXVolume(_sfxSound.value);
        }

        public override void Init()
        {
            _overallSound.onValueChanged.AddListener(delegate { ChangeOverallVolume(); });
            _musicSound.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
            _sfxSound.onValueChanged.AddListener(delegate { ChangeSfxVolume(); });
            _backButton.onClick.AddListener(GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast);
            _audioMonoSystem = GameManager.GetMonoSystem<IAudioMonoSystem>();
        }
    }
}
