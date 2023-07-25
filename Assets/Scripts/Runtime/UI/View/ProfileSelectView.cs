using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Core;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.MonoSystems.UI;
using PaperSouls.Runtime.MonoSystems.DataPersistence;
using PaperSouls.Runtime.MonoSystems.DungeonGeneration;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.UI.View
{
    internal sealed class ProfileSelectView : View
    {
        [SerializeField] private List<Button> _profileButtons;
        [SerializeField] private List<Text> _profileNames;
        [SerializeField] private List<Button> _deleteProfileButtons;

        [SerializeField] private Button _backButton;

        private IUIMonoSystem _uiManager;
        private IDataPersistenceMonoSystem _dataManager;

        /// <summary>
        /// 
        /// </summary>
        private void SelectProfile(GameData profileData, string profileName, int profileID)
        {
            Debug.Log($"Opening Profile {profileName}");
            GameManager.Emit<SetProfileIDMessage>(new(profileID));
            GameManager.Emit<ChangeProfileMessage>(new(profileName));
            GameManager.Emit<StartGameMessage>(new());
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateNewProfile(int profileID)
        {
            Debug.Log($"Creating a New Profile on Slot {profileID}");
            GameManager.Emit<SetProfileIDMessage>(new(profileID));
            _uiManager.Show<GameSetupView>();

        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteProfile(string profileName, int profileID)
        {
            Debug.Log($"Deleting Profile {profileName}");
            GameManager.Emit<DeleteProfileMessage>(new(profileName));
            _profileNames[profileID].text = "Create New Game";
            _profileButtons[profileID].onClick.RemoveAllListeners();
            _profileButtons[profileID].onClick.AddListener(delegate { CreateNewProfile(profileID); });
            _deleteProfileButtons[profileID].gameObject.SetActive(false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitProfiles()
        {
            for (int i = 0; i < _profileNames.Count; i++)
            {
                int profileID = i;
                _profileNames[i].text = "Create New Game";
                _profileButtons[i].onClick.AddListener(delegate { CreateNewProfile(profileID + 1); });
                _deleteProfileButtons[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadProfiles()
        {
            InitProfiles();

            Dictionary<string, GameData> profiles = _dataManager.FetchAllProfiles();

            List<KeyValuePair<string, GameData>> vaildProfiles = profiles.Where(kvp => kvp.Value.ProfileID > 0 && kvp.Value.ProfileID <= _profileNames.Count).ToList();
            vaildProfiles.Sort(
                delegate (KeyValuePair<string, GameData> profile1,
                KeyValuePair<string, GameData> profile2)
                {
                    return profile1.Value.ProfileID.CompareTo(profile2.Value.ProfileID);
                }
            );

            for (int i = 0; i < vaildProfiles.Count; i++)
            {
                int profileID = vaildProfiles[i].Value.ProfileID - 1;
                int profileIndex = i;
                _profileNames[profileID].text = vaildProfiles[profileIndex].Key;
                _profileButtons[profileID].onClick.RemoveAllListeners();
                _profileButtons[profileID].onClick.AddListener(delegate { SelectProfile(vaildProfiles[profileIndex].Value, vaildProfiles[profileIndex].Key, profileID + 1); });
                _deleteProfileButtons[profileID].gameObject.SetActive(true);
                _deleteProfileButtons[profileID].onClick.RemoveAllListeners();
                _deleteProfileButtons[profileID].onClick.AddListener(delegate { DeleteProfile(vaildProfiles[profileIndex].Key, profileID); });
            }
        }

        public override void Init()
        {
            _uiManager = GameManager.GetMonoSystem<IUIMonoSystem>();
            _dataManager = GameManager.GetMonoSystem<IDataPersistenceMonoSystem>();

            LoadProfiles();
            _backButton.onClick.AddListener(_uiManager.ShowLast);
        }
    }
}
