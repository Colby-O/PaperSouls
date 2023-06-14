using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Items
{
    public enum SlotType
    {
        None,
        Any,
        Item,
        Helment,
        Chest,
        Leggings,
        Shoes,
        RangedWeapon,
        MeleeWeapon
    }

    public enum ItemRarity
    {
        Common,
        Rare,
        Legendary
    }

    public enum PickupType
    {
        Inventory,
        Ammo
    }

    [System.Serializable]
    public class ItemClasssification
    {
        public ItemRarity rarity;
        [Range(0.01f, 1f)] public float probability = 0.01f;

        public ItemClasssification(ItemRarity rarity, float probability)
        {
            this.rarity = rarity;
            this.probability = probability;
        }
    }

    [CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1)]
    public class Item : ScriptableObject
    {
        [Header("Header Infomation")]
        public int id = -1;
        public string displayName;
        [TextArea(4, 4)] public string description;

        [Header("Item Infomation")]
        public GameObject itemPrefab;
        public UnityEngine.Sprite icon;
        public PickupType pickupType = PickupType.Inventory;
        public ItemRarity rarity;

        [Header("Inventory Properties")]
        [Min(1)] public int maxStackSize;
        public SlotType slotType;

        /// <summary>
        /// Get the hard code infomation for rarity types
        /// </summary>
        public ItemClasssification GetRarityInfo()
        {
            if (rarity == ItemRarity.Common) return new ItemClasssification(ItemRarity.Common, 0.7f);
            else if (rarity == ItemRarity.Rare) return new ItemClasssification(ItemRarity.Rare, 0.2f);
            else if (rarity == ItemRarity.Legendary) return new ItemClasssification(ItemRarity.Legendary, 0.1f);
            else return null;
        }
    }
}
