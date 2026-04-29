using System;
using UnityEngine;

/// <summary>
/// LEGADO/DEPRECADO: fluxo de interação substituído por InputFacade.
/// Mantenha este componente apenas para compatibilidade temporária em cenas antigas.
/// </summary>
[Obsolete("PlayerInteraction is deprecated. Use InputFacade as the official interaction flow.", false)]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Legacy Bridge")]
    [SerializeField] private InputFacade inputFacade;

    public event Action<IInteractable> InteractionPerformed
    {
        add { if (inputFacade != null) inputFacade.InteractionPerformed += value; }
        remove { if (inputFacade != null) inputFacade.InteractionPerformed -= value; }
    }

    public event Action<IInteractable> HoverEntered
    {
        add { if (inputFacade != null) inputFacade.HoverEntered += value; }
        remove { if (inputFacade != null) inputFacade.HoverEntered -= value; }
    }

    public event Action<IInteractable> HoverExited
    {
        add { if (inputFacade != null) inputFacade.HoverExited += value; }
        remove { if (inputFacade != null) inputFacade.HoverExited -= value; }
    }

    private void Awake()
    {
        if (inputFacade == null)
        {
            inputFacade = GetComponent<InputFacade>();
        }
    }

    public bool TryGetInteractableInSight(out IInteractable interactable)
    {
        interactable = null;
        return inputFacade != null && inputFacade.TryGetInteractableInSight(out interactable);
    }

    public void TryInteract()
    {
        if (inputFacade == null)
        {
            Debug.LogWarning("PlayerInteraction (legado): InputFacade não atribuído.", this);
            return;
        }

        inputFacade.TryInteract();
    }
}
