using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotsUI : MonoBehaviour
{
    public Image itemSprite;
    public TextMeshProUGUI itemCount;
    public InventorySlot inventorySlot;
    public InventoryDisplay display {get; private set;}

    private GUIClickController button;

    public void Init(InventorySlot slot)
    {
        inventorySlot = slot;
        UpdateSlot(slot);
    }

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot.itemData != null)
        {
            itemSprite.sprite = slot.itemData.icon;
            itemSprite.color = Color.white;
            itemCount.text = (slot.stackSize > 1) ? slot.stackSize.ToString() : "";
        }
        else ClearSlot();
    }

    public void UpdateSlot()
    {
        if (inventorySlot != null) UpdateSlot(inventorySlot);
    }

    public void OnSlotClick()
    {
        display?.SlotClicked(this);
    }

    public void ClearSlot()
    {
        inventorySlot?.ClearSlot();
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCount.text = "";
    }

    private void UseItem(UseableItem item)
    {
        PlayerManger player = FindObjectOfType<PlayerManger>();
        item.Use(player, out bool sucessful);

        if (!sucessful) return;

        if (inventorySlot.stackSize > 1) inventorySlot.RemoveFromStack(1);
        else inventorySlot.ClearSlot();

        UpdateSlot();
    }

    public void InteractWithSlot()
    {
        if (inventorySlot.itemData != null)
        {
            Item item = inventorySlot.itemData;
            if (item is UseableItem) UseItem(item as UseableItem);
        }
    }

    private void Awake()
    {
        ClearSlot();

        button = GetComponent<GUIClickController>();
        if (button == null) button = this.gameObject.AddComponent(typeof(GUIClickController)) as GUIClickController;

        button?.onLeft.AddListener(OnSlotClick);
        button?.onRight.AddListener(InteractWithSlot);

        display = transform.parent.GetComponent<InventoryDisplay>();
    }
}
