using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.Data;

namespace PaperSouls.Runtime.Inventory
{
    [System.Serializable]
    internal class InventoryManger
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
        /// Deseralize a List of Seralizanble inventory slots
        /// </summary>
        public InventoryManger(List<SerializableInventorySlot> slots)
        {
            InventorySlots = new();
            foreach (SerializableInventorySlot slot in slots) InventorySlots.Add(slot.Deserialize());
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
                if (item.maxStackSize < amount)
                {
                    freeSlot.UpdateInventorySlot(item, item.maxStackSize);
                    AddToInventory(item, amount - item.maxStackSize);

                } else freeSlot.UpdateInventorySlot(item, amount);
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

        /// <summary>
        /// Returns true if item is in inventory
        /// </summary>
        public bool HasItem(Item item, int count = 1)
        {
            int countInInv = 
                InventorySlots
                    .Where(slot => slot.ItemData == item)
                    .Sum(slot => slot.StackSize);
            return countInInv >= count;
        }

        /// <summary>
        /// Takes 'count' items from the inventory if theres enought space
        /// Returns true if there were enough items else false
        /// </summary>
        public bool TakeItem(Item item, int count)
        {
            if (!HasItem(item, count)) return false;

            IEnumerable<InventorySlot> slotsOfItem = InventorySlots.Where(
                slot => slot.ItemData == item
            );

            int leftToRemove = count;
            foreach (var slot in slotsOfItem) {
                int toRemove = Mathf.Min(slot.StackSize, leftToRemove);
                slot.RemoveFromStack(toRemove);
                leftToRemove -= toRemove;
                OnInventoryChange?.Invoke(slot);
                if (leftToRemove == 0) break;
            }

            return true;
        }

        public List<SerializableInventorySlot> Seralize()
        {
            List<SerializableInventorySlot> serializableSlots = new();
            foreach (InventorySlot slot in InventorySlots) serializableSlots.Add(new((slot.ItemData != null) ? slot.ItemData.id : -1, slot.StackSize)); 
            return serializableSlots;
        }
    }
}
