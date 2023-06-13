using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InventoryHolder : MonoBehaviour
{
    public Vector2Int inventorySize;
    public InventoryManger inventoryManger;
    public List<SlotType> slotTypes;

    public static UnityAction<InventoryManger> OnDynamicInventoryDisplayRequest;

    public void ResizeInventory(Vector2Int inventorySize)
    {
        inventoryManger = new(inventorySize);

        if (slotTypes.Count != inventoryManger.inventorySlots.Count) return;

        for (int i = 0; i < slotTypes.Count; i++)
        {
            inventoryManger.inventorySlots[i].slotType = slotTypes[i];
        }
    }

    protected virtual void Awake() { }

}
