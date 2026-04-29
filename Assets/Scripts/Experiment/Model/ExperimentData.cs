using Assets.Scripts;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Interaction;

[Serializable]
public class ExperimentData {
    public Step[] Steps { get; set; }
    public BackgroundProcess[] BackgroundProcesses { get; set; }
    public string[] Objects { get; set; }
    public string Description { get; set; }
    public int Time { get; set; }

    public bool HasTimeLimit() {
        return Time > 0;
    }

    public override string ToString() {
        string result = $"Experiment: {Description}\nTime Limit: {(HasTimeLimit() ? Time.ToString() + " seconds" : "None")}\nObjects: {string.Join(", ", Objects)}\nSteps:\n";
        for (int i = 0; i < Steps.Length; i++) {
            var step = Steps[i];
            result += $"  Step {i + 1}: {step.Description}, Interaction: {step.Interaction}, Target: {step.InteractionTarget}, Instruction: {step.Instruction}, Value: {step.Value}, OnSuccess: {step.OnSuccessMessage}, OnFailure: {step.OnFailureMessage}, ActionOnSuccess: {step.ActionOnSuccess}, ActionOnFailure: {step.ActionOnFailure}, OnFailureReturnToStep: {step.OnFailureReturnToStep}\n";
        }
        return result;
    }
}

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
            Debug.LogWarning($"Invalid interaction type: {interaction}. Defaulting to NONE.");
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

[Serializable]
public class BackgroundProcess {
    public string Id { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public int? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public int Score { get; set; }
    public int?[] ShouldCompleteBetweenSteps { get; set; }

    public bool IsRunning { get; private set; } = false;

    public void Start() {
        if (IsRunning) {
            Debug.LogWarning($"BackgroundProcess {Id} is already running.");
            return;
        }
        IsRunning = true;
        Debug.Log($"Started BackgroundProcess: {Description}");
    }

    public void Stop() {
        if (!IsRunning) {
            Debug.LogWarning($"BackgroundProcess {Id} is not running.");
            return;
        }
        IsRunning = false;
        Debug.Log($"Stopped BackgroundProcess: {Description}");
    }

    public bool ValidateCompletion(int currentStepIndex) {
        if (IsRunning) {
            Debug.LogWarning($"BackgroundProcess {Id} is still running. Cannot validate completion.");
            return false;
        }

        if (ShouldCompleteBetweenSteps == null || ShouldCompleteBetweenSteps.Length == 0) {
            return true;
        }
        if (ShouldCompleteBetweenSteps[0] == null && ShouldCompleteBetweenSteps[1] != null) {
            return currentStepIndex <= ShouldCompleteBetweenSteps[1];
        }
        if (ShouldCompleteBetweenSteps[0] != null && ShouldCompleteBetweenSteps[1] == null) {
            return currentStepIndex >= ShouldCompleteBetweenSteps[0];
        }
        if (ShouldCompleteBetweenSteps[0] != null && ShouldCompleteBetweenSteps[1] != null) {
            return currentStepIndex >= ShouldCompleteBetweenSteps[0] && currentStepIndex <= ShouldCompleteBetweenSteps[1];
        }
        return true;
    }
}

public enum OnStepFinished {
    DEFAULT,
    COMPLETE_EXPERIMENT_SUCCESS,
    COMPLETE_EXPERIMENT_FAILURE,
    COMPLETE_EXPERIMENT_STEP,
    BACK_TO_STEP,
    BACK_TO_START,
    START_BACKGROUND_WORK,
}
