using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Database/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> itemDatabase;

    [ContextMenu(itemName:"Set IDs")]
    public void SetItemIDs()
    {
        itemDatabase = new();

        List<Item> foundItems = Resources.LoadAll<Item>(path:"Data").OrderBy(e => e.id).ToList();

        List<Item> hasIDInRange = foundItems.Where(e => e.id != -1 && e.id < foundItems.Count).OrderBy(e => e.id).ToList();
        List<Item> hasIDNotInRange = foundItems.Where(e => e.id != -1 && e.id >= foundItems.Count).OrderBy(e => e.id).ToList();
        List<Item> noID = foundItems.Where(e => e.id  <= -1).ToList();

        int index = 0;
        for (int i = 0; i < foundItems.Count; i++)
        {
            Item newItem;
            newItem = hasIDInRange.Find(e => e.id == i);
            if (newItem != null) itemDatabase.Add(newItem);
            else if (index < noID.Count)
            {
                noID[index].id = i;
                newItem = noID[index];
                index++;
                itemDatabase.Add(newItem);
            }
        }

        foreach (Item item in hasIDNotInRange) 
        {
            item.id = itemDatabase.Count;
            itemDatabase.Add(item);
        }
    }

    public Item GetItem(int id)
    {
        return itemDatabase.Find(e => e.id == id);
    }
}
