using System.Linq;
using UnityEngine;
using PaperSouls.Runtime.Inventory;

namespace PaperSouls.Runtime.UI.Inventory
{
    public class DynamicInventoryDisplay : InventoryDisplay
    {
        [SerializeField] private InventorySlotsUI _slotPrefab;

        /// <summary>
        /// Refresh the inventory when a new external inventory is opened
        /// </summary>
        public void RefreshDynamicInventory(InventoryManger inventory)
        {
            ClearAllSLots();
            _inventoryManger = inventory;
            AssignSlot(inventory);
        }

        /// <summary>
        /// Clears all slots in the inventory
        /// </summary>
        private void ClearAllSLots()
        {
            foreach (Transform item in transform.Cast<Transform>())
            {
                GameObject.Destroy(item.gameObject);
            }

            if (_slotDictionary != null) _slotDictionary.Clear();
        }

        public override void AssignSlot(InventoryManger inventory)
        {
            _slotDictionary = new();

            if (inventory == null) return;

            for (int i = 0; i < inventory.NumOfInventorySlots; i++)
            {
                InventorySlotsUI slot = Instantiate(_slotPrefab, transform);
                _slotDictionary.Add(slot, inventory.InventorySlots[i]);
                slot.Init(inventory.InventorySlots[i]);
                slot.UpdateSlot();
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}
