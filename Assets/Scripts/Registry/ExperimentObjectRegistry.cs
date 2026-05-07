using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExperimentObjectRegistry : MonoBehaviour {
    private readonly Dictionary<string, LaboratoryObject> objectsById = new(StringComparer.Ordinal);
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
        objectsById.Clear();
        LaboratoryObject[] interactables = FindObjectsByType<LaboratoryObject>(FindObjectsSortMode.None);

        foreach (var interactable in interactables) {
            string identifier = interactable.Identifier;
            if (string.IsNullOrWhiteSpace(identifier)) {
                Debug.LogError($"LaboratoryObject '{interactable.gameObject.name}' has no canonical identifier configured.");
                continue;
            }

            if (objectsById.TryGetValue(identifier, out var existing)) {
                Debug.LogError($"Duplicate LaboratoryObject identifier '{identifier}'. Objects: '{existing.gameObject.name}' and '{interactable.gameObject.name}'.");
                continue;
            }

            objectsById.Add(identifier, interactable);
        }
    }

    private bool IsNull(string identifier) {
        return string.IsNullOrWhiteSpace(identifier) || !objectsById.ContainsKey(identifier);
    }

    [Obsolete("Use TryGetObject instead.")]
    public bool TryGetInteractable(string identifier, out LaboratoryObject interactable) {
        interactable = null;
        if (IsNull(identifier)) {
            return false;
        }

        return objectsById.TryGetValue(identifier, out interactable);
    }

    public bool TryGetObject(string identifier, out LaboratoryObject interactable) {
        interactable = null;
        if (IsNull(identifier)) {
            return false;
        }
        return objectsById.TryGetValue(identifier, out interactable);
    }

    public LaboratoryObject ResolveTargetFromContext(InteractionContext context) {
        if (context == null) {
            Debug.LogError("InteractionContext is null.");
            return null;
        }
        var tags = context.Tags;
        if (tags == null || tags.Count == 0) {
            Debug.LogError("InteractionContext has no tags defined.");
            return null;
        }
        foreach (var labObj in objectsById.Values) {
            if (labObj.Tags != null && labObj.Tags.Count > 0) {
                foreach (var tag in tags) {
                    if (labObj.HasTag(tag)) {
                        return labObj;
                    }
                }
            }
        }

        return null;    
    }


}
