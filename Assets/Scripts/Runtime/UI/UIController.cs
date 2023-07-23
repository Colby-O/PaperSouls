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
    public class UIController : MonoBehaviour
    {
        [SerializeField] private DynamicInventoryDisplay _inventoryDisplay;

        private IUIMonoSystem _uiMonoSystem;

        private PlayerInput _uiInput;

        private InputAction _menuAction;
        private InputAction _closeAction;
        private InputAction _consoleAction;

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
            else 
            {
                GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Paused));
                _uiMonoSystem.Show<PauseMenuView>();
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
            else if (_uiMonoSystem.GetCurrentViewIs<MenuInventoryView>()) CloseCurrent();
        }

        public void ToggleConsole()
        {
            if (_uiMonoSystem.GetCurrentViewIs<PlayerHUDView>())
            {
                GameManager.Emit<ChangeGameStateMessage>(new(GameStates.Paused));
                _uiMonoSystem.Show<DeveloperConsoleView>();
            }
            else if (_uiMonoSystem.GetCurrentViewIs<DeveloperConsoleView>()) CloseCurrent();
        }

        private void ToggleInventory(InputAction.CallbackContext e) => ToggleInventory();
       

        private void CloseCurrent(InputAction.CallbackContext e) => CloseCurrent();


        private void ToggleConsole(InputAction.CallbackContext e) => ToggleConsole();

        private void AddListeners()
        {
            InventoryHolder.OnDynamicInventoryDisplayRequest += DisplayInventory;
            _menuAction.performed += ToggleInventory;
            _closeAction.performed += CloseCurrent;
            _consoleAction.performed += ToggleConsole;
        }

        private void RemoveListeners()
        {
            InventoryHolder.OnDynamicInventoryDisplayRequest -= DisplayInventory;
            _menuAction.performed -= ToggleInventory;
            _closeAction.performed -= CloseCurrent;
            _consoleAction.performed -= ToggleConsole;
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void Awake()
        {
            _uiMonoSystem = GameManager.GetMonoSystem<IUIMonoSystem>();

            _uiInput = GetComponent<PlayerInput>();

            _menuAction = _uiInput.actions["Inventory"];
            _closeAction = _uiInput.actions["Close"];
            _consoleAction = _uiInput.actions["Console"];
        }
    }
}
