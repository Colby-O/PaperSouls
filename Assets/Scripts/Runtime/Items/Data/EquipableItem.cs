using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Player; 

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Equipable Item", order = 3)]
    public class EquipableItem : Item
    {
        // TODO: Make Just Equipable Item have slot type??

        [Header("Item Stat Bonuses")]
        public int strengthBonus;
        public int damageBonous;
        public int armorBonous;
        public int agilityBonous;

        public void Equip(PlayerManger player)
        {

        }

        public void Unequip(PlayerManger player)
        {

        }
    }
}
