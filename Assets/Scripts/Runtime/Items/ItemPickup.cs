using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaperSouls.Core;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Player;
using PaperSouls.Runtime.MonoSystems.Audio;

namespace PaperSouls.Runtime.Items
{
    [RequireComponent(typeof(SphereCollider))]
    internal class ItemPickup : MonoBehaviour
    {
        [SerializeField] private float _pickUpRadius = 0.1f;
        [SerializeField] private Item _itemData;

        private SphereCollider _itemCollider;

        private void Awake()
        {
            _itemCollider = GetComponent<SphereCollider>();
            _itemCollider.isTrigger = true;
            _itemCollider.radius = _pickUpRadius;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (_itemData.pickupType)
            {
                case PickupType.Inventory:
                    InventoryHolder inventory = other.transform.GetComponentInChildren<InventoryHolder>();

                    if (!inventory) return;

                    if (inventory.InventoryManger.AddToInventory(_itemData, 1))
                    {
                        GameManager.Emit<PlayAudioMessage>(new("Item Pickup", MonoSystems.Audio.AudioType.SfX));
                        GameObject.Destroy(this.gameObject);
                    }
                    break;
                case PickupType.Ammo:
                    PlayerManger player = other.transform.GetComponentInChildren<PlayerManger>();

                    if (!player) return;

                    player.PlayerHUD.IncrementAmmoCount();
                    GameManager.Emit<PlayAudioMessage>(new("Item Pickup", MonoSystems.Audio.AudioType.SfX));
                    GameObject.Destroy(this.gameObject);
                    break;
                default:
                    Debug.Log("Invaild Item Pickup Type!!!!");
                    break;
            }
        }
    }
}
