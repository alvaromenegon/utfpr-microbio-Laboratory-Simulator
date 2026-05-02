
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BackgroundProcess {
    public string Id { get; set; }
    public string Description { get; set; }

    public List<When> StartConditions;
    public List<When> CancelConditions;
    public List<When> SuccessConditions;

    public string OnSuccessEffectId;
    public string OnFailureEffectId;
    public int Duration { get; set; }
    public int? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public int Score { get; set; }

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

}