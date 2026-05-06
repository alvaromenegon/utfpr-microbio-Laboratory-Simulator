using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem {
    private ExperimentObjectRegistry registry;
    private EffectRegistry effectRegistry;
    private LoggerService logger = new ("EffectSystem");
    private HashSet<string> executedTriggeredEffectsId; // armazena os IDs dos TriggeredEffects já executados para evitar execuções repetidas

    public EffectSystem(EffectRegistry effectRegistry) {
        this.effectRegistry = effectRegistry;
        registry = ExperimentObjectRegistry.Instance;
        executedTriggeredEffectsId = new HashSet<string>();
    }

    private void ApplyContinuousEffect(EffectDefinition effect, float deltatime) {
        if (effect == null) { return; }
        if (!CheckRegistry()) { return; }

        foreach (var operation in effect.Operations) {
            registry.TryGetObject(operation.TargetObject, out var obj);
            var currentValue = obj.Get<float>(effect.Property);
            obj.Set(effect.Property, currentValue + effect.Rate * deltatime);
            ExperimentHudPresenter.Instance.HandleFeedback($"Applied {obj}.{effect.Property}= {currentValue + effect.Rate * deltatime}");
        }//logger.Log($"EffectSystem: Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {effect.Property} with rate {effect.Rate * deltatime}");

    }

    public void HandleInstantEffect(InteractionRequest request) {
        logger.Log("HandleInstantEffect called with request: " + request);
        var effects = effectRegistry.GetEffects(request.Context);
        effects = effects.FindAll(e => !e.IsContinuous);
        foreach (var effect in effects) {
            if (ConditionEvaluator.Check(effect.Conditions)) {
                ApplyInstantEffect(effect);
            }           
        }
    }

    private void EvaluateTriggeredEffects() {
        var effects = effectRegistry.GetAllTriggeredEffects();
        foreach (var effect in effects) {
            if (ConditionEvaluator.Check(effect.Conditions)) {
                if (executedTriggeredEffectsId.Contains(effect.Id)) {
                    continue;
                }
                ApplyTriggeredEffect(effect.Id);
                executedTriggeredEffectsId.Add(effect.Id);
            }
        }
    }

    private bool CheckRegistry() {
        if (registry == null) {
            logger.Warning("ExperimentObjectRegistry instance is null. Attempting to rebuild index.");
            // tenta pegar novamente a instância do registry
            registry = ExperimentObjectRegistry.Instance;
            if (registry == null) {
                logger.Error("ExperimentObjectRegistry instance is still null after attempting to rebuild index.");
                return false;
            }
        }
        return true;
    }

    private void ApplyInstantEffect(EffectDefinition effect) {

        if (effect == null) { return; }
        if (!CheckRegistry()) { return; }
        logger.Log($"Applying instant effect: {effect}"); 
        
        foreach (var operation in effect.Operations) {
            registry.TryGetObject(operation.TargetObject, out var obj);
            if (obj == null) {
                logger.Error($"Object with identifier {operation.TargetObject} not found in registry.");
                continue;
            }
            obj.Set(operation.Property, operation.Value);
        }
    }

    public void ApplyValueAction(PlayerAction action) {
        if (registry.TryGetObject(action.Target, out var obj)) {

            obj.Set(action.Property, action.Value);
        }
        else {
            logger.Error($"Object with identifier {action.Target} not found in registry.");
        }
    }

    public void ApplyTriggeredEffect(string effectId) {
        var effect = effectRegistry.GetTriggeredEffect(effectId);
        if (effect == null) {
            logger.Error($"TriggeredEffect with identifier {effectId} not found in registry.");
            return;
        }
        if (registry == null) {
            logger.Warning("ExperimentObjectRegistry instance is null. Attempting to rebuild index.");
            // tenta pegar novamente a instância do registry
            registry = ExperimentObjectRegistry.Instance;
            if (registry == null) {
                logger.Error("ExperimentObjectRegistry instance is still null after attempting to rebuild index.");
                return;
            }
        }
        if (ConditionEvaluator.Check(effect.Conditions)) {
            foreach (var operation in effect.Operations) {
                //logger.Log("Applying operation on target: " + operation.TargetObject + " for property: " + operation.Property + " with value: " + operation.Value);
                ExperimentHudPresenter.Instance.HandleFeedback($"Applying operation on target: {operation.TargetObject} for property: {operation.Property} with value: {operation.Value}");
                registry.TryGetObject(operation.TargetObject, out var targetObj);
                if (targetObj == null) {
                    logger.Error($"Object with identifier {operation.TargetObject} not found in registry.");
                    continue;
                }
                targetObj.Set(operation.Property, operation.Value);
                logger.Log("[ApplyTriggeredEffect] Applied operation on target: " + operation.TargetObject + " for property: " + operation.Property + " with value: " + operation.Value);
            }
        }
    }

    public void Update(List<ActiveInteraction> activeInteractions) {
        foreach (var interaction in activeInteractions) {

            foreach (var effect in interaction.Effects) {
                //logger.Log("Evaluating continuous effect with source: " + effect.SourceObject + " and target: " + effect.TargetObject);
                if (effect.IsContinuous) {
                    if (ConditionEvaluator.Check(effect.Conditions)) {
                        ApplyContinuousEffect(effect, Time.deltaTime);
                    }
                }
            }
            EvaluateTriggeredEffects();
        }
    }
}