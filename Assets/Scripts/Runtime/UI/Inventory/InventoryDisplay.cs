using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class InventoryDisplay : MonoBehaviour
{
    public MouseItemData mouseItemData;
    public InventoryManger inventoryManger;
    public Dictionary<InventorySlotsUI, InventorySlot> slotDictionary;

    public abstract void AssignSlot(InventoryManger inventory);

    protected virtual void UpdateSlot(InventorySlot slotToUpdate)
    {
        foreach (var slot in slotDictionary)
        {
            if (slot.Value == slotToUpdate) slot.Key.UpdateSlot(slotToUpdate);
        }
    }

    private bool IsCorrectType(InventorySlotsUI slot)
    {
        bool isCorrectType = (slot.inventorySlot.slotType == SlotType.Any || slot.inventorySlot.slotType == mouseItemData.inventorySlot.slotType);

        if (!isCorrectType) mouseItemData.ReturnItem();

        return isCorrectType;
    }

    private void SwapSlot(InventorySlotsUI slot)
    {
        if (!IsCorrectType(slot)) return;

        InventorySlot copySlot = new(mouseItemData.inventorySlot.itemData, mouseItemData.inventorySlot.stackSize);
        mouseItemData.ClearSlotData();

        mouseItemData.UpdateSlot(slot.inventorySlot);

        slot.ClearSlot();
        slot.inventorySlot.AssignItem(copySlot);
        slot.UpdateSlot();
    }

    private void MoveItemTo(InventorySlotsUI slot)
    {
        if (!IsCorrectType(slot)) return;

        slot.inventorySlot.AssignItem(mouseItemData.inventorySlot);
        slot.UpdateSlot();
        mouseItemData.ClearSlot();
    }

    public void SlotClicked(InventorySlotsUI slot)
    {
        bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

        if (slot.inventorySlot.itemData != null && mouseItemData.inventorySlot.itemData == null)
        {
            if (isShiftPressed && slot.inventorySlot.SplitStack(out InventorySlot halfStackSlot))
            {
                mouseItemData.UpdateSlot(halfStackSlot, slot);
                slot.UpdateSlot();
            }
            else
            {
                mouseItemData.UpdateSlot(slot);
                slot.ClearSlot();
            }

        }
        else if (slot.inventorySlot.itemData == null && mouseItemData.inventorySlot.itemData != null)
        {
            MoveItemTo(slot);
        }
        else if (slot.inventorySlot.itemData != null && mouseItemData.inventorySlot.itemData != null)
        {
            bool isSameItem = slot.inventorySlot.itemData == mouseItemData.inventorySlot.itemData;

            if (isSameItem && slot.inventorySlot.CheckRoomLeftInStack(mouseItemData.inventorySlot.stackSize)) MoveItemTo(slot);
            else if (isSameItem && !slot.inventorySlot.CheckRoomLeftInStack(mouseItemData.inventorySlot.stackSize, out int spaceLeft))
            {
                if (spaceLeft < 1) SwapSlot(slot);
                else
                {
                    int amountLeft = mouseItemData.inventorySlot.stackSize - spaceLeft;

                    slot.inventorySlot.AddToStack(spaceLeft);
                    slot.UpdateSlot();

                    InventorySlot newItem = new(mouseItemData.inventorySlot.itemData, amountLeft);

                    mouseItemData.ClearSlotData();
                    mouseItemData.UpdateSlot(newItem);
                }
            }
            else SwapSlot(slot);
        }
    }

    protected virtual void Awake()
    {

    }
}

