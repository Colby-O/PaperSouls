using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.Interfaces
{
    internal interface IDataPersistence
    {
        /// <summary>
        /// Load the <paramref name="data"/>.
        /// Current implementation requires this be done between scenes.
        /// </summary>
        public bool SaveData(GameData data);

        /// <summary>
        /// save the <paramref name="data"/>.
        /// Current implementation requires this be done between scenes.
        /// </summary>
        public bool LoadData(ref GameData data);
    }
}
