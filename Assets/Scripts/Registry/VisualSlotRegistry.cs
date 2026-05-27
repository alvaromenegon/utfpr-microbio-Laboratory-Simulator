using System.Collections.Generic;
using UnityEngine;

public class VisualSlotRegistry {
    private Dictionary<string, VisualSlot> objectSlots;
    private static VisualSlotRegistry _instance;
    private VisualSlotRegistry() {
        objectSlots = new Dictionary<string, VisualSlot>();
    }

    public static VisualSlotRegistry GetInstance() {
        _instance ??= new VisualSlotRegistry();
        return _instance;
    }

    public void RegisterSlotsByObject(GameObject obj) {
        var slots = obj.GetComponentsInChildren<VisualSlot>();
        foreach (var slot in slots) {
            RegisterVisualSlot(slot);
        }
    }

    public void RegisterVisualSlot(VisualSlot slot) {
        if (!objectSlots.ContainsKey(slot.SlotId)) {
            objectSlots.Add(slot.SlotId, slot);
        } else {
            // Handle duplicate ID case if necessary
            throw new System.Exception($"VisualSlot with ID {slot.SlotId} is already registered.");
        }
    }
    public VisualSlot GetVisualSlot(string id) {
        if (objectSlots.TryGetValue(id, out var slot)) {
            return slot;
        } else {
            // Handle not found case if necessary
            throw new System.Exception($"VisualSlot with ID {id} not found.");
        }
    }
    public IEnumerable<VisualSlot> GetAllVisualSlots() {
        return objectSlots.Values;
    }
}