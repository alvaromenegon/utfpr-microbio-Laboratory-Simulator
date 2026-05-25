using UnityEngine;
using System.Collections.Generic;
using System;

public class PrefabRegistry : MonoBehaviour {
    [SerializeField] private List<PrefabEntry> prefabs;

    private static PrefabRegistry _instance;

    public static PrefabRegistry GetInstance() {
        if (_instance == null) {
            _instance = FindFirstObjectByType<PrefabRegistry>();
            if (_instance == null) {
                Debug.LogError("No PrefabRegistry instance found in the scene. Please ensure a PrefabRegistry component is present on a GameObject.");
            }
        }
        return _instance;
    }

    public GameObject GetPrefab(string identifier) {
        var prefab = prefabs.Find(entry => entry.CanonicalIdentifier == identifier);
        return prefab?.Prefab;
    }
 
}

[Serializable]
public class PrefabEntry {
    public string CanonicalIdentifier;
    public GameObject Prefab;
}