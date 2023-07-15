using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(fileName = "UseableItem", menuName = "Items/Usable Item", order = 2)]
    internal class UseableItem : Item, IUseables
    {
        [Header("Effects")]
        public List<UseableItemEffect> effects;

        public virtual void Use(PlayerManger player)
        {
            foreach (UseableItemEffect effect in effects)
            {
                effect.ExcuteEffect(this, player);
            }
        }

        public virtual void Use(PlayerManger player, out bool sucessful)
        {
            int effectExceutedSucessfully = 0;
            foreach (UseableItemEffect effect in effects)
            {
                bool excuted = effect.ExcuteEffect(this, player);
                if (excuted) effectExceutedSucessfully += 1;
            }

            sucessful = effectExceutedSucessfully > 0;
        }
    }
}
