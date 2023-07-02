using UnityEngine;
using UnityEngine.Events;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Helpers;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Inventory
{
    [RequireComponent(typeof(Animator))]
    public class ChestInventory : InventoryHolder, IInteractable
    {
        public UnityAction<IInteractable> OnInteractionComplete { get; set; }

        public Item itemRequiredToUnlock;
        public bool spawnLoot = true;
        public LootTable lootTable = null;
        [Range(0f, 1f)] public float lootSpawnProbability = 0.3f;

        [SerializeField] private bool _isLocked = true;
        private Animator _animator;

        /// <summary>
        /// Spawns Loot within the inventory
        /// </summary>
        protected void SpawnLoot()
        {
            int inventorySize = InventoryManger.NumOfInventorySlots;

            for (int i = 0; i < inventorySize; i++)
            {
                if (lootSpawnProbability >= Random.value)
                {
                    Item item = lootTable.GetItem();
                    InventoryManger.AddToInventory(item, RandomGenerator.GetRandomSkewed(1, item.maxStackSize), i);
                }
            }
        }

        private void OnUnlock()
        {
            _animator.SetBool("Locked", false);
            Debug.Log("chest unlock");
        }

        private void OnFailedUnlock()
        {
            Debug.Log("failed to unlock chest");
        }

        public void Interact(Interactor interactor, out bool successful)
        {
            InventoryManger inv = interactor.gameObject.GetComponentInChildren<InventoryHolder>().InventoryManger;
            if (_isLocked) {
                if (inv.HasItem(itemRequiredToUnlock)) {
                    inv.TakeItem(itemRequiredToUnlock, 1);
                    OnUnlock();
                    _isLocked = false;
                } else {
                    OnFailedUnlock();
                    successful = true;
                    return;
                }
            }

            OnDynamicInventoryDisplayRequest?.Invoke(InventoryManger);

            successful = true;
        }

        public void EndInteraction()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        protected override void Awake()
        {
            base.Awake();
            ResizeInventory(_inventorySize);
            _animator = GetComponent<Animator>();
            _animator.SetBool("Locked", true);

            if (spawnLoot && lootTable != null) SpawnLoot();
        }
    }
}
