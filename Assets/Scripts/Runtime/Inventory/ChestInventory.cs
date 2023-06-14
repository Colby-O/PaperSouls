using UnityEngine;
using UnityEngine.Events;
using PaperSouls.Runtime.Interfaces;
using PaperSouls.Runtime.Helpers;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.Player;

namespace PaperSouls.Runtime.Inventory
{
    public class ChestInventory : InventoryHolder, IInteractable
    {
        public UnityAction<IInteractable> OnInteractionComplete { get; set; }

        public bool spawnLoot = true;
        public LootTable lootTable = null;
        [Range(0f, 1f)] public float lootSpawnProbability = 0.3f;

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

        public void Interact(Interactor interactor, out bool successful)
        {
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

            if (spawnLoot && lootTable != null) SpawnLoot();
        }
    }
}
