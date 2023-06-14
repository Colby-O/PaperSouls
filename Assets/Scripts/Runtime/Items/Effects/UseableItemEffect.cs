using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Items
{
    public abstract class UseableItemEffect : ScriptableObject
    {
        /// <summary>
        /// Define an effect a Useable Item Has
        /// </summary>
        public abstract bool ExcuteEffect(UseableItem item, PlayerManger player);
    }
}
