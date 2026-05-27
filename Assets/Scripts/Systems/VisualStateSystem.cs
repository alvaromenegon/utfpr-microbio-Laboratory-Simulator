using System.Collections.Generic;
using UnityEngine;

public class VisualStateSystem {
    private VisualStateRegistry visualStateRegistry;
    private VisualSlotRegistry visualSlotRegistry;
    private List<string> appliedVisualStates;
    public VisualStateSystem(VisualStateRegistry visualStateRegistry, VisualSlotRegistry visualSlotRegistry) {
        this.visualStateRegistry = visualStateRegistry;
        this.visualSlotRegistry = visualSlotRegistry;
        appliedVisualStates = new List<string>();
    }
    public void HandleVisualState(VisualStateRule state) {
        var slot = visualSlotRegistry.GetObjectSlot(state.Slot);
        if (slot == null) {
            UnityEngine.Debug.LogWarning($"VisualStateSystem: Slot {state.Slot} not found for visual state {state.Id}");
            return;
        }
        
        switch (state.ActionType) {      
            case VisualActionType.SET_ACTIVE:
                HandleActivateObject(state, slot);
                break;
            case VisualActionType.SET_MATERIAL:
                HandleMaterialOverride(state, slot);
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
            default:
                Debug.LogWarning($"VisualStateSystem: Unsupported action type {state.ActionType} for visual state {state.Id}");
                break;
        }
        appliedVisualStates.Add(state.Id);

    }

    // todo: implementar cada tipo de visual target
    // métodos específicos para cada tipo de visual target
    private void HandleActivateObject(VisualStateRule state, VisualSlot slot) {
        var targetSlot = visualSlotRegistry.GetObjectSlot(state.Slot);
        if (targetSlot != null) {
            targetSlot.gameObject.SetActive(true);
        } else {
            Debug.LogWarning($"VisualStateSystem: Target slot {state.Slot} not found for visual state {state.Id}");
        }
    }

    private void HandleMaterialOverride(VisualStateRule state, VisualSlot slot) {
        var targetSlot = visualSlotRegistry.GetObjectSlot(state.Slot);
        if (targetSlot == null) {
            Debug.LogWarning($"VisualStateSystem: Target slot {state.Slot} not found for visual state {state.Id}");
            return;
        }
        if (targetSlot.gameObject.TryGetComponent<Renderer>(out var renderer)) {
            var material = Resources.Load<Material>(state.VisualAssetId);
            if (material != null) {
                renderer.material = material;
            }
            else {
                Debug.LogWarning($"VisualStateSystem: Material {state.VisualAssetId} not found for visual state {state.Id}");
            }
        }
        else {
            Debug.LogWarning($"VisualStateSystem: Renderer not found on slot {state.Slot} for visual state {state.Id}");
        }
    }

    private void HandleAnimation(VisualStateRule state, VisualSlot slot) {
        // Implementar lógica para aplicar microscope view
    }

    private void HandleSpawnPrefab(VisualStateRule state, VisualSlot slot) {
        // Implementar lógica para aplicar world object
    }

    private void HandleUIPanel(VisualStateRule state, VisualSlot slot) {
        // Implementar lógica para aplicar UI panel
    }

    public void Update() {
        foreach (var state in visualStateRegistry.GetAllVisualStates()) {
            if (ConditionEvaluator.Check(state.Conditions)) {
                if (!appliedVisualStates.Contains(state.Id)) {
                    HandleVisualState(state);
                }
            }            
        }
    }
}