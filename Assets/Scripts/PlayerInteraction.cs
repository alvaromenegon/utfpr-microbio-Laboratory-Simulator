using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;

    [Header("Interaction Settings")]
    public float interactionRange = 1.3f;
    public LayerMask interactableLayer;

    public event Action<IInteractable> InteractionPerformed;
    public event Action<IInteractable> HoverEntered;
    public event Action<IInteractable> HoverExited;

    private IInteractable _currentHoveredInteractable;

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

    public void TryInteract()
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

}
