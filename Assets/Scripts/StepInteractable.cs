 using UnityEngine;

[DisallowMultipleComponent]
public class StepInteractable : MonoBehaviour, IInteractable
{

    [SerializeField] private string stepId;
    [SerializeField] private bool applyInteractableLayer = true;
    public InteractionType stepType = InteractionType.USE;

    public string StepId { get; set; }

    public void Interact(){
        PlayerAction action = new() {
            InteractionType = stepType,
            Target = gameObject
        };
        ExperimentManager.Instance.ProcessInteraction(action);
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
