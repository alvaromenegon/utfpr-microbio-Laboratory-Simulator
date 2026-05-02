
using System;
using UnityEngine;

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