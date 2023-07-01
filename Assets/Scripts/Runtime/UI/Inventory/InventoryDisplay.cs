using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.UI.Inventory
{
    public abstract class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] protected MouseItemData _mouseItemData;
        protected InventoryManger _inventoryManger;
        protected Dictionary<InventorySlotsUI, InventorySlot> _slotDictionary;

        /// <summary>
        /// Assign Inventroy slots to UI slots
        /// </summary>
        public abstract void AssignSlot(InventoryManger inventory);

        protected virtual void UpdateSlot(InventorySlot slotToUpdate)
        {
            foreach (var slot in _slotDictionary)
            {
                if (slot.Value == slotToUpdate) slot.Key.UpdateSlot(slotToUpdate);
            }
        }

        /// <summary>
        /// Checks if a Inventory Slot Contains a Vaild type or not
        /// </summary>
        private bool IsCorrectType(InventorySlotsUI slot)
        {
            bool isCorrectType = (slot.InventorySlot.InventorySlotType == SlotType.Any || slot.InventorySlot.InventorySlotType == _mouseItemData.InventorySlot.InventorySlotType);

            if (!isCorrectType) _mouseItemData.ReturnItem();

            return isCorrectType;
        }

        /// <summary>
        /// Swap the item in the mouses cursor with the item in a given slot
        /// </summary>
        private void SwapSlot(InventorySlotsUI slot)
        {
            if (!IsCorrectType(slot)) return;

            InventorySlot copySlot = new(_mouseItemData.InventorySlot.ItemData, _mouseItemData.InventorySlot.StackSize);
            _mouseItemData.ClearSlotData();

            _mouseItemData.UpdateSlot(slot.InventorySlot);

            slot.ClearSlot();
            slot.InventorySlot.AssignItem(copySlot);
            slot.UpdateSlot();
        }

        /// <summary>
        /// Move item in mouse cursor to given slot
        /// </summary>
        private void MoveItemTo(InventorySlotsUI slot)
        {
            if (!IsCorrectType(slot)) return;

            slot.InventorySlot.AssignItem(_mouseItemData.InventorySlot);
            slot.UpdateSlot();
            _mouseItemData.ClearSlot();
        }

        /// <summary>
        /// Processes the logic When a inventory slot is clicked.
        /// </summary>
        public void SlotClicked(InventorySlotsUI slot)
        {
            bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;

            if (slot.InventorySlot.ItemData != null && _mouseItemData.InventorySlot.ItemData == null)
            {
                if (isShiftPressed && slot.InventorySlot.SplitStack(out InventorySlot halfStackSlot))
                {
                    _mouseItemData.UpdateSlot(halfStackSlot, slot, _inventoryManger);
                    slot.UpdateSlot();
                }
                else
                {
                    _mouseItemData.UpdateSlot(slot, _inventoryManger);
                    slot.ClearSlot();
                }

            }
            else if (slot.InventorySlot.ItemData == null && _mouseItemData.InventorySlot.ItemData != null)
            {
                MoveItemTo(slot);
            }
            else if (slot.InventorySlot.ItemData != null && _mouseItemData.InventorySlot.ItemData != null)
            {
                bool isSameItem = slot.InventorySlot.ItemData == _mouseItemData.InventorySlot.ItemData;

                if (isSameItem && slot.InventorySlot.CheckRoomLeftInStack(_mouseItemData.InventorySlot.StackSize)) MoveItemTo(slot);
                else if (isSameItem && !slot.InventorySlot.CheckRoomLeftInStack(_mouseItemData.InventorySlot.StackSize, out int spaceLeft))
                {
                    if (spaceLeft < 1) SwapSlot(slot);
                    else
                    {
                        int amountLeft = _mouseItemData.InventorySlot.StackSize - spaceLeft;

                        slot.InventorySlot.AddToStack(spaceLeft);
                        slot.UpdateSlot();

                        InventorySlot newItem = new(_mouseItemData.InventorySlot.ItemData, amountLeft);

                        _mouseItemData.ClearSlotData();
                        _mouseItemData.UpdateSlot(newItem);
                    }
                }
                else SwapSlot(slot);
            }
        }

        protected virtual void Awake() { }
    }
}

// "word" - Roo 2K23

