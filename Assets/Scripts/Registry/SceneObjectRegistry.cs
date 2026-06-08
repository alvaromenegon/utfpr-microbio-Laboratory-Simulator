using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjectRegistry : MonoBehaviour {
    private Dictionary<string, LaboratoryObject> sceneObjects = new();
    private static SceneObjectRegistry _instance;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this);
            return;
        }
        _instance = this;
        RebuildIndex();
    }

    public void RebuildIndex() {
        sceneObjects.Clear();
        var objects = FindObjectsByType<LaboratoryObject>(FindObjectsSortMode.None);
        Debug.Log($"Registering {objects.Length} LaboratoryObjects in the scene.");
        foreach (var obj in objects) {
            Register(obj);
        }
    }

    public static SceneObjectRegistry GetInstance() {
        if (_instance == null) {
            _instance = FindFirstObjectByType<SceneObjectRegistry>();
            if (_instance == null) {
                Debug.LogError("No instance of SceneObjectRegistry found in the scene. Please add one to the scene.");
            }
        }
        return _instance;
    }

    public void Register(LaboratoryObject obj) {
        sceneObjects[obj.InstanceId] = obj;
        Debug.Log($"Registered {obj} on SceneObjectRegistry");
    }

    public bool TryGet(string instanceId, out LaboratoryObject obj) {
        return sceneObjects.TryGetValue(instanceId, out obj);
    }
}