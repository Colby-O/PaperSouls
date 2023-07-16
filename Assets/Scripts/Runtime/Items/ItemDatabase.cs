using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PaperSouls.Runtime.Items
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]
    internal class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<Item> _itemDatabase;

        /// <summary>
        /// Initalizes the ID's of all items in the database
        /// </summary>
        [ContextMenu(itemName: "Set IDs")]
        public void SetItemIDs()
        {
            _itemDatabase = new();

            List<Item> foundItems = Resources.LoadAll<Item>(path: "Data").OrderBy(e => e.id).ToList();

            List<Item> hasIDInRange = foundItems.Where(e => e.id != -1 && e.id < foundItems.Count).OrderBy(e => e.id).ToList();
            List<Item> hasIDNotInRange = foundItems.Where(e => e.id != -1 && e.id >= foundItems.Count).OrderBy(e => e.id).ToList();
            List<Item> noID = foundItems.Where(e => e.id <= -1).ToList();

            int index = 0;
            for (int i = 0; i < foundItems.Count; i++)
            {
                Item newItem;
                newItem = hasIDInRange.Find(e => e.id == i);
                if (newItem != null) _itemDatabase.Add(newItem);
                else if (index < noID.Count)
                {
                    noID[index].id = i;
                    newItem = noID[index];
                    index++;
                    _itemDatabase.Add(newItem);
                }
            }

            foreach (Item item in hasIDNotInRange)
            {
                item.id = _itemDatabase.Count;
                _itemDatabase.Add(item);
            }
        }

        /// <summary>
        /// Fetches an item from the database with a gven id
        /// </summary>
        public Item GetItem(int id)
        {
            return _itemDatabase.Find(e => e.id == id);
        }

        /// <summary>
        /// Fetches an item from the database with a gven id
        /// </summary>
        public Item GetItem(string name)
        {
            return _itemDatabase.Find(e => e.displayName == name);
        }
    }
}
