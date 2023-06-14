using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PaperSouls.Core;
using PaperSouls.Runtime.Sprite;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Items;

namespace PaperSouls.Runtime.Player
{

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : Billboard
    {
        [SerializeField] private float _turnSpeed = 10.0f;
        [SerializeField] private float _lookSpeed = 10.0f;
        [SerializeField] [Range(-90, 0)] private float _minViewY;
        [SerializeField] [Range(0, 90)] private float _maxViewY;
        [SerializeField] private Vector2 _sensitivity;
        [SerializeField] private float _jumpHeight = 10;
        [SerializeField] private float _jumpFalloff = 0.5f;
        [SerializeField] private float _gravityMultiplier = 3;
        [SerializeField] private UseableItem _quickUseItem;

        [SerializeField] private GameObject _playerUI;
        [SerializeField] private GameObject _inventoryUI;

        private PlayerInput _playerInput;
        private PlayerManger _playerManger;
        private CharacterController _characterController;
        private Animator _animator;

        private bool _isWalking;
        private bool _isSprinting;
        private Vector3 _jumpForce;
        private Vector3 _jumpVel;
        private float _gravity = -9.81f;
        private float _gravityForceOnPlayer = 0.0f;
        private float _verticalMovement;
        private float _horizontalMovement;

        private float _cameraOffsetZ;
        private float _cameraOffsetY;

        private float _mouseScrollY;

        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _zoomAction;
        private InputAction _jumpAction;
        private InputAction _sprintAction;
        private InputAction _quickUseAction;

        private Vector3 _targetDirection;
        private Vector2 _cameraAngle;

        private Vector2 _rawMovementInput;
        private Vector2 _rawMousePosition;

        /// <summary>
        /// Gets the direction the player is facing
        /// </summary>
        void GetPlayerTargetDirection()
        {
            _horizontalMovement = _rawMovementInput.x;
            _verticalMovement = _rawMovementInput.y;

            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;

            Vector3 right = Camera.main.transform.TransformDirection(Vector3.right);

            _targetDirection = _horizontalMovement * right + _verticalMovement * forward;

            _isWalking = _targetDirection.magnitude > 0;
        }

        /// <summary>
        /// Processes the players movement
        /// </summary>
        void ProcessPlayerMovement()
        {
            Vector3 playerMovement = _targetDirection * ((_isSprinting) ? _playerManger.PlayerSettings.playerSpeedRunning : _playerManger.PlayerSettings.playerSpeedWalking);

            _characterController.Move(playerMovement);
        }

        /// <summary>
        /// Apply a gravity force to the player
        /// </summary>
        void ApplyGravity()
        {
            if (_characterController.isGrounded && _gravityForceOnPlayer < 0.0f) _gravityForceOnPlayer = -1.0f;
            else _gravityForceOnPlayer += _gravity * _gravityMultiplier * Time.deltaTime;
            _targetDirection.y = _gravityForceOnPlayer;
        }

        /// <summary>
        /// Process a jump
        /// </summary>
        void ProcessJump()
        {
            _jumpForce = Vector3.SmoothDamp(_jumpForce, Vector3.zero, ref _jumpVel, _jumpFalloff);
            _targetDirection += _jumpForce;
        }

        /// <summary>
        /// Mkae the player Jump
        /// </summary>
        void Jump()
        {
            if (_characterController.isGrounded && GameManger.AccpetPlayerInput)
            {
                _jumpForce = Vector3.up * _jumpHeight;
                _gravityForceOnPlayer = 0;
            }
        }

        /// <summary>
        /// Rotates around the player
        /// </summary>
        void RotateAroundTarget(Vector3 axis, float angle)
        {
            Camera.main.transform.RotateAround(transform.position, axis, angle);
        }

        /// <summary>
        /// Makes the camera follow the player
        /// </summary>
        void FollowPlayer()
        {
            Vector3 cameraPosition = new(transform.position.x, transform.position.y + _cameraOffsetY, transform.position.z + _cameraOffsetZ);
            Camera.main.transform.position = cameraPosition;
            Camera.main.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Check if there is any object in the way of the cameras view of the player
        /// </summary>
        void CheckCameriaCollision()
        {
            Vector3 direction = Camera.main.transform.position - transform.position;
            Ray ray = new(transform.position, direction.normalized);

            Debug.DrawLine(ray.origin, ray.origin + direction.magnitude * ray.direction, Color.blue);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, direction.magnitude))
            {
                if (hitInfo.transform.tag.CompareTo("Player") == 1) Camera.main.transform.position = hitInfo.point;
            }
        }

