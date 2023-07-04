using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using PaperSouls.Core;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.UI.Inventory;
using System.Diagnostics;
using PaperSouls.Runtime.Inventory;

namespace PaperSouls.Runtime.Inventory
{
    public class MouseItemData : MonoBehaviour
    {
        [SerializeField] private Image _sprite;
        [SerializeField] private TextMeshProUGUI _count;
        public InventorySlot InventorySlot;
        private InventorySlotsUI _fromSlot;
        private InventoryManger _inv;

        /// <summary>
        /// Returns the item in the mouse cursor to ir's orignal slot
        /// </summary>
        public void ReturnItem()
        {
            if (_fromSlot.InventorySlot.ItemData == null)
            {
                _fromSlot?.InventorySlot.AssignItem(InventorySlot);
                _fromSlot?.UpdateSlot();
            }
            else
            {
                if (!_inv.AddToInventory(InventorySlot.ItemData, InventorySlot.StackSize)) DropItems();
            }
            ClearSlot();
        }

        /// <summary>
        /// Clear the slots data
        /// </summary>
        public void ClearSlotData()
        {
            InventorySlot?.ClearSlot();
            InventorySlot.InventorySlotType = SlotType.Any;
            _sprite.color = Color.clear;
            _count.text = "";
            _sprite.sprite = null;
        }

        /// <summary>
        /// Clear the slots data and the _fromSlot
        /// </summary>
        public void ClearSlot()
        {
            ClearSlotData();
            _fromSlot = null;
        }

        /// <summary>
        /// Updates the slots data given a slot
        /// </summary>
        public void UpdateSlot(InventorySlot slot)
        {
            InventorySlot.AssignItem(slot);
            InventorySlot.InventorySlotType = slot.ItemData.slotType;
            _sprite.sprite = slot.ItemData.icon;
            _sprite.color = Color.white;
            _count.text = slot.StackSize.ToString();
        }

        /// <summary>
        /// Updates the slots data given a UI slot
        /// </summary>
        public void UpdateSlot(InventorySlotsUI fromSlot, InventoryManger inv)
        {
            _inv = inv;
            this._fromSlot = fromSlot;
            UpdateSlot(fromSlot.InventorySlot);
        }

        /// <summary>
        /// Updates the slots data given a slot and a from slot
        /// </summary>
        public void UpdateSlot(InventorySlot slot, InventorySlotsUI fromSlot, InventoryManger inv)
        {
            _inv = inv;
            this._fromSlot = fromSlot;
            UpdateSlot(slot);
        }

        /// <summary>
        /// Drops the items in the mosue slot
        /// </summary>
        private void DropItems()
        {
            for (int i = 0; i < InventorySlot.StackSize; i++)
            {
                Vector3 playerFrontLocaton = GameManger.Instance.Player.transform.position + 2 * GameManger.Instance.Player.transform.forward;
                Vector3 dropLocaton = new Vector3(playerFrontLocaton.x + 0.1f * i, 0, playerFrontLocaton.z + 0.1f * i);
                GameObject.Instantiate(InventorySlot.ItemData.itemPrefab, new Vector3(dropLocaton.x, 0, dropLocaton.z), Quaternion.identity);
            }

            ClearSlot();
        }

        /// <summary>
        /// Moves the mouse slot to the current cursor position
        /// </summary>
        private void FollowCursor()
        {
            if (InventorySlot.ItemData != null)
            {
                transform.position = Mouse.current.position.ReadValue();

                if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject()) DropItems();
            }
        }

        /// <summary>
        /// Checks if mouse cursor is over UI or not
        /// </summary>
        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrent = new(EventSystem.current);
            eventDataCurrent.position = Mouse.current.position.ReadValue();
            List<RaycastResult> res = new();
            EventSystem.current.RaycastAll(eventDataCurrent, res);

            return res.Count > 0;
        }

        private void Awake()
        {
            ClearSlot();
        }

        private void Update()
        {
            FollowCursor();
        }
    }
}
