using PaperSouls.Core;
using PaperSouls.Runtime.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Inventory;

namespace PaperSouls.Runtime.Data
{
    [System.Serializable]
    internal sealed class SerializableInventorySlot
    {
        public int ID = -1;
        public int StackSize = -1;

        public SerializableInventorySlot(int id, int stackSize)
        {
            ID = id;
            StackSize = stackSize;
        }

        public InventorySlot Deserialize()
        {
            return new InventorySlot((ID != -1) ? PaperSoulsGameManager.ItemDatabase.GetItem(ID) : null, StackSize);
        }
    }
}
