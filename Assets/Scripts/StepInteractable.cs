using UnityEngine;

[DisallowMultipleComponent]
public class StepInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string stepId;
    [SerializeField] private bool disableAfterInteraction = true;
    [SerializeField] private bool applyInteractableLayer = true;

    public string StepId => stepId;

    public void Interact()
    {
        // Hook para feedback visual/sonoro local do item, se necessário.
    }

    public void MarkCompleted()
    {
        if (disableAfterInteraction)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        if (!applyInteractableLayer)
        {
            return;
        }

        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer >= 0)
        {
            gameObject.layer = interactableLayer;
        }
    }
}
