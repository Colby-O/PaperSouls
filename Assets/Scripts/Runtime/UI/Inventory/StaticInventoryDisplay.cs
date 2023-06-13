using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaticInventoryDisplay : InventoryDisplay
{
    public InventoryHolder inventoryHolder;
    public InventorySlotsUI[] slots;


    public override void AssignSlot(InventoryManger inventory)
    {
        slotDictionary = new();

        if (slots.Length != inventory.numOfInventorySlots) Debug.Log($"Inventory slots out of sync on {this.gameObject}");

        for (int i = 0; i < inventory.numOfInventorySlots; i++)
        {
            slots[i].inventorySlot.slotType = inventory.inventorySlots[i].slotType;
            slotDictionary.Add(slots[i], inventory.inventorySlots[i]);
            slots[i].Init(inventory.inventorySlots[i]);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (inventoryHolder != null)
        {
            if (GetComponent<GridLayoutGroup>() != null) inventoryHolder.ResizeInventory(GridLayoutGroupHelper.GetSize(GetComponent<GridLayoutGroup>()));
            else inventoryHolder.ResizeInventory(new Vector2Int(slots.Length, 1));
            inventoryManger = inventoryHolder.inventoryManger;
            inventoryManger.OnInventoryChange += UpdateSlot;
        }
        else Debug.Log($"No inventory assigned to {this.gameObject}");

        AssignSlot(inventoryManger);
    }
}
