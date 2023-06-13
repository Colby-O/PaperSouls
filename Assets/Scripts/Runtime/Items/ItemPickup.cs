using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ItemPickup : MonoBehaviour
{
    public float pickUpRadius = 0.1f;
    public Item itemData;

    private SphereCollider itemCollider;

    private void Awake()
    {
        itemCollider = GetComponent<SphereCollider>();
        itemCollider.isTrigger = true;
        itemCollider.radius = pickUpRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (itemData.pickupType)
        {
            case PickupType.Inventory:
                InventoryHolder inventory = other.transform.GetComponentInChildren<InventoryHolder>();

                if (!inventory) return;

                if (inventory.inventoryManger.AddToInventory(itemData, 1))
                {
                    if (AudioManger.Instance != null) AudioManger.Instance.PlaySFX("Item Pickup");
                    GameObject.Destroy(this.gameObject);
                }
                break;
            case PickupType.Ammo:
                PlayerManger player = other.transform.GetComponentInChildren<PlayerManger>();

                if (!player) return;

                player.playerHUD.IncrementAmmoCount();
                if (AudioManger.Instance != null) AudioManger.Instance.PlaySFX("Item Pickup");
                GameObject.Destroy(this.gameObject);
                break;
            default:
                Debug.Log("Invaild Item Pickup Type!!!!");
                break;
        }
    }
}
