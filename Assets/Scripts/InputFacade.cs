using Assets.Scripts;
using System;
using UnityEngine;

public class InputFacade : MonoBehaviour {
    [Header("References")]
    public Transform playerCamera;
    public CursorChanger cursorChanger;

    [Header("Interaction Settings")]
    public float interactionRange = 1.3f;
    public LayerMask interactableLayer;

    private GameControls _controls;
    private IMovable _currentMovable;
    private IInteractable _currentHoveredInteractable;
    private LaboratoryObject _currentLaboratoryObject;

    public event Action<IInteractable> InteractionPerformed;
    public event Action<IInteractable> HoverEntered;
    public event Action<IInteractable> HoverExited;

    void Awake()
    {
        _controls = new GameControls();
        _controls.Player.Click.performed += _ => HandleClick();
        _controls.Player.Click.canceled += _ => HandleClickRelease();
        _controls.Player.Interact.performed += _ => HandleInteract();
    }

    private void HandleInteract()
    {
        if (_currentHoveredInteractable != null)
        {
            _currentHoveredInteractable.Interact();
            InteractionPerformed?.Invoke(_currentHoveredInteractable);
        }
    }

    void OnEnable() => _controls.Player.Enable();
    void OnDisable() => _controls.Player.Disable();

    void Update()
    {
        UpdateHoverState();
    }

    public Vector2 GetMoveInput() => _controls.Player.Move.ReadValue<Vector2>();
    public Vector2 GetLookInput() => _controls.Player.Look.ReadValue<Vector2>();

    private void HandleClick()
    {
        if (_currentMovable != null && !_currentMovable.IsMoving)
        {
            _currentMovable.PickUp();
        }
    }

    private void HandleClickRelease()
    {
        if (_currentMovable != null && _currentMovable.IsMoving){
            _currentMovable.Drop();
            
        }
    }



    // Legacy compatibility API migrated from PlayerInteraction.
    public bool TryGetInteractableInSight(out IInteractable interactable)
    {
        interactable = null;
        if (!TryGetObjectInSight(out RaycastHit hit))
        {
            return false;
        }

        interactable = hit.collider.GetComponentInParent<IInteractable>();
        return interactable != null;
    }

    // Legacy compatibility API migrated from PlayerInteraction.
    public void TryInteract() => HandleInteract();
    public bool TryGetObjectInSight(out RaycastHit hit)
    {
        hit = default;

        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = new(playerCamera.position, playerCamera.forward);
        return Physics.Raycast(ray, out hit, interactionRange, interactableLayer);
    }

    private void UpdateHoverState()
    {
        IMovable nextMovable = null;
        IInteractable nextInteractable = null;
        LaboratoryObject nextLaboratoryObject = null;
        CursorChanger.CursorType cursorType = CursorChanger.CursorType.Pointer;

        if (TryGetObjectInSight(out RaycastHit hit))
        {
            nextMovable = hit.collider.GetComponentInParent<IMovable>();
            nextInteractable = hit.collider.GetComponentInParent<IInteractable>();
            nextLaboratoryObject = hit.collider.GetComponentInParent<LaboratoryObject>();         

            if (nextMovable != null) {
                cursorType = CursorChanger.CursorType.Selector;
            }
            else if (nextInteractable != null) {
                cursorType = CursorChanger.CursorType.Interact;
            }
        }

        if (!ReferenceEquals(_currentLaboratoryObject, nextLaboratoryObject))
        {
            if (_currentLaboratoryObject != null)
            {
                _currentLaboratoryObject.OnExitHover();
            }
            _currentLaboratoryObject = nextLaboratoryObject;
            if (_currentLaboratoryObject != null)
            {
                _currentLaboratoryObject.OnHover();
            }
        } else {
            _currentLaboratoryObject = nextLaboratoryObject;
        }

        if (!ReferenceEquals(_currentMovable, nextMovable)) {
            _currentMovable = nextMovable;
        }

        if (!ReferenceEquals(_currentHoveredInteractable, nextInteractable))
        {
            if (_currentHoveredInteractable != null)
            {
                HoverExited?.Invoke(_currentHoveredInteractable);
            }

            _currentHoveredInteractable = nextInteractable;

            if (_currentHoveredInteractable != null)
            {
                HoverEntered?.Invoke(_currentHoveredInteractable);
            }
        }

        if (cursorChanger != null)
        {
            cursorChanger.ChangeTo(cursorType);
        }
    }

}