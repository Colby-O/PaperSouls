using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Player;
using PaperSouls.Runtime.MonoSystems.Audio;

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(menuName = "Items/Effects/HealthPotionEffect")]
    internal class HealthPotionEffect : UseableItemEffect
    {
        [SerializeField] private float _healthIncrease = 30;

        /// <summary>
        /// Applies a Health boost effect to the player
        /// </summary>
        internal override bool ExcuteEffect(UseableItem item, PlayerManger player)
        {
            if (player != null && !player.IsFullHealth())
            {
                GameManager.Emit<PlayAudioMessage>(new("Health Potion Use", MonoSystems.Audio.AudioType.SfX));
                player.AddHealth(_healthIncrease);
                return true;
            }

            return false;
        }
    }
}
