using Assets.Scripts;
using UnityEngine;
using static Interaction;

[DisallowMultipleComponent]
public class StepInteractable : LaboratoryObject, IInteractable{

    [SerializeField] private string stepId;
    [SerializeField] private bool applyInteractableLayer = true;
    public InteractionType stepType = InteractionType.USE;

    public string StepId { get; set; }

    public virtual void Interact(){
        if (RequireValue(stepType)) {
            InputPanelController.Instance.Show(new PlayerAction {
                InteractionType = stepType,
                Target = gameObject
            });
        ExperimentManager.Instance.HandlePlayerAction(action);
            ExperimentManager.Instance.HandlePlayerAction(action);
            return;

        }
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

    public virtual void OnTriggerEnter(Collider other) { // todo: validar quando o jogador soltar o objeto (MoveObjects.Drop) e não quando ele entrar em contato com o collider
        if (other.TryGetComponent<MoveObjects>(out MoveObjects moveObjects)) {
            if (moveObjects.IsMoving) {
                return; // o jogador está movendo um objeto, não processa a interação.
            }
        }
        if (stepType == InteractionType.MOVE && other.gameObject.CompareTag("SecondaryTarget")) {
            PlayerAction action = new() {
                InteractionType = stepType,
                Target = gameObject
            };
            Debug.Log($"Player moved {gameObject.name} to {other.gameObject.name}");
            ExperimentManager.Instance.OnSecondaryTargetInteraction(gameObject); // o manager decide se alguma outra ação vai ocorrer antes de processar a interação.
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
