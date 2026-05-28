using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class VisualSlotRegistry {
    private readonly Dictionary<string, Dictionary<string, VisualSlot>> slotsByObject = new();
    private static VisualSlotRegistry _instance;
    private VisualSlotRegistry() {}

    public static VisualSlotRegistry GetInstance() {
        _instance ??= new VisualSlotRegistry();
        return _instance;
    }

    public void RegisterSlotsByObject(LaboratoryObject obj) {
        var slots = obj.GetComponentsInChildren<VisualSlot>(true);
        Debug.Log($"Found {slots.Length} VisualSlots in GameObject: {obj.InstanceId}");
        if (obj.TryGetComponent<VisualSlot>(out var objSlot)){
            Debug.Log($"Found VisualSlot directly on GameObject: {obj.InstanceId}");
            RegisterVisualSlot(obj, objSlot);
        }
        foreach (var slot in slots) {
            if (slot.Owner == null) {
                slot.SetOwner(obj);
            }
            RegisterVisualSlot(slot.Owner, slot);
        }
    }

    public void RegisterVisualSlot(LaboratoryObject owner, VisualSlot slot) {
        if (owner == null) {
            Debug.LogError($"Cannot register VisualSlot: {slot} because owner is null");
            return;
        }
        Debug.Log($"Registering VisualSlot: {slot} for Object: {owner}");
        if (!slotsByObject.TryGetValue(owner.InstanceId, out var slots)) {
            slots = new Dictionary<string, VisualSlot>();
            slotsByObject[owner.InstanceId] = slots;
        }

        slots[slot.SlotId] = slot;
    }

    public bool TryGetSlot(
       string instanceId,
       string slotId,
       out VisualSlot slot) {
        slot = null;

        if (!slotsByObject.TryGetValue(instanceId, out var slots))
            return false;

        return slots.TryGetValue(slotId, out slot);
    }
}