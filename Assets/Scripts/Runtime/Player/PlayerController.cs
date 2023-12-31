using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using PaperSouls.Core;
using PaperSouls.Runtime.Sprite;
using PaperSouls.Runtime.Inventory;
using PaperSouls.Runtime.Items;
using PaperSouls.Runtime.MonoSystems.Audio;
using PaperSouls.Runtime.MonoSystems.GameState;
using PaperSouls.Runtime.MonoSystems.DungeonGeneration;

namespace PaperSouls.Runtime.Player
{

    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    internal sealed class PlayerController : Billboard
    {
        [SerializeField] private float _turnSpeed = 10.0f;
        [SerializeField] private float _lookSpeed = 10.0f;
        [SerializeField] [Range(-90, 0)] private float _minViewY;
        [SerializeField] [Range(0, 90)] private float _maxViewY;
        [SerializeField] private Vector2 _sensitivity;
        [SerializeField] private float _dashStrength = 10;
        [SerializeField] private float _dashFalloff = 0.5f;
        [SerializeField] private float _jumpHeight = 10;
        [SerializeField] private float _gravityMultiplier = 3;
        [SerializeField] private UseableItem _quickUseItem;

        [SerializeField] private GameObject _playerUI;
        [SerializeField] private GameObject _inventoryUI;
        [SerializeField] private GameObject _dashTrail;

        private PlayerInput _playerInput;
        private PlayerManger _playerManger;
        private CharacterController _characterController;
        private Animator _animator;

        private bool _isWalking;
        private bool _isSprinting;
        private Vector3 _dashForce;
        private Vector3 _dashVel;
        private float _gravity = -9.81f;
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
        private InputAction _dashAction;
        private InputAction _quickUseAction;

        private Vector3 _playerVelocity;
        private Vector2 _cameraAngle;

        private Vector2 _rawMovementInput;
        private Vector2 _rawMousePosition;

        private bool _dashing = false;

        /// <summary>
        /// Gets the direction the player is facing
        /// </summary>
        private void GetPlayerTargetDirection()
        {
            _horizontalMovement = _rawMovementInput.x;
            _verticalMovement = _rawMovementInput.y;

            Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;

            Vector3 right = Camera.main.transform.TransformDirection(Vector3.right);
            right.y = 0;
            right = right.normalized;

            Vector3 movement = (
                (_horizontalMovement * right + _verticalMovement * forward).normalized *
                ((_isSprinting) ? _playerManger.PlayerSettings.playerSpeedRunning : _playerManger.PlayerSettings.playerSpeedWalking)
            );

            _playerVelocity = new Vector3(
                movement.x,
                _playerVelocity.y,
                movement.z
            );

            _isWalking = _playerVelocity.magnitude > 0;
        }

        /// <summary>
        /// Applys the players movement
        /// </summary>
        private void ApplyPlayerMovement()
        {
            _characterController.Move(_playerVelocity);
        }

        /// <summary>
        /// Apply a gravity force to the player
        /// </summary>
        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _playerVelocity.y < 0.0f) _playerVelocity.y = -1.0f;
            else _playerVelocity.y += _gravity * _gravityMultiplier;
        }

        /// <summary>
        /// Makes the player Jump
        /// </summary>
        private void Jump()
        {
            if (_characterController.isGrounded && PaperSoulsGameManager.AccpetPlayerInput)
            {
                _playerVelocity.y = _jumpHeight;
            }
        }

        /// <summary>
        /// Rotates around the player
        /// </summary>
        private void RotateAroundTarget(Vector3 axis, float angle)
        {
            Camera.main.transform.RotateAround(transform.position, axis, angle);
        }

        /// <summary>
        /// Makes the camera follow the player
        /// </summary>
        private void FollowPlayer()
        {
            Vector3 cameraPosition = new(transform.position.x, transform.position.y + _cameraOffsetY, transform.position.z + _cameraOffsetZ);
            Camera.main.transform.position = cameraPosition;
            Camera.main.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// calculates the raycast to the camera with an offset
        /// and returns the distance and the resulting position
        /// of the camera if there was a collision
        /// </summary>
        private (float distance, Vector3 newPos) CalculateCameraCollisionWithOffset(Vector3 offset)
        {
            Vector3 direction = Camera.main.transform.position + offset - transform.position;
            Ray ray = new(transform.position, direction.normalized);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, direction.magnitude))
            {
                if (hitInfo.transform.tag.CompareTo("Player") == 1) {
                    return (hitInfo.distance, hitInfo.point - offset);
                }
            }

            return (float.MaxValue, Vector3.zero);
        }

        /// <summary>
        /// Check if there is any object in the way of the cameras view of the player
        /// </summary>
        private void CheckCameriaCollision()
        {
            Camera cam = Camera.main;
            // calculate the size of the viewport in world space
            float viewHeight = 2.0f * Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView * 0.5f) * cam.nearClipPlane;
            float viewWidth = viewHeight / cam.pixelHeight * cam.pixelWidth;

            // check all 4 corners of the camera viewport in world space
            // to insure they are not intersecting any objects
            (float distance, Vector3 newPos)[] corners = {
                CalculateCameraCollisionWithOffset(
                    -cam.transform.right * viewWidth / 2.0f + cam.transform.up * viewHeight / 2.0f
                ),
                CalculateCameraCollisionWithOffset(
                    cam.transform.right * viewWidth / 2.0f + cam.transform.up * viewHeight / 2.0f
                ),
                CalculateCameraCollisionWithOffset(
                    -cam.transform.right * viewWidth / 2.0f - cam.transform.up * viewHeight / 2.0f
                ),
                CalculateCameraCollisionWithOffset(
                    cam.transform.right * viewWidth / 2.0f - cam.transform.up * viewHeight / 2.0f
                ),
            };

            if (corners.Any(c => c.distance != float.MaxValue))
            {
                // set camera pos to the closest intersection
                Camera.main.transform.position = corners.Aggregate(
                    (m, n) => m.distance < n.distance ? m : n
                ).newPos;
            }
        }

        /// <summary>
        /// Updates the angle of the camera
        /// </summary>
        private void UpdateCameraAngle()
        {
            _cameraAngle += _rawMousePosition * _sensitivity;
            _cameraAngle.y = Mathf.Clamp(_cameraAngle.y, _minViewY, _maxViewY);
        }

        /// <summary>
        /// Process how far the camera should orbit the player
        /// </summary>
        private void ProcessCameraZoom()
        {
            if (_mouseScrollY > 0) _cameraOffsetZ += 1;
            else if (_mouseScrollY < 0) _cameraOffsetZ -= 1;
        }

        /// <summary>
        /// Process all camera movement
        /// </summary>
        private void ProcessCamera()
        {
            ProcessCameraZoom();
            FollowPlayer();
            UpdateCameraAngle();
            RotateAroundTarget(Camera.main.transform.up, _cameraAngle.x);
            RotateAroundTarget(-Camera.main.transform.right, _cameraAngle.y);
            if(!PaperSoulsGameManager.DisableCameraCollsions) CheckCameriaCollision();
        }

        /// <summary>
        /// Process all player movement
        /// </summary>
        private void ProcessPlayer()
        {
            GetPlayerTargetDirection();
            ProcessDash();
        }

        /// <summary>
        /// Walking animation
        /// </summary>
        private void HandleAnimations()
        {
            _animator.SetBool("isWalking", _isWalking);
            _animator.speed = (_isSprinting) ? 2 : 1;
        }

        private void Dash()
        {
            _dashing = true;
            _dashTrail.SetActive(true);
            GameManager.Emit<PlayAudioMessage>(new("Dash", MonoSystems.Audio.AudioType.SfX));
            _dashForce = Camera.main.transform.TransformDirection(Vector3.forward) * _dashStrength;
            
        }

        private void ProcessDash()
        {
            _dashForce = Vector3.SmoothDamp(_dashForce, Vector3.zero, ref _dashVel, _dashFalloff);
            if (_dashForce.magnitude <= 0.01f)
            {
                _dashing = false;
                _dashTrail.SetActive(false);
            }
            _playerVelocity += _dashForce;
        }

        /// <summary>
        /// Use a Useable item selected to quick use
        /// </summary>
        private void QickUse()
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

        public void TeleportTo(Vector3 pos)
        {
            GameManager.Emit<StopChunkLoadingMessage>(new());
            _characterController.enabled = false;

            transform.position = pos;

            _characterController.enabled = true;
            GameManager.Emit<StartChunkLoadingMessage>(new());
        }

        protected override void Awake()
        {
            base.Awake();

            _characterController = GetComponent<CharacterController>();
            _playerInput = GetComponent<PlayerInput>();
            _playerManger = GetComponent<PlayerManger>();
            _animator = GetComponentInChildren<Animator>();

            _characterController.enabled = false;
        }

        protected override void Start()
        {
            base.Start();

            _characterController.enabled = true;

            _dashTrail.SetActive(false);

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
            _dashAction = _playerInput.actions["Dash"];
            _quickUseAction = _playerInput.actions["QuickUse"];

            _moveAction.performed += e => _rawMovementInput = (PaperSoulsGameManager.AccpetPlayerInput) ? e.ReadValue<Vector2>() : Vector2.zero;
            _lookAction.performed += e => _rawMousePosition = (PaperSoulsGameManager.AccpetPlayerInput) ? e.ReadValue<Vector2>() : Vector2.zero;
            _zoomAction.performed += e => _mouseScrollY = (PaperSoulsGameManager.AccpetPlayerInput) ? e.ReadValue<float>() : 0.0f;
            _jumpAction.performed += e => Jump();
            _sprintAction.performed += e => _isSprinting = !_isSprinting;
            _sprintAction.canceled += e => _isSprinting = !_isSprinting;
            _quickUseAction.performed += e => QickUse();
            _dashAction.performed += e => Dash();

            _playerInput.actions.Enable();
        }

        void Update()
        {
            if (GameManager.GetMonoSystem<IGameStateMonoSystem>().GetCurrentState() == GameStates.Dead || 
                GameManager.GetMonoSystem<IGameStateMonoSystem>().GetCurrentState() == GameStates.MainMenu ||
                !_characterController.enabled) return;
            ProcessPlayer();
            HandleAnimations();
        }

        void FixedUpdate()
        {
            if (!_characterController.enabled) return;
            ApplyGravity();
            ApplyPlayerMovement();
        }

        protected override void LateUpdate()
        {
            ProcessCamera();
            base.LateUpdate();
        }
    }
}
