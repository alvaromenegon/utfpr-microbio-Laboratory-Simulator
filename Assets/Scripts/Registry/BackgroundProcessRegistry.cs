using System;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundProcessRegistry {
    private readonly Dictionary<string, BackgroundProcess> processesById = new(StringComparer.Ordinal);


    public void RegisterProcess(string identifier, BackgroundProcess process) {
        if (string.IsNullOrWhiteSpace(identifier)) {
            throw new ArgumentException("Identifier cannot be null or whitespace.", nameof(identifier));
        }
        if (process == null) {
            throw new ArgumentNullException(nameof(process), "BackgroundProcess cannot be null.");
        }
        if (processesById.ContainsKey(identifier)) {
            throw new InvalidOperationException($"A BackgroundProcess with identifier '{identifier}' is already registered.");
        }
        processesById.Add(identifier, process);
    }

    public void UnregisterProcess(string identifier) {
        if (string.IsNullOrWhiteSpace(identifier)) {
            throw new ArgumentException("Identifier cannot be null or whitespace.", nameof(identifier));
        }
        processesById.Remove(identifier);

    }

    public bool TryGetProcess(string identifier, out BackgroundProcess process) {
        process = null;
        if (string.IsNullOrWhiteSpace(identifier)) {
            return false;
        }
        return processesById.TryGetValue(identifier, out process);
    }

    public IEnumerable<BackgroundProcess> GetAllProcesses() {
        return processesById.Values;
    }

    public void SetupRegistry(ExperimentData experimentData) {
        if (experimentData == null || experimentData.BackgroundProcesses == null) {
            Debug.LogWarning("Experiment data is null or has no background processes defined.");
            return;
        }
        foreach (var processData in experimentData.BackgroundProcesses) {
            if (string.IsNullOrWhiteSpace(processData.Id)) {
                Debug.LogError("BackgroundProcessData has no valid identifier. Skipping.");
                continue;
            }
            if (processesById.ContainsKey(processData.Id)) {
                Debug.LogError($"Duplicate BackgroundProcess identifier '{processData.Id}' in experiment data. Skipping.");
                continue;
            }
            processesById.Add(processData.Id, processData);
        }
    }

}