using Assets.Scripts;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class VisualStateSystem {
    private VisualStateRegistry visualStateRegistry;
    private VisualSlotRegistry visualSlotRegistry;
    private readonly Dictionary<string, bool> appliedActiveStateBySlot = new();
    private readonly Dictionary<string, string> appliedMaterialBySlot = new();
    private readonly Dictionary<string, string> appliedImageBySlot = new();
    private VisualAssetRegistry visualAssetRegistry;
    public VisualStateSystem(VisualSlotRegistry visualSlotRegistry) {
        visualStateRegistry = VisualStateRegistry.Instance;
        this.visualSlotRegistry = visualSlotRegistry;
        visualAssetRegistry = VisualAssetRegistry.GetInstance();
    }

    private string GetSlotKey(LaboratoryObject obj, string slotId) {
        return $"{obj.InstanceId}:{slotId}";
    }
    public void HandleVisualState(VisualStateRule state) {
        var resolved = new ObjectResolver(state.Subject).Resolve();
        foreach (var subject in resolved) {
            var context = new InteractionContext();
            context.Roles[RoleType.SUBJECT] = subject;

            if (!ConditionEvaluator.Check(state.Conditions, context)) {
                //Debug.Log($"Conditions not met for visual state {state.Id} on subject {subject.InstanceId} and context {context}, skipping.");
                continue;
            }

            if (!visualSlotRegistry.TryGetSlot(subject.InstanceId, state.Slot, out var slot)) {
                Debug.LogWarning($"VisualStateSystem: Slot {state.Slot} not found for subject {subject.InstanceId} in visual state {state.Id}");
                continue;
            }

            ApplyVisualAction(state, slot, subject);
        }
    }

    public void ApplyVisualAction(VisualStateRule state, VisualSlot slot, LaboratoryObject subject) {
        switch (state.ActionType) {      
            case VisualActionType.SET_ACTIVE:
                HandleActivateObject(slot, (bool)state.Value, subject);
                break;
            case VisualActionType.SET_MATERIAL:
                HandleMaterialOverride(state, slot, subject);
                break;
            case VisualActionType.PLAY_ANIMATION:
                HandleAnimation(state, slot);
                break;
            case VisualActionType.SPAWN_PREFAB:
                HandleSpawnPrefab(state, slot);
                break;
            case VisualActionType.SHOW_UI_IMAGE:
                HandleUIPanel(state, slot);
                break;
            case VisualActionType.SET_IMAGE:
                HandleSetImage(state, slot);
                break;
            default:
                Debug.LogWarning($"VisualStateSystem: Unsupported action type {state.ActionType} for visual state {state.Id}");
                break;
        }

    }

    // todo: implementar cada tipo de visual target
    // métodos específicos para cada tipo de visual target
    private void HandleActivateObject(VisualSlot slot, bool value, LaboratoryObject subject) {
        var key = GetSlotKey(subject, slot.SlotId);
        if (appliedActiveStateBySlot.TryGetValue(key, out var currentValue) && currentValue == value) {
            return;
        }
        Debug.Log($"Applying visual action SET_ACTIVE on slot {slot} for subject {slot.gameObject.name}");
        slot.gameObject.SetActive(value);
        appliedActiveStateBySlot[key] = value;
    }

    private void HandleSetImage(VisualStateRule state, VisualSlot slot) {
        var key = GetSlotKey(slot.Owner, slot.SlotId);
        if (appliedImageBySlot.TryGetValue(key, out var currentAssetId) && currentAssetId == state.VisualAssetId) {
            return;
        }
        if (!visualAssetRegistry.TryGetSprite(state.VisualAssetId, out var sprite)) {
            Debug.LogWarning($"Sprite asset {state.VisualAssetId} not found for visual state {state.Id}");
            return;
        }
        if (slot.TargetImage == null) {
            Debug.LogWarning($"Slot {slot.SlotId} has no image component.");
            return;
        }
        slot.TargetImage.sprite = sprite;
    }

    private void HandleMaterialOverride(VisualStateRule state, VisualSlot slot, LaboratoryObject subject) {
        var key = GetSlotKey(subject, state.Slot);
        if (appliedMaterialBySlot.TryGetValue(key, out var currentAssetId) && currentAssetId == state.VisualAssetId) {
            return;
        }

        if (!visualAssetRegistry.TryGetMaterial(state.VisualAssetId, out var material)) {
            Debug.LogWarning($"Material asset {state.VisualAssetId} not found for visual state {state.Id}");
            return;
        }

        if (slot.TargetRenderer == null) {
            Debug.LogWarning($"Slot {slot.SlotId} has no renderer.");
            return;
        }
        Debug.Log($"Applying visual action SET_MATERIAL for state {state.Id} on slot {slot} for subject {subject.InstanceId}");


        var materials = slot.TargetRenderer.materials;

        if (slot.MaterialIndex < 0 || slot.MaterialIndex >= materials.Length) {
            Debug.LogWarning(
                $"Invalid material index {slot.MaterialIndex} for slot {slot.SlotId}.");
            return;
        }

        materials[slot.MaterialIndex] = material;
        slot.TargetRenderer.materials = materials;
        Debug.Log($"Material {state.VisualAssetId} applied to slot {slot.SlotId} on subject {subject.InstanceId}");

        appliedMaterialBySlot[key] = state.VisualAssetId;
    }

    private void HandleAnimation(VisualStateRule state, VisualSlot slot) {
        // Implementar lógica para aplicar microscope view
    }

    private void HandleSpawnPrefab(VisualStateRule state, VisualSlot slot) {
        // Implementar lógica para aplicar world object
    }

    private void HandleUIPanel(VisualStateRule state, VisualSlot slot) {
        
    }

    public void Update() {
        foreach (var state in visualStateRegistry.GetAllVisualStates()) {
            HandleVisualState(state);
        }
    }
}