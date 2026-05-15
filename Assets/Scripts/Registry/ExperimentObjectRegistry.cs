using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExperimentObjectRegistry : MonoBehaviour {
    private readonly Dictionary<string, LaboratoryObject> objectsById = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<LaboratoryObject>> objectsByIdentifier = new(StringComparer.Ordinal);
    private readonly Dictionary<string, LaboratoryObject> objectsByInstanceId = new(StringComparer.Ordinal);
    private PrefabRegistry prefabRegistry;
    public event Action WillRebuildIndex;
    public event Action OnRegistryUpdated;

    public static ExperimentObjectRegistry Instance { get; private set; }


    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        prefabRegistry = PrefabRegistry.GetInstance();
        RebuildIndex();
    }

    public void RebuildIndex() {
        WillRebuildIndex?.Invoke();
        objectsById.Clear();
        objectsByIdentifier.Clear();
        objectsByInstanceId.Clear();
        LaboratoryObject[] interactables = FindObjectsByType<LaboratoryObject>(FindObjectsSortMode.None);

        foreach (var interactable in interactables) {
            string identifier = interactable.Identifier;
            if (string.IsNullOrWhiteSpace(identifier)) {
                Debug.LogError($"LaboratoryObject '{interactable.gameObject.name}' has no canonical identifier configured.");
                continue;
            }

            if (objectsById.TryGetValue(identifier, out var existing)) {
                Debug.LogWarning($"Multiple LaboratoryObject instances with identifier '{identifier}'. Objects: '{existing.gameObject.name}' and '{interactable.gameObject.name}'. This is allowed when using InstanceIds.");
            }

            objectsById[identifier] = interactable;

            if (!objectsByIdentifier.ContainsKey(identifier)) {
                objectsByIdentifier[identifier] = new List<LaboratoryObject>();
            }
            objectsByIdentifier[identifier].Add(interactable);

            if (!string.IsNullOrWhiteSpace(interactable.InstanceId)) {
                objectsByInstanceId[interactable.InstanceId] = interactable;
            }
            prefabRegistry.RegisterPrefab(identifier,interactable.gameObject);
        }
        OnRegistryUpdated?.Invoke();
    }

    private bool IsNull(string identifier) {
        return string.IsNullOrWhiteSpace(identifier) || !objectsById.ContainsKey(identifier);
    }

    public bool TryGetObject(string identifier, out LaboratoryObject interactable) {
        interactable = null;
        if (IsNull(identifier)) {
            return false;
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

    public List<LaboratoryObject> GetAllByIdentifier(string identifier) {
        if (IsNull(identifier)) {
            return new List<LaboratoryObject>();
        }
        return objectsByIdentifier.TryGetValue(identifier, out var list) ? list : new List<LaboratoryObject>(); 
    }

}
