using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.UI.Inventory
{
    public class InventorySlotsUI : MonoBehaviour
    {
        [SerializeField] private Image _itemSprite;
        [SerializeField] private TextMeshProUGUI _itemCount;
        public InventorySlot InventorySlot;
        public InventoryDisplay Display { get; set; }

        private GUIClickController _button;

        /// <summary>
        /// Initializes UI inventory slot with a InventorySlot
        /// </summary>
        public void Init(InventorySlot slot)
        {
            InventorySlot = slot;
            UpdateSlot(slot);
        }

        /// <summary>
        /// Update the inventory slots data given a InventorySlot
        /// </summary>
        public void UpdateSlot(InventorySlot slot)
        {
            if (slot.ItemData != null)
            {
                _itemSprite.sprite = slot.ItemData.icon;
                _itemSprite.color = Color.white;
                _itemCount.text = (slot.StackSize > 1) ? slot.StackSize.ToString() : "";
            }
            else ClearSlot();
        }

        /// <summary>
        /// Update the inventory slots data with currently assigned Slot
        /// </summary>
        public void UpdateSlot()
        {
            if (InventorySlot != null) UpdateSlot(InventorySlot);
        }

        /// <summary>
        /// Process a OnClick event
        /// </summary>
        public void OnSlotClick()
        {
            Display?.SlotClicked(this);
        }

        /// <summary>
        /// Clears the inventory slot
        /// </summary>
        public void ClearSlot()
        {
            InventorySlot?.ClearSlot();
            _itemSprite.sprite = null;
            _itemSprite.color = Color.clear;
            _itemCount.text = "";
        }

        /// <summary>
        /// Uses a UseableItem
        /// </summary>
        private void UseItem(UseableItem item)
        {
            PlayerManger player = FindObjectOfType<PlayerManger>();
            item.Use(player, out bool sucessful);

            if (!sucessful) return;

            if (InventorySlot.StackSize > 1) InventorySlot.RemoveFromStack(1);
            else InventorySlot.ClearSlot();

            UpdateSlot();
        }

        /// <summary>
        /// Interact with item in slot
        /// </summary>
        public void InteractWithSlot()
        {
            if (InventorySlot.ItemData != null)
            {
                Item item = InventorySlot.ItemData;
                if (item is UseableItem) UseItem(item as UseableItem);
            }
        }

        private void Awake()
        {
            ClearSlot();

            _button = GetComponent<GUIClickController>();
            if (_button == null) _button = gameObject.AddComponent(typeof(GUIClickController)) as GUIClickController;

            _button?.OnLeft.AddListener(OnSlotClick);
            _button?.OnRight.AddListener(InteractWithSlot);

            Display = transform.parent.GetComponent<InventoryDisplay>();
        }
    }
}
