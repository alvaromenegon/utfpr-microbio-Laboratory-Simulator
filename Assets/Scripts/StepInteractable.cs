using Assets.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using static Interaction;

[DisallowMultipleComponent]
public class StepInteractable : LaboratoryObject, IInteractable {

    [SerializeField, FormerlySerializedAs("stepId")] private string interactionIdentifier;
    [SerializeField] private bool applyInteractableLayer = true;
    public InteractionType stepType = InteractionType.USE;

    public string StepId { get; set; }

    public override string GetIdentifier() {
        if (!string.IsNullOrWhiteSpace(interactionIdentifier)) {
            return interactionIdentifier;
        }

        return base.GetIdentifier();
    }

    public virtual void Interact() {
        if (RequireValue(stepType)) {
            InputPanelController.Instance.Show(new PlayerAction {
                InteractionType = stepType,
                Target = gameObject
            });        
            return;

        }
        PlayerAction action = new() {
            InteractionType = stepType,
            Target = gameObject
        };
        ExperimentManager.Instance.HandlePlayerAction(action);
    }

    private void OnValidate() {
        if (!applyInteractableLayer) {
            return;
        }

        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer >= 0) {
            gameObject.layer = interactableLayer;
        }
    }

    public virtual void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<MoveObjects>(out MoveObjects moveObjects)) {
            if (moveObjects.IsMoving) {
                return;
            }
        }
        if (stepType == InteractionType.MOVE && other.gameObject.CompareTag("SecondaryTarget")) {
            PlayerAction action = new() {
                InteractionType = stepType,
                Target = gameObject
            };
            Debug.Log($"Player moved {gameObject.name} to {other.gameObject.name}");
            ExperimentManager.Instance.OnSecondaryTargetInteraction(gameObject);
            ExperimentManager.Instance.ProcessInteraction(action);
        }
    }

    public override void OnHover() {
        HUDController.Instance.UpdateObjectName(GetObjectName());
    }

    public override void OnExitHover() {
        HUDController.Instance.UpdateObjectName("");
    }

}
