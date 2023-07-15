using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.Items
{

    [System.Serializable]
    internal class Drop
    {
        public Item item;
        [Range(0.01f, 1f)] public float probability = 0.01f;

        public Drop(Item item, float probability)
        {
            this.item = item;
            this.probability = probability;
        }
    }

    [CreateAssetMenu(fileName = "LootTable", menuName = "Loot/Table")]
    internal class LootTable : ScriptableObject
    {
        [SerializeField] private List<Drop> table;

        /// <summary>
        /// Fetches a random item from the table
        /// </summary>
        public Item GetItem()
        {
            if (table.Count <= 0) return null;

            Item item = null;
            do
            {
                float rollRarity = Random.value;
                float itemRarity = Random.value;
                int itemIndex = Random.Range(0, table.Count);

                Drop drop = table[itemIndex];

                if (drop.item.GetRarityInfo().probability >= rollRarity)
                {
                    if (drop.probability >= itemRarity) item = drop.item;
                }

            } while (item == null);

            return item;
        }

        /// <summary>
        /// Fetches N random item from the table
        /// </summary>
        public List<Item> GetListOfItem(int numberOfItems)
        {
            List<Item> items = new();

            for (int i = 0; i < numberOfItems; i++)
            {
                items.Add(GetItem());
            }

            return items;
        }
    }
}

