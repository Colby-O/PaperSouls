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
        [SerializeField] private float _attractRadius = 2.0f;
        [SerializeField] private float _attractSpeed = 1f;
        [SerializeField] private Item _itemData;

        private float _attrachDiableTime = 1.0f;
        private float _timeAttrachDisabledFor;
        private bool _isAttrachDiable = false;

        private SphereCollider _itemCollider;

        public void SetAttractSpeed(float attractSpeed) => _attractSpeed = attractSpeed;

        private void Awake()
        {
            _itemCollider = GetComponent<SphereCollider>();
            _itemCollider.isTrigger = true;
            _itemCollider.radius = _pickUpRadius;
        }

        private void Update()
        {
            if (_isAttrachDiable)
            {
                if (_timeAttrachDisabledFor >= _attrachDiableTime) _isAttrachDiable = false;
                else _timeAttrachDisabledFor += Time.deltaTime;
                return;
            }

            if ((PaperSoulsGameManager.Player.transform.position - transform.position).magnitude <= 0.01f)
            {
                _isAttrachDiable = true;
                _timeAttrachDisabledFor = 0.0f;
            }
            else if ((PaperSoulsGameManager.Player.transform.position - transform.position).magnitude <= _attractRadius)
            {
                transform.position += (PaperSoulsGameManager.Player.transform.position - transform.position).normalized * _attractSpeed * Time.deltaTime;
            }
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
