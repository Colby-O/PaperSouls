using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Interactor : MonoBehaviour
{
    public Transform interactionPoint;
    public LayerMask interactionLayer;
    public float interactionRadius = 0.1f;
    public bool isInteracting;

    private PlayerInput playerInput;

    private InputAction interactAction;

    void StartInteraction(IInteractable interactable)
    {
        interactable.Interact(this, out isInteracting);
    }

    void EndInteraction()
    {
        isInteracting = false;
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        interactAction = playerInput.actions["Interact"];
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(interactionPoint.position, interactionRadius, interactionLayer);

        if (interactAction.WasPressedThisFrame())
        {
            if (colliders.Length == 0) EndInteraction();

            for (int i = 0; i < colliders.Length; i++)
            {
                IInteractable interactable = colliders[i].GetComponent<IInteractable>();
                if (interactable != null) StartInteraction(interactable);
            }
        }
    }
}
