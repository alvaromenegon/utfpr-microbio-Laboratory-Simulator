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

    private void Start() {
        prefabRegistry = PrefabRegistry.GetInstance();
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


    //public void RebuildIndex() {
    //    WillRebuildIndex?.Invoke();
    //    objectsById.Clear();
    //    objectsByIdentifier.Clear();
    //    objectsByInstanceId.Clear();
    //    LaboratoryObject[] interactables = FindObjectsByType<LaboratoryObject>(FindObjectsSortMode.None);

    //    foreach (var interactable in interactables) {
    //        string identifier = interactable.Identifier;
    //        if (string.IsNullOrWhiteSpace(identifier)) {
    //            Debug.LogError($"LaboratoryObject '{interactable.gameObject.name}' has no canonical identifier configured.");
    //            continue;
    //        }

    //        if (objectsById.TryGetValue(identifier, out var existing)) {
    //            Debug.LogWarning($"Multiple LaboratoryObject instances with identifier '{identifier}'. Objects: '{existing.gameObject.name}' and '{interactable.gameObject.name}'. This is allowed when using InstanceIds.");
    //        }

    //        objectsById[identifier] = interactable;

    //        if (!objectsByIdentifier.ContainsKey(identifier)) {
    //            objectsByIdentifier[identifier] = new List<LaboratoryObject>();
    //        }
    //        objectsByIdentifier[identifier].Add(interactable);

    //        if (!string.IsNullOrWhiteSpace(interactable.InstanceId)) {
    //            objectsByInstanceId[interactable.InstanceId] = interactable;
    //        }
    //        prefabRegistry.RegisterPrefab(identifier,interactable.gameObject);
    //    }
    //    OnRegistryUpdated?.Invoke();
    //}

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

    public List<LaboratoryObject> GetAllByIdentifier(string identifier) {
        if (IsNull(identifier)) {
            return new List<LaboratoryObject>();
        }
        return objectsByIdentifier.TryGetValue(identifier, out var list) ? list : new List<LaboratoryObject>(); 
    }

    public List<LaboratoryObject> Query(string canonicalIdentifier = null, List<string> requiredTags = null) {
        var result = new List<LaboratoryObject>();
        if (!string.IsNullOrWhiteSpace(canonicalIdentifier)) {
            if (objectsByIdentifier.TryGetValue(canonicalIdentifier, out var candidates)) {
                result.AddRange(candidates);
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
        return result;
    }

    //public void SetupRegistry(List<ExperimentInstanceObject> experimentObjects) {
    //    if (experimentObjects == null || experimentObjects.Count == 0) {
    //        Debug.Log("[ExperimentObjectRegistry] No objects defined in ExperimentData.");
    //        return;
    //    }

    //    RebuildIndex();

    //    foreach (var experimentObject in experimentObjects) {

    //            if (string.IsNullOrWhiteSpace(experimentObject.InstanceId)) {
    //                Debug.LogWarning($"[ExperimentObjectRegistry] Instance of object '{experimentObject.CanonicalIdentifier}' has no instanceId.");
    //                continue;
    //            }

    //            if (!TryGetObject(experimentObject.CanonicalIdentifier, out var laboratoryObject)) {
    //                Debug.LogWarning($"[ExperimentObjectRegistry] No LaboratoryObject found with identifier '{experimentObject.CanonicalIdentifier}' for instance '{experimentObject.InstanceId}'.");
    //                continue;
    //            }

    //            laboratoryObject.InstanceId = experimentObject.InstanceId;
    //            RegisterInstance(experimentObject.InstanceId, laboratoryObject);
    //            Debug.Log($"[ExperimentObjectRegistry] Registered instance '{experimentObject.InstanceId}' for object '{experimentObject.CanonicalIdentifier}'.");
    //        }
    //}

}
