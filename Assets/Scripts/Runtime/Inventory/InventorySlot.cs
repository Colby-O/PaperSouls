using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.Inventory
{

    [System.Serializable]
    public class InventorySlot
    {
        public Item ItemData;
        public int StackSize;
        public SlotType InventorySlotType = SlotType.Any;

        /// <summary>
        /// Create an empty slot
        /// </summary>
        public InventorySlot()
        {
            ClearSlot();
        }

        /// <summary>
        /// Create a slot with ItemData and StackSize
        /// </summary>
        public InventorySlot(Item itemData, int stackSize)
        {
            this.ItemData = itemData;
            this.StackSize = stackSize;
        }

        /// <summary>
        /// Clears the slot
        /// </summary>
        public void ClearSlot()
        {
            this.ItemData = null;
            this.StackSize = -1;
        }

        /// <summary>
        /// Assigns a another inventory slots values to this slot
        /// </summary>
        public void AssignItem(InventorySlot slot)
        {
            if (ItemData == slot.ItemData) AddToStack(slot.StackSize);
            else
            {
                ItemData = slot.ItemData;
                StackSize = 0;
                AddToStack(slot.StackSize);
            }
        }

        /// <summary>
        /// Updates the slots data
        /// </summary>
        public void UpdateInventorySlot(Item item, int amount)
        {
            ItemData = item;
            StackSize = amount;
        }

        /// <summary>
        /// Checks if there is room left in the slot
        /// </summary>
        public bool CheckRoomLeftInStack(int amount)
        {
            return (StackSize + amount <= ItemData.maxStackSize);
        }

        /// <summary>
        /// Checks if there is room left in the slot and returns the amount left
        /// </summary>
        public bool CheckRoomLeftInStack(int amount, out int amountLeft)
        {
            amountLeft = ItemData.maxStackSize - StackSize;

            return CheckRoomLeftInStack(amount);
        }

        /// <summary>
        /// Add an amount to a existing item stack
        /// </summary>
        public void AddToStack(int amount)
        {
            if (CheckRoomLeftInStack(amount)) StackSize += amount;
        }

        /// <summary>
        /// Removes an amount to a existing item stack
        /// </summary>
        public void RemoveFromStack(int amount)
        {
            StackSize -= amount;
        }

        /// <summary>
        /// Splits a stack in half and assigns the other half to an inputed slot
        /// </summary>
        public bool SplitStack(out InventorySlot slot)
        {
            if (StackSize <= 1)
            {
                slot = null;
                return false;
            }

            int halfStack = Mathf.RoundToInt(StackSize / 2);

            StackSize -= halfStack;

            slot = new InventorySlot(ItemData, halfStack);

            return true;
        }
    }
}
