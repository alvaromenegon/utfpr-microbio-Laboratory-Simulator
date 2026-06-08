
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Step {
    public string Description { get; set; }
    public int Score { get; set; }

    public string Instruction { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public OnStepFinished ActionOnSuccess { get; set; }

    public List<Condition> Goals { get; set; }

    public List<Condition> FailureConditions { get; set; }

    public bool IsValid(int numSteps) {
        if (string.IsNullOrEmpty(Description)) {
            Debug.LogError("Step description is null or empty.");
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