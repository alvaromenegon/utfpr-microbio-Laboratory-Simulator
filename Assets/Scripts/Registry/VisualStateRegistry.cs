using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualStateRegistry {
    private static VisualStateRegistry _instance;
    private Dictionary<string, VisualStateRule> visualStates = new();

    private VisualStateRegistry() { }

    public static VisualStateRegistry Instance {
        get {
            _instance ??= new VisualStateRegistry();
            return _instance;
        }
    }
    public void RegisterVisualState(string id, VisualStateRule state) {
        if (string.IsNullOrEmpty(id)) {
            throw new ArgumentException("Visual state ID cannot be null or empty.");
        }

        visualStates[id] = state ?? throw new ArgumentNullException(nameof(state), "Visual state cannot be null.");
    }
    public VisualStateRule GetVisualState(string id) {
        if (string.IsNullOrEmpty(id)) {
            throw new ArgumentException("Visual state ID cannot be null or empty.");
        }
        if (visualStates.TryGetValue(id, out var state)) {
            return state;
        }
        throw new KeyNotFoundException($"No visual state found with ID: {id}");
    }

    public List<string> GetAllVisualStateIds() {
        return new List<string>(visualStates.Keys);
    }

    public List<VisualStateRule> GetAllVisualStates() {
        return new List<VisualStateRule>(visualStates.Values);
    }

    public void SetupRegistry(List<VisualStateRule> states) {
        foreach (var state in states) {
            RegisterVisualState(state.Id, state);
        }
        Debug.Log($"VisualStateRegistry setup complete with {states.Count} states.");
        Debug.Log($"Registered Visual State IDs: {string.Join(", ", GetAllVisualStateIds())}");
    }
}