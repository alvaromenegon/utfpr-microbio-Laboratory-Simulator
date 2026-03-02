using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WebReticleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private Image defaultReticleIcon;
    [SerializeField] private Image focusedReticleIcon;

    [Header("Cursor")]
    [SerializeField] private bool lockSystemCursorOnWeb = true;

    private void Awake()
    {
        if (playerInteraction == null)
        {
            playerInteraction = GetComponent<PlayerInteraction>();
        }
    }

    private void OnEnable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.HoverEntered += OnHoverEnter;
            playerInteraction.HoverExited += OnHoverExit;
        }

        SetFocusedState(false);
        ApplyCursorState();
    }

    private void OnDisable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.HoverEntered -= OnHoverEnter;
            playerInteraction.HoverExited -= OnHoverExit;
        }

        SetFocusedState(false);
    }

    private void Update()
    {
        ApplyCursorState();
    }

    private void OnHoverEnter(IInteractable interactable)
    {
        Debug.Log($"Hover entered on interactable: {interactable}");
        SetFocusedState(interactable != null);
    }

    private void OnHoverExit(IInteractable interactable)
    {
        SetFocusedState(false);
    }

    private void SetFocusedState(bool focused)
    {
        if (defaultReticleIcon != null)
        {
            Debug.Log($"Setting default reticle icon enabled: {!focused}");
            defaultReticleIcon.enabled = !focused;
        }

        if (focusedReticleIcon != null)
        {
            Debug.Log($"Setting focused reticle icon enabled: {focused}");
            focusedReticleIcon.enabled = focused;
        }
    }

    private void ApplyCursorState()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!lockSystemCursorOnWeb || !isActiveAndEnabled)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }
}
