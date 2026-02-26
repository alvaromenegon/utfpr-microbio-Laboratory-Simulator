using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera;

    [Header("Interaction Settings")]
    public float interactionRange = 3.0f;
    public LayerMask interactableLayer;

    private GameControls _controls;

    void Awake()
    {
        _controls = new GameControls();
        _controls.Player.Interact.performed += ctx => TryInteract();
    }

    void OnEnable() => _controls.Player.Enable();
    void OnDisable() => _controls.Player.Disable();

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayer))
        {
            // Procurar componente de interação no objeto
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact();
        }
    }

    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.position, playerCamera.forward * interactionRange);
        }
    }
}

// Interface para objetos interativos
public interface IInteractable
{
    void Interact();
}