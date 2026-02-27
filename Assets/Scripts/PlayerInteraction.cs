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
    public event Action<IInteractable> HoverEntered;
    public event Action<IInteractable> HoverExited;

    private GameControls _controls;
    private IInteractable _currentHoveredInteractable;

    private void Awake()
    {
        _controls = new GameControls();
        _controls.Player.Interact.performed += _ => TryInteract();
    }

    private void OnEnable() => _controls.Player.Enable();
    private void OnDisable()
    {
        _controls.Player.Disable();
        ClearHoverState();
    }

    private void Update()
    {
        UpdateHoverState();
    }

    public bool TryGetInteractableInSight(out IInteractable interactable)
    {
        interactable = null;

        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            return false;
        }

        interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            return true;
        }

        StepInteractable stepInteractable = hit.collider.GetComponentInParent<StepInteractable>();
        if (stepInteractable == null)
        {
            return false;
        }

        interactable = stepInteractable;
        return true;
    }

    private void TryInteract()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerInteraction: playerCamera não atribuída.", this);
            return;
        }

        if (!TryGetInteractableInSight(out IInteractable interactable))
        {
            return;
        }

        interactable.Interact();
        InteractionPerformed?.Invoke(interactable);
    }

    private void UpdateHoverState()
    {
        IInteractable nextHover = null;
        if (TryGetInteractableInSight(out IInteractable interactable))
        {
            nextHover = interactable;
        }

        if (ReferenceEquals(_currentHoveredInteractable, nextHover))
        {
            return;
        }

        if (_currentHoveredInteractable != null)
        {
            HoverExited?.Invoke(_currentHoveredInteractable);
        }

        _currentHoveredInteractable = nextHover;

        if (_currentHoveredInteractable != null)
        {
            HoverEntered?.Invoke(_currentHoveredInteractable);
        }
    }

    private void ClearHoverState()
    {
        if (_currentHoveredInteractable == null)
        {
            return;
        }

        HoverExited?.Invoke(_currentHoveredInteractable);
        _currentHoveredInteractable = null;
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
