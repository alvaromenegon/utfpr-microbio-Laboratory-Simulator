 using UnityEngine;
using static Interaction;

[DisallowMultipleComponent]
public class StepInteractable : MonoBehaviour, IInteractable
{

    [SerializeField] private string stepId;
    [SerializeField] private bool applyInteractableLayer = true;
    public InteractionType stepType = InteractionType.USE;

    public string StepId { get; set; }

    public void Interact(){
        if (RequireValue(stepType)) {
            // tipo de interação que requer um valor obtido
            // por entrada do usuário, como uma temperatura ou tempo.
            InputPanelController.Instance.Show();
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

    public void OnTriggerEnter(Collider other) { // todo: validar quando o jogador soltar o objeto (MoveObjects.Drop) e não quando ele entrar em contato com o collider
        if (stepType == InteractionType.MOVE && other.gameObject.CompareTag("SecondaryTarget"))
        {
            PlayerAction action = new() {
                InteractionType = stepType,
                Target = gameObject
            };
            ExperimentManager.Instance.ProcessInteraction(action);
        }
    }
   
}
