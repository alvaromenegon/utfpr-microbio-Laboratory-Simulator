using Assets.Scripts;
using System;
using UnityEngine;

class InputFacade : MonoBehaviour {
    [Header("References")]
    public Transform playerCamera;
    public CursorChanger cursorChanger;

    [Header("Interaction Settings")]
    public float interactionRange = 1.3f;
    public LayerMask interactionLayer;
    public LayerMask movableLayer;

    private GameControls _controls;
    private IMovable _currentMovable;
    private LaboratoryObject _currentHoveredInteractable;

    private InteractionSystem interactionSystem;
    private InteractionRequest _activeRequest;

    public event Action<LaboratoryObject> HoverEntered;
    public event Action<LaboratoryObject> HoverExited;

    void Start() {
        interactionSystem = ExperimentManager.Instance.InteractionSystem;
    }

    void Awake()
    {
        _controls = new GameControls();
        _controls.Player.Click.performed += _ => HandleClick();
        _controls.Player.Click.canceled += _ => HandleClickRelease();
        _controls.Player.Interact.performed += _ => HandleInteractStart();
        _controls.Player.Interact.canceled += _ => HandleInteractEnd();

    }

    private void HandleInteractStart(){
        Debug.Log("Interact started.");
        if (_currentMovable == null && _currentHoveredInteractable == null) {
            Debug.Log("No interactable in sight.");
            return;
        }

        _activeRequest = new InteractionResolver(_currentHoveredInteractable, _currentMovable).BuildRequest();

        interactionSystem.HandleInteractionRequestStart(_activeRequest);
    }

    private void HandleInteractEnd(){
        Debug.Log("Interact ended.");
        if (_activeRequest == null) return;

        interactionSystem.HandleInteractionRequestEnd(_activeRequest);
        _activeRequest = null;
    }

    void OnEnable() => _controls.Player.Enable();
    void OnDisable() => _controls.Player.Disable();

    void Update()
    {
        UpdateHoverState();
        UpdateCursor();
    }

    public Vector2 GetMoveInput() => _controls.Player.Move.ReadValue<Vector2>();
    public Vector2 GetLookInput() => _controls.Player.Look.ReadValue<Vector2>();

    private void UpdateCursor() {
        var cursor = CursorChanger.CursorType.Pointer;
        if (_currentMovable != null) {
            cursor = CursorChanger.CursorType.Selector;
        } else if (_currentHoveredInteractable != null) {
            cursor = CursorChanger.CursorType.Interact;
        }
        cursorChanger.ChangeTo(cursor);
    }

    private void HandleClick()
    {
        if (_currentMovable == null) {
            if (TryGetMovable(out var movable)) {
                _currentMovable = movable;
                movable.PickUp();
            }           
        } 
    }

    private void HandleClickRelease()
    {
        if (_currentMovable != null && _currentMovable.IsMoving){
            _currentMovable.Drop();
            _currentMovable = null;
        }
    }

    private bool TryGetMovable(out IMovable movable) {       
        movable = null;

        Ray ray = new(playerCamera.position, playerCamera.forward);

        if (Physics.Raycast(ray, out var hit, interactionRange, movableLayer)) {
            movable = hit.collider.GetComponentInParent<IMovable>();
            return movable != null;
        }
        return false;
    }

    private bool TryGetInteractionTarget(out RaycastHit hit) {
        hit = default;

        Ray ray = new(playerCamera.position, playerCamera.forward);

        return Physics.Raycast(ray, out hit, interactionRange, interactionLayer);
    }

    private void UpdateHoveredObject(LaboratoryObject nextObject) {         
        if (_currentHoveredInteractable != nextObject) {
            HoverEntered += (obj) => obj.OnHover();
            HoverExited += (obj) => obj.OnExitHover();
            if (_currentHoveredInteractable != null) {
                HoverExited?.Invoke(_currentHoveredInteractable);
            }
            _currentHoveredInteractable = nextObject;
            if (_currentHoveredInteractable != null) {
                HoverEntered?.Invoke(_currentHoveredInteractable);
            }
            HoverEntered = null;
            HoverExited = null;
        }
    }

    private void UpdateHoverState(){
        LaboratoryObject nextObject = null;
        if (playerCamera == null) {
            Debug.LogError("Player camera reference is missing.");
            return ;
        }

        if (TryGetInteractionTarget(out RaycastHit hit))
        {
            nextObject = hit.collider.GetComponentInParent<LaboratoryObject>();           
        }        
        UpdateHoveredObject(nextObject);
    }
}