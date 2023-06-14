using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(menuName = "Items/Effects/ExpPotionEffect")]
    public class ExpPotionEffect : UseableItemEffect
    {
        [SerializeField] private float _expIncrease = 30;

        /// <summary>
        /// Applies a Exp boost effect to the player
        /// </summary>
        public override bool ExcuteEffect(UseableItem item, PlayerManger player)
        {
            if (player != null)
            {
                AudioManger.Instance.PlaySFX("Health Potion Use");
                player.AddXP(_expIncrease);
                return true;
            }

            return false;
        }
    }
}
