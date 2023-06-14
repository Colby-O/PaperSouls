using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(menuName = "Items/Effects/HealthPotionEffect")]
    public class HealthPotionEffect : UseableItemEffect
    {
        [SerializeField] private float _healthIncrease = 30;

        /// <summary>
        /// Applies a Health boost effect to the player
        /// </summary>
        public override bool ExcuteEffect(UseableItem item, PlayerManger player)
        {
            if (player != null && !player.IsFullHealth())
            {
                AudioManger.Instance.PlaySFX("Health Potion Use");
                player.AddHealth(_healthIncrease);
                return true;
            }

            return false;
        }
    }
}
