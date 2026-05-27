using System;
using System.Collections.Generic;

public class VisualStateRegistry {
    private Dictionary<string, VisualStateRule> visualStates = new();
    public void RegisterVisualState(string id, VisualStateRule state) {
        if (string.IsNullOrEmpty(id)) {
            throw new ArgumentException("Visual state ID cannot be null or empty.");
        }
        if (state == null) {
            throw new ArgumentNullException(nameof(state), "Visual state cannot be null.");
        }
        visualStates[id] = state;
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

    public IEnumerable<string> GetAllVisualStateIds() {
        return visualStates.Keys;
    }

    public IEnumerable<VisualStateRule> GetAllVisualStates() {
        return visualStates.Values;
    }
}