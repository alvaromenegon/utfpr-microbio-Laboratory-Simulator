using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [Header("References")]
	public Transform playerCamera;
    [Header("Movement Settings")]
    public float moveSpeed = 3.0f;

    [Header("Camera Settings")]
    public float mouseSensitivity = 0.3f;
    public float maxLookAngle = 60.0f;

    private InputFacade inputFacade;
    private CharacterController _characterController;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _cameraPitch = 0f;

    void Awake()
    {
        inputFacade = GetComponent<InputFacade>();
        _characterController = GetComponent<CharacterController>();

        // Travar cursor no centro da tela
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void HandleMovement() {
        _moveInput = inputFacade.GetMoveInput();

        // Movimento relativo à direção da câmera
        Vector3 forward = transform.forward * _moveInput.y;
        Vector3 right = transform.right * _moveInput.x;
        Vector3 movement = (forward + right).normalized * moveSpeed;

        _characterController.SimpleMove(movement);
    }

    void HandleMouseLook() {
        _lookInput = inputFacade.GetLookInput();

        // Rotação horizontal (Yaw) - Rotaciona o corpo do player
        float yaw = _lookInput.x * mouseSensitivity;

        transform.Rotate(0f, yaw, 0f);

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
