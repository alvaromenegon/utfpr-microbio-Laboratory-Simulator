using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;

    [Header("Interaction Settings")]
    public float interactionRange = 3.0f;
    public LayerMask interactableLayer;

    public event Action<IInteractable> InteractionPerformed;

    private GameControls _controls;

    private void Awake()
    {
        _controls = new GameControls();
        _controls.Player.Interact.performed += _ => TryInteract();
    }

    private void OnEnable() => _controls.Player.Enable();
    private void OnDisable() => _controls.Player.Disable();

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerInteraction: playerCamera não atribuída.", this);
            return;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            return;
        }

        IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable == null)
        {
            return;
        }

        interactable.Interact();
        InteractionPerformed?.Invoke(interactable);
    }

    private void OnDrawGizmos()
    {
        if (playerCamera == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(playerCamera.position, playerCamera.forward * interactionRange);
    }
}