        /// <summary>
        /// Updates the angle of the camera
        /// </summary>
        void UpdateCameraAngle()
        {
            _cameraAngle += _rawMousePosition;
            _cameraAngle.y = Mathf.Clamp(_cameraAngle.y, _minViewY, _maxViewY);
        }

        /// <summary>
        /// Process how far the camera should orbit the player
        /// </summary>
        void ProcessCameraZoom()
        {
            if (_mouseScrollY > 0) _cameraOffsetZ += 1;
            else if (_mouseScrollY < 0) _cameraOffsetZ -= 1;
        }

        /// <summary>
        /// Process all camera movement
        /// </summary>
        void ProcessCamera()
        {
            ProcessCameraZoom();
            FollowPlayer();
            UpdateCameraAngle();
            RotateAroundTarget(Camera.main.transform.up, _cameraAngle.x);
            RotateAroundTarget(-Camera.main.transform.right, _cameraAngle.y);
            CheckCameriaCollision();
        }

        /// <summary>
        /// Process all player movement
        /// </summary>
        void ProcessPlayer()
        {
            GetPlayerTargetDirection();
            ApplyGravity();
            ProcessJump();
            ProcessPlayerMovement();
        }

        /// <summary>
        /// Walking animation
        /// </summary>
        void HandleAnimations()
        {
            _animator.SetBool("isWalking", _isWalking);
            _animator.speed = (_isSprinting) ? 2 : 1;
        }

        /// <summary>
        /// Use a Useable item selected to quick use
        /// </summary>
        void QickUse()
        {
            InventoryHolder inventoryHolder = GetComponentInChildren<InventoryHolder>();
            if (inventoryHolder == null) return;

            InventorySlot slot = inventoryHolder.InventoryManger.InventorySlots.Find(e => (e.ItemData != null) ? e.ItemData.id == _quickUseItem.id : false);
            if (slot == null) return;

            UseableItem item = slot.ItemData as UseableItem;
            if (item == null) return;

            item.Use(_playerManger, out bool sucessful);
            if (!sucessful) return;

            if (slot.StackSize > 1) slot.RemoveFromStack(1);
            else slot.ClearSlot();

            inventoryHolder.InventoryManger.OnInventoryChange?.Invoke(slot);
        }

        public override void Awake()
        {
            base.Awake();

            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _playerManger = GetComponent<PlayerManger>();
            _animator = GetComponentInChildren<Animator>();

            _playerUI.SetActive(true);
            _inventoryUI.SetActive(true);

            _isWalking = false;
            _isSprinting = false;

            _cameraOffsetZ = Camera.main.transform.position.z - transform.position.z;
            _cameraOffsetY = Camera.main.transform.position.y - transform.position.y;
            _cameraAngle = new(0, 0);

            _moveAction = _playerInput.actions["Movement"];
            _lookAction = _playerInput.actions["View"];
            _zoomAction = _playerInput.actions["Zoom"];
            _jumpAction = _playerInput.actions["Jump"];
            _sprintAction = _playerInput.actions["Sprint"];
            _quickUseAction = _playerInput.actions["QuickUse"];

            _moveAction.performed += e => _rawMovementInput = (GameManger.AccpetPlayerInput) ? e.ReadValue<Vector2>() : Vector2.zero;
            _lookAction.performed += e => _rawMousePosition = (GameManger.AccpetPlayerInput) ? e.ReadValue<Vector2>() : Vector2.zero;
            _zoomAction.performed += e => _mouseScrollY = (GameManger.AccpetPlayerInput) ? e.ReadValue<float>() : 0.0f;
            _jumpAction.performed += e => Jump();
            _sprintAction.performed += e => _isSprinting = !_isSprinting;
            _sprintAction.canceled += e => _isSprinting = !_isSprinting;
            _quickUseAction.performed += e => QickUse();

            _playerInput.actions.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            _playerUI.SetActive(true);
            _inventoryUI.SetActive(false);
        }

        void Update()
        {
            ProcessPlayer();
            ProcessCamera();
            HandleAnimations();
        }
    }
}
