using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }

    public bool spawnLoot = true;
    public LootTable lootTable = null;
    [Range(0f, 1f)] public float lootSpawnProbability = 0.3f;

    public void Interact(Interactor interactor, out bool successful)
    {
        OnDynamicInventoryDisplayRequest?.Invoke(inventoryManger);

        successful = true;
    }
    public void EndInteraction()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    protected void SpawnLoot()
    {
        int inventorySize = inventoryManger.numOfInventorySlots;

        for (int i = 0; i < inventorySize; i++)
        {
            if (lootSpawnProbability >= Random.value)
            {
                Item item = lootTable.GetItem();
                inventoryManger.AddToInventory(item, RandomGenerator.GetRandomSkewed(1, item.maxStackSize), i);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        ResizeInventory(inventorySize);

        if (spawnLoot && lootTable != null) SpawnLoot();
    }
}
