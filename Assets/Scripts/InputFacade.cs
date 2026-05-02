using Assets.Scripts;
using System;
using UnityEngine;
using static Interaction;

class InputFacade : MonoBehaviour {
    [Header("References")]
    public Transform playerCamera;
    public CursorChanger cursorChanger;

    [Header("Interaction Settings")]
    public float interactionRange = 1.3f;
    public LayerMask interactableLayer;

    private GameControls _controls;
    private IMovable _currentMovable;
    private LaboratoryObject _currentHoveredInteractable;

    private InteractionSystem interactionSystem;
    private InteractionRequest _activeRequest;

    public event Action<LaboratoryObject> HoverEntered;
    public event Action<LaboratoryObject> HoverExited;

    void Awake()
    {
        _controls = new GameControls();
        _controls.Player.Click.performed += _ => HandleClick();
        _controls.Player.Click.canceled += _ => HandleClickRelease();
        _controls.Player.Interact.performed += _ => HandleInteractStart();
        _controls.Player.Interact.canceled += _ => HandleInteractEnd();

    }

    private void HandleInteractStart(){
        if (_currentHoveredInteractable == null) return;

        _activeRequest = new InteractionResolver(_currentHoveredInteractable, _currentMovable).BuildRequest();

        interactionSystem.HandleInteractionRequestStart(_activeRequest);
    }

    private void HandleInteractEnd(){
        if (_activeRequest == null) return;

        interactionSystem.HandleInteractionRequestEnd(_activeRequest);
        _activeRequest = null;
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
        LaboratoryObject nextInteractable = null;
        CursorChanger.CursorType cursorType = CursorChanger.CursorType.Pointer;

        if (TryGetObjectInSight(out RaycastHit hit))
        {
            nextMovable = hit.collider.GetComponentInParent<IMovable>();
            nextInteractable = hit.collider.GetComponentInParent<LaboratoryObject>();

            if (nextMovable != null) {
                cursorType = CursorChanger.CursorType.Selector;
            }
            else if (nextInteractable != null) {
                cursorType = CursorChanger.CursorType.Interact;
            }
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