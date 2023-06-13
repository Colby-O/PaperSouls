using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicInventoryDisplay : InventoryDisplay
{
    public InventorySlotsUI slotPrefab;

    public void RefreshDynamicInventory(InventoryManger inventory)
    {
        ClearAllSLots();
        inventoryManger = inventory;
        AssignSlot(inventory);
    }

    private void ClearAllSLots()
    {
        foreach (Transform item in transform.Cast<Transform>())
        {
            GameObject.Destroy(item.gameObject);
        }

        if (slotDictionary != null) slotDictionary.Clear();
    }

    public override void AssignSlot(InventoryManger inventory)
    {
        slotDictionary = new();

        if (inventory == null) return;

        for (int i = 0; i < inventory.numOfInventorySlots; i++)
        {
            InventorySlotsUI slot = Instantiate(slotPrefab, transform);
            slotDictionary.Add(slot, inventory.inventorySlots[i]);
            slot.Init(inventory.inventorySlots[i]);
            slot.UpdateSlot();
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
