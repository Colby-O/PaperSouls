using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ViewManger), typeof(PlayerInput))]
public class UIManger : MonoBehaviour
{
    private static UIManger instance;
    public static UIManger Instance
    {
        get
        {
            if (instance == null) Debug.Log("UI Manger is null!!!");

            return instance;
        }

        private set { }
    }

    public DynamicInventoryDisplay inventoryDisplay;

    private PlayerInput uiInput;

    private InputAction menuAction;
    private InputAction closeAction;

    void DisplayInventory(InventoryManger inventory)
    {
        if (!OpenExternalInventory()) return;
        
        inventoryDisplay.RefreshDynamicInventory(inventory);
    }

    public static bool OpenExternalInventory()
    {
        if (ViewManger.GetCurrentViewIs<PlayerHUDView>())
        {
            ViewManger.Show<ExternalInventoryView>();
            GameManger.UpdateGameState(GameState.InMenu);
            return true;
        }

        return false;
    }

    static void CloseCurrent()
    {
        if (!ViewManger.GetCurrentViewIs<PlayerHUDView>())
        {
            ViewManger.ShowLast();
            if (ViewManger.GetCurrentViewIs<PlayerHUDView>()) GameManger.UpdateGameState(GameState.Playing);
        }
    }

    static void ToggleInventory()
    {
        if (ViewManger.GetCurrentViewIs<PlayerHUDView>())
        {
            GameManger.UpdateGameState(GameState.InMenu);
            ViewManger.Show<MenuInventoryView>();
        }
        else CloseCurrent();
    }

    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequest += DisplayInventory;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequest -= DisplayInventory;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        uiInput = GetComponent<PlayerInput>();

        menuAction = uiInput.actions["Inventory"];
        closeAction = uiInput.actions["Close"];

        menuAction.performed += e => ToggleInventory();
        closeAction.performed += e => CloseCurrent();

    }
}
