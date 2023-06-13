using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item itemData;
    public int stackSize;
    public SlotType slotType = SlotType.Any;

    public InventorySlot()
    {
        ClearSlot();
    }

    public InventorySlot(Item itemData, int stackSize)
    {
        this.itemData = itemData;
        this.stackSize = stackSize;
    }

    public void ClearSlot()
    {
        this.itemData = null;
        this.stackSize = -1;
    }

    public void AssignItem(InventorySlot slot)
    {
        if (itemData == slot.itemData) AddToStack(slot.stackSize);
        else
        {
            itemData = slot.itemData;
            stackSize = 0; 
            AddToStack(slot.stackSize);
        }
    }

    public void UpdateInventorySlot(Item item, int amount)
    {
        itemData = item;
        stackSize = amount;
    }

    public bool CheckRoomLeftInStack(int amount)
    {
        return (stackSize + amount <= itemData.maxStackSize);
    }

    public bool CheckRoomLeftInStack(int amount, out int amountLeft)
    {
        amountLeft = itemData.maxStackSize - stackSize;

        return CheckRoomLeftInStack(amount);
    }

    public void AddToStack(int amount)
    {
        if (CheckRoomLeftInStack(amount)) stackSize += amount;
    }
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
    }

    public bool SplitStack(out InventorySlot slot)
    {
        if (stackSize <= 1)
        {
            slot = null;
            return false;
        }

        int halfStack = Mathf.RoundToInt(stackSize / 2);

        stackSize -= halfStack;

        slot = new InventorySlot(itemData, halfStack);

        return true;
    }
}
