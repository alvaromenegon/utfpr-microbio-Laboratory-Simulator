using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("References")]
	public Transform playerCamera;
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 0.5f;
    public float maxLookAngle = 60.0f;

    private GameControls _controls;
    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _cameraPitch = 0f;

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _controls = new GameControls();

        // Movimentação WASD
        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        // Mouse Look
        _controls.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += ctx => _lookInput = Vector2.zero;

        // Travar cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable() => _controls.Player.Enable();
    void OnDisable() => _controls.Player.Disable();

    void HandleMovement() {
        _moveInput = _controls.Player.Move.ReadValue<Vector2>();

        // Movimento relativo à direção da câmera
        Vector3 forward = transform.forward * _moveInput.y;
        Vector3 right = transform.right * _moveInput.x;
        Vector3 movement = (forward + right).normalized * moveSpeed;

        _characterController.SimpleMove(movement);
    }

    void HandleMouseLook() {
        _lookInput = _controls.Player.Look.ReadValue<Vector2>();

        // Rotação horizontal (Yaw) - Rotaciona o corpo do player
        float yaw = _lookInput.x * mouseSensitivity;

        transform.Rotate(0f, yaw, 0f);
        Debug.Log("look input x: " + _lookInput.x);
        Debug.Log("Look input y: " + _lookInput.y);
        // Rotação vertical (Pitch) - Rotaciona apenas a câmera
        _cameraPitch -= _lookInput.y * mouseSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    // Update is called once per frame
    void Update ()
    {
        HandleMovement();
        HandleMouseLook();
    }

   
}
