using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExperimentObjectRegistry : MonoBehaviour {
    private readonly Dictionary<string, LaboratoryObject> interactablesById = new(StringComparer.Ordinal);
    public static ExperimentObjectRegistry Instance { get; private set; }

    private void Start() {
        RebuildIndex();
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    public void RebuildIndex() {
        interactablesById.Clear();
        LaboratoryObject[] interactables = FindObjectsByType<LaboratoryObject>(FindObjectsSortMode.None);

        foreach (var interactable in interactables) {
            string identifier = interactable.GetIdentifier();
            if (string.IsNullOrWhiteSpace(identifier)) {
                Debug.LogError($"LaboratoryObject '{interactable.gameObject.name}' has no canonical identifier configured.");
                continue;
            }

            if (interactablesById.TryGetValue(identifier, out var existing)) {
                Debug.LogError($"Duplicate LaboratoryObject identifier '{identifier}'. Objects: '{existing.gameObject.name}' and '{interactable.gameObject.name}'.");
                continue;
            }

            interactablesById.Add(identifier, interactable);
        }
    }

    private bool IsNull(string identifier) {
        return string.IsNullOrWhiteSpace(identifier) || !interactablesById.ContainsKey(identifier);
    }

    public bool TryGetInteractable(string identifier, out LaboratoryObject interactable) {
        interactable = null;
        if (IsNull(identifier)) {
            return false;
        }

        return interactablesById.TryGetValue(identifier, out interactable);
    }

    public bool ValidateStepTargets(ExperimentDefinition experimentDefinition, out string validationReport) {
        var report = new StringBuilder();

        if (experimentDefinition == null || experimentDefinition.experimentData == null || experimentDefinition.experimentData.Steps == null) {
            report.AppendLine("Experiment definition is null or malformed.");
            validationReport = report.ToString();
            return false;
        }

        for (int i = 0; i < experimentDefinition.experimentData.Steps.Count; i++) {
            var step = experimentDefinition.experimentData.Steps[i];
            ValidateTarget(step.InteractionTarget, i, "InteractionTarget", report);

            if (!string.IsNullOrWhiteSpace(step.InteractionTargetSecondary)) {
                ValidateTarget(step.InteractionTargetSecondary, i, "InteractionTargetSecondary", report);
            }
        }

        validationReport = report.ToString();
        return report.Length == 0;
    }

    private void ValidateTarget(string targetId, int stepIndex, string fieldName, StringBuilder report) {
        if (string.IsNullOrWhiteSpace(targetId)) {
            report.AppendLine($"Step {stepIndex + 1}: {fieldName} is empty.");
            return;
        }

        if (!interactablesById.ContainsKey(targetId)) {
            report.AppendLine($"Step {stepIndex + 1}: {fieldName} '{targetId}' was not found in ExperimentObjectRegistry.");
        }
    }
}
