using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExperimentObjectRegistry : MonoBehaviour {
    private readonly Dictionary<string, LaboratoryObject> objectsById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<LaboratoryObject>> objectsByIdentifier = new(StringComparer.Ordinal);
    private readonly Dictionary<string, LaboratoryObject> objectsByInstanceId = new(StringComparer.Ordinal);
    public event Action WillRebuildIndex;
    public event Action OnRegistryUpdated;

    private static ExperimentObjectRegistry _instance;

    public static ExperimentObjectRegistry GetInstance() {
        if (_instance == null) {
            _instance = FindFirstObjectByType<ExperimentObjectRegistry>();
            if (_instance == null) {
                Debug.LogError("No instance of ExperimentObjectRegistry found in the scene. Please add one to the scene.");
            }
        }
        return _instance;
    }


    private void Awake() {
        if (_instance != null && _instance != this) {
            Debug.LogError("Multiple instances of ExperimentObjectRegistry detected. Destroying duplicate.");
            Destroy(this);
            return;
        }
        _instance = this;
    }

    public List<LaboratoryObject> GetAllObjects() {
        var allObjects = new List<LaboratoryObject>();
        foreach (var kvp in objectsByIdentifier) {
            allObjects.AddRange(kvp.Value);
        }
        return allObjects;
    }

    public void Register(LaboratoryObject obj) {
        // Por InstanceId — chave única
        if (!string.IsNullOrWhiteSpace(obj.InstanceId)) {
            if (objectsByInstanceId.ContainsKey(obj.InstanceId))
                Debug.LogError($"InstanceId duplicado: '{obj.InstanceId}'");
            else
                objectsByInstanceId[obj.InstanceId] = obj;
        }

        // Por CanonicalIdentifier — pode ter vários
        if (!objectsByIdentifier.ContainsKey(obj.Identifier))
            objectsByIdentifier[obj.Identifier] = new List<LaboratoryObject>();
        objectsByIdentifier[obj.Identifier].Add(obj);
    }

    public void Unregister(LaboratoryObject obj) {
        if (!string.IsNullOrWhiteSpace(obj.InstanceId))
            objectsByInstanceId.Remove(obj.InstanceId);

        if (objectsByIdentifier.TryGetValue(obj.Identifier, out var list))
            list.Remove(obj);
    }

    private bool IsNull(string identifier) {
        return string.IsNullOrWhiteSpace(identifier) || !objectsById.ContainsKey(identifier);
    }

    public bool TryGetObject(string identifier, out LaboratoryObject interactable) {
        interactable = null;
        if (IsNull(identifier)) {
            return false;
        }
        // verifica na lista de objetos por identificador, se houver mais de um, dá erro para evitar efeitos inesperados
        if (objectsByIdentifier.TryGetValue(identifier, out var list) && list.Count > 1) {
            Debug.LogError($"Multiple LaboratoryObject instances with identifier '{identifier}'. Objects: {string.Join(", ", list.ConvertAll(obj => $"'{obj.gameObject.name}'"))}. Consider using InstanceIds for unique identification.");
            throw new InvalidOperationException($"Multiple LaboratoryObject instances with identifier '{identifier}'. Objects: {string.Join(", ", list.ConvertAll(obj => $"'{obj.gameObject.name}'"))}. Consider using InstanceIds for unique identification.");
        }
        return objectsById.TryGetValue(identifier, out interactable);
    }

    public bool TryGetObjectInstance(string instanceId, out LaboratoryObject interactable) {
        interactable = null;
        if (string.IsNullOrWhiteSpace(instanceId)) {
            return false;
        }
        return objectsByInstanceId.TryGetValue(instanceId, out interactable);
    }

    public void RegisterInstance(string instanceId, LaboratoryObject interactable) {
        if (string.IsNullOrWhiteSpace(instanceId)) {
            Debug.LogWarning($"Cannot register object '{interactable.gameObject.name}' with empty instanceId.");
            return;
        }
        objectsByInstanceId[instanceId] = interactable;
    }

    public List<LaboratoryObject> Query(string canonicalIdentifier = null, List<string> requiredTags = null, List<string> requiredZoneTags = null) {
        var result = new List<LaboratoryObject>();
        if (!string.IsNullOrWhiteSpace(canonicalIdentifier)) {
            if (objectsByIdentifier.TryGetValue(canonicalIdentifier, out var candidates)) {
                result.AddRange(candidates);
            } else {
                Debug.LogWarning($"No LaboratoryObject found with canonicalIdentifier '{canonicalIdentifier}'.");
            }
        } else {            
            // If no identifier is provided, consider all objects as candidates
            foreach (var kvp in objectsByIdentifier) {
                result.AddRange(kvp.Value);
            }
        }
        if (requiredTags != null && requiredTags.Count > 0) {
            result.RemoveAll(obj => !obj.HasAnyTag(requiredTags));
        }
        if (requiredZoneTags != null && requiredZoneTags.Count > 0) {
            result.RemoveAll(obj => !obj.CurrentZoneTags.Any(tag => requiredZoneTags.Contains(tag)));
        }

        return result;
    }

}
