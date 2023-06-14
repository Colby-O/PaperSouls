using UnityEngine;
using UnityEngine.InputSystem;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.UI.Inventory;
using PaperSouls.Runtime.UI.View;

namespace PaperSouls.Core 
{ 
    [RequireComponent(typeof(ViewManger), typeof(PlayerInput))]
    public class UIManger : MonoBehaviour
    {
        private static UIManger _instance;
        private static readonly object Padlock = new();
        public static UIManger Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new();
                    }

                    return _instance;
                }
            }
        }

        [SerializeField] private DynamicInventoryDisplay _inventoryDisplay;

        private PlayerInput _uiInput;

        private InputAction _menuAction;
        private InputAction _closeAction;

        /// <summary>
        /// Displays an external inventory such as chests. On get called when 
        /// InventoryHolder.OnDynamicInventoryDisplayRequest Action is Invoked.
        /// </summary>
        void DisplayInventory(InventoryManger inventory)
        {
            if (!OpenExternalInventory()) return;
        
            _inventoryDisplay.RefreshDynamicInventory(inventory);
        }
        /// <summary>
        /// Displays an external inventory such as chests
        /// </summary>
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

        /// <summary>
        /// Close out of the current menu
        /// </summary>
        static void CloseCurrent()
        {
            if (!ViewManger.GetCurrentViewIs<PlayerHUDView>())
            {
                ViewManger.ShowLast();
                if (ViewManger.GetCurrentViewIs<PlayerHUDView>()) GameManger.UpdateGameState(GameState.Playing);
            }
        }

        /// <summary>
        /// Toogle the inventory (Opens/Closes)
        /// </summary>
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
            _instance = this;
        }

        private void Start()
        {
            _uiInput = GetComponent<PlayerInput>();

            _menuAction = _uiInput.actions["Inventory"];
            _closeAction = _uiInput.actions["Close"];

            _menuAction.performed += e => ToggleInventory();
            _closeAction.performed += e => CloseCurrent();

        }
    }
}
