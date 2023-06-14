using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Helpers;

namespace PaperSouls.Runtime.UI.Inventory
{

    public class StaticInventoryDisplay : InventoryDisplay
    {
        [SerializeField] private InventoryHolder _inventoryHolder;
        [SerializeField] private InventorySlotsUI[] _slots;

        public override void AssignSlot(InventoryManger inventory)
        {
            _slotDictionary = new();

            if (_slots.Length != inventory.NumOfInventorySlots) Debug.Log($"Inventory _slots out of sync on {this.gameObject}");

            for (int i = 0; i < inventory.NumOfInventorySlots; i++)
            {
                _slots[i].InventorySlot.InventorySlotType = inventory.InventorySlots[i].InventorySlotType;
                _slotDictionary.Add(_slots[i], inventory.InventorySlots[i]);
                _slots[i].Init(inventory.InventorySlots[i]);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (_inventoryHolder != null)
            {
                if (GetComponent<GridLayoutGroup>() != null) _inventoryHolder.ResizeInventory(GridLayoutGroupHelper.GetSize(GetComponent<GridLayoutGroup>()));
                else _inventoryHolder.ResizeInventory(new Vector2Int(_slots.Length, 1));
                _inventoryManger = _inventoryHolder.InventoryManger;
                _inventoryManger.OnInventoryChange += UpdateSlot;
            }
            else Debug.Log($"No inventory assigned to {this.gameObject}");

            AssignSlot(_inventoryManger);
        }
    }
}
