using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExperimentRunner : MonoBehaviour
{
    [System.Serializable]
    public class StepStateChangedEvent : UnityEvent<string, int> { }

    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private List<string> orderedSteps = new List<string>();

    public StepStateChangedEvent onStepCompleted;
    public UnityEvent onExperimentCompleted;

    public int CurrentStepIndex { get; private set; }

    private void OnEnable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.InteractionPerformed += HandleInteraction;
        }
    }

    private void OnDisable()
    {
        if (playerInteraction != null)
        {
            playerInteraction.InteractionPerformed -= HandleInteraction;
        }
    }

    public void HandleInteraction(IInteractable interactable)
    {
        StepInteractable stepInteractable = interactable as StepInteractable;
        if (stepInteractable == null)
        {
            return;
        }

        if (CurrentStepIndex >= orderedSteps.Count)
        {
            return;
        }

        string expectedStep = orderedSteps[CurrentStepIndex];
        if (!string.Equals(stepInteractable.StepId, expectedStep))
        {
            return;
        }
        stepInteractable.MarkCompleted();

        onStepCompleted?.Invoke(stepInteractable.StepId, CurrentStepIndex);

        CurrentStepIndex++;
        if (CurrentStepIndex >= orderedSteps.Count)
        {
            onExperimentCompleted?.Invoke();
        }
    }
}
