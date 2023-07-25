using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PaperSouls.Core;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems.DataPersistence;
using PaperSouls.Runtime.MonoSystems.DungeonGeneration;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class GameSetupView : View
    {
        [SerializeField] TMP_InputField _profleNameInput;
        [SerializeField] Toggle _useRandomSeedInput;
        [SerializeField] TMP_InputField _seedInput;
        [SerializeField] Button _startButton;
        [SerializeField] Button _cancelButton;

        private string _profileName = string.Empty;
        private string _seed = string.Empty;
        private bool _useRandomSeed = true;

        private IUIMonoSystem _uiManager;
        private IDataPersistenceMonoSystem _dataManager;

        /// <summary>
        /// 
        /// </summary>
        private void SetProfileName(string name)
        {
            _profileName = name;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetSeed(string name)
        {
            _seed = name;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetUseRandomSeed(bool useRandomSeed)
        {
            _useRandomSeed = useRandomSeed;
            _seedInput.gameObject.transform.parent.gameObject.SetActive(!_useRandomSeed);
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartGame()
        {
            if (_profileName == string.Empty) return;
            if (!_useRandomSeed && _seed == string.Empty) return;

            if (!int.TryParse(_seed, out int seed) && !_useRandomSeed) return;
            if (_useRandomSeed) seed = Random.Range(-1000000, 1000000);


            GameManager.Emit<ChangeProfileMessage>(new(_profileName));
            GameManager.Emit<GenerateDungeonMessage>(new(seed));
            GameManager.Emit<StartGameMessage>(new());
        }

        public override void Init()
        {
            _uiManager = GameManager.GetMonoSystem<IUIMonoSystem>();
            _dataManager = GameManager.GetMonoSystem<IDataPersistenceMonoSystem>();

            _cancelButton.onClick.AddListener(_uiManager.ShowLast);
            _startButton.onClick.AddListener(StartGame);

            _profleNameInput.onValueChanged.AddListener(SetProfileName);
            _seedInput.onValueChanged.AddListener(SetSeed);
            _useRandomSeedInput.onValueChanged.AddListener(SetUseRandomSeed);

            _seedInput.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
