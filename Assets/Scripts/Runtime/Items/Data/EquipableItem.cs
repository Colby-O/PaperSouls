using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Player; 

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Equipable Item", order = 3)]
    internal class EquipableItem : Item
    {
        // TODO: Make Just Equipable Item have slot type??

        [Header("Item Stat Bonuses")]
        public int strengthBonus;
        public int damageBonous;
        public int armorBonous;
        public int agilityBonous;

        internal void Equip(PlayerManger player)
        {

        }

        internal void Unequip(PlayerManger player)
        {

        }
    }
}
