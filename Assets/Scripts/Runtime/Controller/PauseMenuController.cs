using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using PaperSouls.Core;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.UI.Inventory;
using PaperSouls.Runtime.UI.View;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.MonoSystems.UI;

namespace PaperSouls.Runtime.UI 
{ 
    [RequireComponent(typeof(PlayerInput))]
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private DynamicInventoryDisplay _inventoryDisplay;

        private IUIMonoSystem _uiMonoSystem;

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
        public bool OpenExternalInventory()
        {
            if (_uiMonoSystem.GetCurrentViewIs<PlayerHUDView>())
            {
                _uiMonoSystem.Show<ExternalInventoryView>();
                GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Paused));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Close out of the current menu
        /// </summary>
        public void CloseCurrent()
        {
            if (!_uiMonoSystem.GetCurrentViewIs<PlayerHUDView>())
            {
                _uiMonoSystem.ShowLast();
                if (_uiMonoSystem.GetCurrentViewIs<PlayerHUDView>()) GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Playing));
            }
        }

        /// <summary>
        /// Toogle the inventory (Opens/Closes)
        /// </summary>
        public void ToggleInventory()
        {
            if (_uiMonoSystem.GetCurrentViewIs<PlayerHUDView>())
            {
                GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Paused));
                _uiMonoSystem.Show<MenuInventoryView>();
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
            _uiMonoSystem = GameManager.GetMonoSystem<IUIMonoSystem>();

            _uiInput = GetComponent<PlayerInput>();

            _menuAction = _uiInput.actions["Inventory"];
            _closeAction = _uiInput.actions["Close"];

            _menuAction.performed += e => ToggleInventory();
            _closeAction.performed += e => CloseCurrent();
        }
    }
}
