using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.MonoSystems.DataPersistence
{
    internal interface IDataPersistenceMonoSystem : IMonoSystem
    {
        /// <summary>
        /// Crates a new game.
        /// </summary>
        public void NewGame();

        /// <summary>
        /// Save the game to an JSON file
        /// </summary>
        public void LoadGame();

        /// <summary>
        /// Loads a game from JSON file
        /// </summary>
        public void SaveGame();

        /// <summary>
        /// Loads all profiles
        /// </summary>
        public Dictionary<string, GameData> FetchAllProfiles();
    }
}
