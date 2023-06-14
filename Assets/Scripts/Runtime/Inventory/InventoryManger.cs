using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.Inventory
{
    [System.Serializable]
    public class InventoryManger
    {
        public List<InventorySlot> InventorySlots;
        public int NumOfInventorySlots => InventorySlots.Count;

        public UnityAction<InventorySlot> OnInventoryChange;

        private Vector2Int _inventorySize;

        /// <summary>
        /// Create an inventory of size numRows by numCols
        /// </summary>
        public InventoryManger(int numRows, int numCols) : this(new Vector2Int(numRows, numCols)) { }

        /// <summary>
        /// Create an inventory of size given by a Vector2Int
        /// </summary>
        public InventoryManger(Vector2Int inventorySize)
        {
            this._inventorySize = inventorySize;
            int size = inventorySize.x * inventorySize.y;
            InventorySlots = new(size);

            for (int i = 0; i < size; i++) InventorySlots.Add(new());
        }

        /// <summary>
        /// Gets the inventory slot that contains an item
        /// </summary>
        public bool GetInventorySlotForItem(Item item, out List<InventorySlot> inventorySlots)
        {
            inventorySlots = this.InventorySlots.Where(inventorySlots => inventorySlots.ItemData == item).ToList();

            return inventorySlots.Count > 0;
        }

        /// <summary>
        /// Gets the inventory slot that is empty
        /// </summary>
        public bool GetFreeInventorySlot(out InventorySlot slot)
        {
            slot = InventorySlots.FirstOrDefault(i => i.ItemData == null);

            return slot != null;
        }

        /// <summary>
        /// Add an item to a empty inventory slot
        /// </summary>
        public bool AddToInventoryFreeSlot(Item item, int amount)
        {
            if (GetFreeInventorySlot(out InventorySlot freeSlot))
            {
                freeSlot.UpdateInventorySlot(item, amount);
                OnInventoryChange?.Invoke(freeSlot);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add an item to the next available inventory slot
        /// </summary>
        public bool AddToInventory(Item item, int amount)
        {
            if (GetInventorySlotForItem(item, out List<InventorySlot> inventorySlots))
            {
                foreach (InventorySlot slot in inventorySlots)
                {
                    if (slot.CheckRoomLeftInStack(amount))
                    {
                        slot.AddToStack(amount);
                        OnInventoryChange?.Invoke(slot);
                        return true;
                    }
                }
            }

            return AddToInventoryFreeSlot(item, amount);
        }

        /// <summary>
        /// Add an item to ith inventory slot if available
        /// </summary>
        public bool AddToInventory(Item item, int amount, int index)
        {
            if (index >= NumOfInventorySlots) return false;

            if (InventorySlots[index].ItemData == null)
            {
                InventorySlots[index].UpdateInventorySlot(item, amount);
                OnInventoryChange?.Invoke(InventorySlots[index]);
                return true;
            }
            else if (InventorySlots[index].CheckRoomLeftInStack(amount))
            {
                InventorySlots[index].AddToStack(amount);
                OnInventoryChange?.Invoke(InventorySlots[index]);
                return true;
            }

            return false;
        }
    }
}
