using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Player;
using PaperSouls.Runtime.MonoSystems.Audio;

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(menuName = "Items/Effects/ExpPotionEffect")]
    internal class ExpPotionEffect : UseableItemEffect
    {
        [SerializeField] private float _expIncrease = 30;

        /// <summary>
        /// Applies a Exp boost effect to the player
        /// </summary>
        internal override bool ExcuteEffect(UseableItem item, PlayerManger player)
        {
            if (player != null)
            {
                GameManager.Emit<PlayAudioMessage>(new("Health Potion Use", MonoSystems.Audio.AudioType.SfX));
                player.AddXP(_expIncrease);
                return true;
            }

            return false;
        }
    }
}
