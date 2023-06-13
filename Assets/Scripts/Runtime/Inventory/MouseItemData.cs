using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MouseItemData : MonoBehaviour
{
    public Image sprite;
    public TextMeshProUGUI count;
    public InventorySlot inventorySlot;

    public InventorySlotsUI fromSlot;

    public void ReturnItem()
    {
        Debug.Log("Here!");
        fromSlot?.inventorySlot.AssignItem(inventorySlot);
        fromSlot?.UpdateSlot();
        ClearSlot();
    }

    public void ClearSlotData()
    {
        inventorySlot?.ClearSlot();
        inventorySlot.slotType = SlotType.Any;
        sprite.color = Color.clear;
        count.text = "";
        sprite.sprite = null;
    }

    public void ClearSlot()
    {
        ClearSlotData();
        fromSlot = null;
    }

    public void UpdateSlot(InventorySlot slot)
    {
        inventorySlot.AssignItem(slot);
        inventorySlot.slotType = slot.itemData.slotType;
        sprite.sprite = slot.itemData.icon;
        sprite.color = Color.white;
        count.text = slot.stackSize.ToString();
    }

    public void UpdateSlot(InventorySlotsUI fromSlot)
    {
        this.fromSlot = fromSlot;
        UpdateSlot(fromSlot.inventorySlot);
    }

    public void UpdateSlot(InventorySlot slot, InventorySlotsUI fromSlot)
    {
        this.fromSlot = fromSlot;
        UpdateSlot(slot);
    }

    private void DropItems()
    {
        for (int i = 0; i < inventorySlot.stackSize; i++)
        {
            Vector3 playerFrontLocaton = GameManger.Instance.player.transform.position + 2 * GameManger.Instance.player.transform.forward;
            Vector3 dropLocaton = new Vector3(playerFrontLocaton.x + 0.1f * i, 0, playerFrontLocaton.z + 0.1f * i);
            GameObject.Instantiate(inventorySlot.itemData.itemPrefab, new Vector3(dropLocaton.x, 0, dropLocaton.z), Quaternion.identity);
        }

        ClearSlot();
    }

    private void FollowCursor()
    {
        if (inventorySlot.itemData != null)
        {
            transform.position = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUIObject()) DropItems();
        }
    }

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
