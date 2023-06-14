using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.Inventory
{
    [System.Serializable]
    public class InventoryHolder : MonoBehaviour
    {
        [SerializeField] protected Vector2Int _inventorySize;
        [SerializeField] protected List<SlotType> _slotTypes;
        public InventoryManger InventoryManger;

        public static UnityAction<InventoryManger> OnDynamicInventoryDisplayRequest;

        /// <summary>
        /// Resizes the inventory given a Vector2Int (width, height)
        /// </summary>
        public void ResizeInventory(Vector2Int inventorySize)
        {
            InventoryManger = new(inventorySize);

            if (_slotTypes.Count != InventoryManger.InventorySlots.Count) return;

            for (int i = 0; i < _slotTypes.Count; i++)
            {
                InventoryManger.InventorySlots[i].InventorySlotType = _slotTypes[i];
            }
        }

        protected virtual void Awake() { }
    }
}
