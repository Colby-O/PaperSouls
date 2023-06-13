using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : Billboard
{
    public float turnSpeed = 10.0f;
    public float lookSpeed = 10.0f;
    [Range(-90, 0)] public float minViewY;
    [Range(0, 90)] public float maxViewY;
    public Vector2 sensitivity;
    public float jumpHeight = 10;
    public float jumpFalloff = 0.5f;
    public float gravityMultiplier = 3;
    public UseableItem quickUseItem;

    public GameObject playerUI;
    public GameObject inventoryUI;

    private PlayerInput playerInput;
    private PlayerManger playerManger;
    private CharacterController characterController;
    private Animator animator;

    private bool isWalking;

    private bool isSprinting;
    private Vector3 jumpForce;
    private Vector3 jumpVel;
    private float gravity = -9.81f;
    private float gravityForceOnPlayer = 0.0f;
    private float verticalMovement;
    private float horizontalMovement;

    private float cameraOffsetZ;
    private float cameraOffsetY;

    private float mouseScrollY;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction zoomAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction quickUseAction;

    private Vector3 targetDirection;
    private Vector2 cameraAngle;

    private Vector2 rawMovementInput;
    [SerializeField] private Vector2 rawMousePosition;

    void GetPlayerTargetDirection()
    {
        horizontalMovement = rawMovementInput.x;
        verticalMovement = rawMovementInput.y;

        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        Vector3 right = Camera.main.transform.TransformDirection(Vector3.right);

        targetDirection = horizontalMovement * right + verticalMovement * forward;

        isWalking = targetDirection.magnitude > 0;
    }

    void ProcessPlayerMovement()
    { 
        Vector3 playerMovement = targetDirection * ((isSprinting) ? playerManger.playerSettings.playerSpeedRunning : playerManger.playerSettings.playerSpeedWalking);
        
        characterController.Move(playerMovement);
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded && gravityForceOnPlayer < 0.0f) gravityForceOnPlayer = -1.0f;
        else gravityForceOnPlayer += gravity * gravityMultiplier * Time.deltaTime;
        targetDirection.y = gravityForceOnPlayer;
    }

    void ProcessJump()
    {
        jumpForce = Vector3.SmoothDamp(jumpForce, Vector3.zero, ref jumpVel, jumpFalloff);
        targetDirection += jumpForce;
    }
    
    void Jump()
    {
        if (characterController.isGrounded && GameManger.accpetPlayerInput)
        {
            jumpForce = Vector3.up * jumpHeight;
            gravityForceOnPlayer = 0;
        }
    }

    void RotateAroundTarget(Vector3 axis, float angle)
    {
        Camera.main.transform.RotateAround(transform.position, axis, angle);
    }

    void FollowPlayer()
    {
        Vector3 cameraPosition = new(transform.position.x, transform.position.y + cameraOffsetY, transform.position.z + cameraOffsetZ);
        Camera.main.transform.position = cameraPosition;
        Camera.main.transform.rotation = Quaternion.identity;
    }

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

    void UpdateCameraAngle()
    {
        cameraAngle += rawMousePosition;
        cameraAngle.y = Mathf.Clamp(cameraAngle.y, minViewY, maxViewY);
    }

    void ProcessCameraZoom()
    {
        if (mouseScrollY > 0) cameraOffsetZ += 1;
        else if (mouseScrollY < 0) cameraOffsetZ -= 1;
    }

    void ProcessCamera()
    {
        ProcessCameraZoom();
        FollowPlayer();
        UpdateCameraAngle();
        RotateAroundTarget(Camera.main.transform.up, cameraAngle.x);
        RotateAroundTarget(-Camera.main.transform.right, cameraAngle.y);
        CheckCameriaCollision();
    }

    void ProcessPlayer()
    {
        GetPlayerTargetDirection();
        ApplyGravity();
        ProcessJump();
        ProcessPlayerMovement();
    }

    void HandleAnimations()
    {
        animator.SetBool("isWalking", isWalking);
        animator.speed = (isSprinting) ? 2 : 1;
    }

    void QickUse()
    {
        InventoryHolder inventoryHolder = GetComponentInChildren<InventoryHolder>();
        if (inventoryHolder == null) return;

        InventorySlot slot = inventoryHolder.inventoryManger.inventorySlots.Find(e => (e.itemData != null)  ? e.itemData.id == quickUseItem.id : false );
        if (slot == null) return;

        UseableItem item = slot.itemData as UseableItem;
        if (item == null) return;

        item.Use(playerManger, out bool sucessful);
        if (!sucessful) return;

        if (slot.stackSize > 1) slot.RemoveFromStack(1);
        else slot.ClearSlot();

        inventoryHolder.inventoryManger.OnInventoryChange?.Invoke(slot);
    }

    public override void Awake()
    {
        base.Awake();

        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        playerManger = GetComponent<PlayerManger>();
        animator = GetComponentInChildren<Animator>();

        playerUI.SetActive(true);
        inventoryUI.SetActive(true);

        isWalking = false;
        isSprinting = false;

        cameraOffsetZ = Camera.main.transform.position.z - transform.position.z;
        cameraOffsetY = Camera.main.transform.position.y - transform.position.y;
        cameraAngle = new(0, 0);

        moveAction = playerInput.actions["Movement"];
        lookAction = playerInput.actions["View"];
        zoomAction = playerInput.actions["Zoom"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
        quickUseAction = playerInput.actions["QuickUse"];

        moveAction.performed += e => rawMovementInput = (GameManger.accpetPlayerInput) ? e.ReadValue<Vector2>() : Vector2.zero;
        lookAction.performed += e => rawMousePosition = (GameManger.accpetPlayerInput) ? e.ReadValue<Vector2>() : Vector2.zero;
        zoomAction.performed += e => mouseScrollY = (GameManger.accpetPlayerInput) ? e.ReadValue<float>() : 0.0f;
        jumpAction.performed += e => Jump();
        sprintAction.performed += e => isSprinting = !isSprinting;
        sprintAction.canceled += e => isSprinting = !isSprinting;
        quickUseAction.performed += e => QickUse();

        playerInput.actions.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        playerUI.SetActive(true);
        inventoryUI.SetActive(false);
    }

    void Update()
    {
        ProcessPlayer();
        ProcessCamera();
        HandleAnimations();
    }
}
