
using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Interaction;

[Serializable]
public class Step {
    public string Description { get; set; }
    public int Score { get; set; }
    public string interaction;
    public InteractionType Interaction {
        get {
            if (Enum.TryParse(interaction, out InteractionType result)) {
                return result;
            }
            return InteractionType.USE;
        }
    }
    public string InteractionTarget { get; set; }
    public string InteractionTargetSecondary { get; set; }
    public string Instruction { get; set; }
    public double? Value { get; set; }
    public double? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public OnStepFinished ActionOnSuccess { get; set; }
    public string OnSuccessStartBackgroundWorkId { get; set; }

    public List<StepGoal> Goals { get; set; }

    public bool IsValid(int numSteps) {
        if (string.IsNullOrEmpty(Description)) {
            Debug.LogError("Step description is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(interaction)) {
            Debug.LogError("Step interaction is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(InteractionTarget)) {
            Debug.LogError("Step interaction target is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(Instruction)) {
            Debug.LogError("Step instruction is null or empty.");
            return false;
        }
        if (ActionOnFailure == OnStepFinished.BACK_TO_STEP && (OnFailureReturnToStep < 0 || OnFailureReturnToStep >= numSteps)) {
            Debug.LogError("Step action on failure is BACK_TO_STEP but OnFailureReturnToStep is not set to a valid step index.");
            return false;
        }
        return true;
    }
}