using UnityEngine;

[DisallowMultipleComponent]
public class StepInteractable : MonoBehaviour, IInteractable
{

    public enum InteractionType {
        Interact,
        MoveTo,
    }
    [SerializeField] private string stepId;
    [SerializeField] private bool disableAfterInteraction = true;
    [SerializeField] private bool applyInteractableLayer = true;
    [SerializeField] private InteractionType stepType = InteractionType.Interact;

    public string StepId => stepId;

    public void Interact(){
        
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
