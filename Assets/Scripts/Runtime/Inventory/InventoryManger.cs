using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryManger
{
    public List<InventorySlot> inventorySlots;
    public int numOfInventorySlots => inventorySlots.Count;

    private Vector2Int inventorySize;

    public UnityAction<InventorySlot> OnInventoryChange;

    public InventoryManger(int numRows, int numCols) : this(new Vector2Int(numRows, numCols)) { }

    public InventoryManger(Vector2Int inventorySize)
    {
        this.inventorySize = inventorySize;
        int size = inventorySize.x * inventorySize.y;
        inventorySlots = new(size);

        for (int i = 0; i < size; i++) inventorySlots.Add(new());
    }

    public bool GetInventorySlotForItem(Item item, out List<InventorySlot> inventorySlots)
    {
        inventorySlots = this.inventorySlots.Where(inventorySlots => inventorySlots.itemData == item).ToList();

        return inventorySlots.Count > 0;
    }

    public bool GetFreeInventorySlot(out InventorySlot slot)
    {
        slot = inventorySlots.FirstOrDefault(i => i.itemData == null);

        return slot != null;
    }

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

    public bool AddToInventory(Item item, int amount, int index)
    {
        if (index >= numOfInventorySlots) return false;

        if (inventorySlots[index].itemData == null)
        {
            inventorySlots[index].UpdateInventorySlot(item, amount);
            OnInventoryChange?.Invoke(inventorySlots[index]);
            return true;
        } else if (inventorySlots[index].CheckRoomLeftInStack(amount))
        {
            inventorySlots[index].AddToStack(amount);
            OnInventoryChange?.Invoke(inventorySlots[index]);
            return true;
        }

        return false;
    }
}
