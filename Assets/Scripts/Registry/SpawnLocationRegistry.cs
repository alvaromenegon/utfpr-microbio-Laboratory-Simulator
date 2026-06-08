using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnLocationRegistry : MonoBehaviour {
    public Dictionary<string, SpawnPoint> spawnLocations;
    private static SpawnLocationRegistry _instance;

    public static SpawnLocationRegistry GetInstance() {
        if (_instance == null) {
            _instance = FindFirstObjectByType<SpawnLocationRegistry>();
            if (_instance == null) {
                var obj = new GameObject("SpawnLocationRegistry");
                _instance = obj.AddComponent<SpawnLocationRegistry>();
            }
        }
        return _instance;
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start() {
        spawnLocations = new Dictionary<string, SpawnPoint>();
        var spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var spawnPoint in spawnPoints) {
            if (spawnLocations.ContainsKey(spawnPoint.Id)) {
                Debug.LogWarning($"Duplicate spawn point ID detected: {spawnPoint.Id}. Ignoring this spawn point.");
                continue;
            }
            spawnLocations.Add(spawnPoint.Id, spawnPoint);
        }
    }

    public bool TryResolve(string spawnLocation, out Transform transform) {
        if (spawnLocations.TryGetValue(spawnLocation, out SpawnPoint spawnPoint)) {
            transform = spawnPoint.transform;
            return true;
        } else {
            Debug.LogError($"Spawn location '{spawnLocation}' not found in registry.");
            transform = null;
            return false;
        }
    }
}
