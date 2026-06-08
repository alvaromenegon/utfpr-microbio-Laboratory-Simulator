using System;
using UnityEngine;

[Serializable]
public class ExperimentDefinition {
    public string id;
    public string title;
    public DateTime createdAt;
    public DateTime updatedAt;
    public ExperimentData experimentData;
    public int userId;

    public override string ToString() {
        return $"ExperimentDefinition: {title} (ID: {id}, CreatedAt: {createdAt}, UpdatedAt: {updatedAt})\n{experimentData}";
    }

    public bool IsValid() {
        if (string.IsNullOrEmpty(id)) {
            Debug.LogError("ExperimentDefinition ID is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(title)) {
            Debug.LogError("ExperimentDefinition title is null or empty.");
            return false;
        }
        if (experimentData == null) {
            Debug.LogError("ExperimentDefinition experimentData is null.");
            return false;
        }
        if (experimentData.Steps == null || experimentData.Steps.Count == 0) {
            Debug.LogError("ExperimentDefinition experimentData has no steps defined.");
            return false;
        }

        int stepCount = experimentData.Steps.Count;
        foreach (var step in experimentData.Steps) {
            if (!step.IsValid(stepCount)) {
                Debug.LogError($"ExperimentDefinition step {step} is invalid.");
                return false;
            }
        }

        return true;
    }
}
