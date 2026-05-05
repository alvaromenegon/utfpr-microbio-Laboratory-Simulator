using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem {
    private ExperimentObjectRegistry registry;
    private EffectRegistry effectRegistry;
    private LoggerService logger = new ("EffectSystem");
    private HashSet<string> executedEffectsId; // armazena os IDs dos efeitos instantâneos já executados para evitar execuções repetidas

    private List<ContinuousEffect> GetMatchingContinuousEffects(ActiveInteraction interaction) {
        var effects = effectRegistry.GetContinuousEffects(interaction.Type, interaction.Source, interaction.Target);
        logger.Log("Found " + effects.Count + " matching continuous effects for the active interaction.");
        return effects;

    }

    public EffectSystem(EffectRegistry effectRegistry) {
        this.effectRegistry = effectRegistry;
        registry = ExperimentObjectRegistry.Instance;
        executedEffectsId = new HashSet<string>();
    }

    private void ApplyContinuousEffect(ContinuousEffect effect, float deltatime) {
        if (effect == null) { return; }
        if (registry == null) {
            logger.Warning("ExperimentObjectRegistry instance is null. Attempting to rebuild index.");
            // tenta pegar novamente a instância do registry
            registry = ExperimentObjectRegistry.Instance;
            if (registry == null) {
                logger.Error("ExperimentObjectRegistry instance is still null after attempting to rebuild index.");
                return;
            }
        }
        var source = registry.TryGetInteractable(effect.SourceObject, out var obj);
        if (!source) {
            logger.Error($"Could not find object with identifier {effect.SourceObject}");
            return;
        }
        var target = registry.TryGetInteractable(effect.TargetObject, out var targetObj);
        if (!target) {
            logger.Error($"Could not find object with identifier {effect.TargetObject}");
            return;
        }
        if (targetObj == null) {
            logger.Error($"Target object with identifier {effect.TargetObject} is null");
            return;
        }
        var currentValue = targetObj.Get<float>(effect.Property);
        targetObj.Set(effect.Property, currentValue + effect.Rate * deltatime);
        logger.Log($"EffectSystem: Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {effect.Property} with rate {effect.Rate * deltatime}");

    }

    public void HandleInstantEffect(InteractionRequest request) {
        logger.Log("HandleInstantEffect called with request: " + request.Type + " between " + request.PrimaryObject + " and " + request.SecondaryObject);
        var effects = effectRegistry.GetInstantEffects(request.Type,  request.SecondaryObject, request.PrimaryObject);
        logger.Log("Found " + effects.Count + " matching instant effects for the interaction request.");
        foreach (var effect in effects) {
            logger.Log("Evaluating effect with source: " + effect.SourceObject + " and target: " + effect.TargetObject);
            if (ConditionEvaluator.Check(effect.Conditions)) {
                ApplyInstantEffect(effect);
            }
            else {
                logger.Log("Conditions not met for effect with source: " + effect.SourceObject + " and target: " + effect.TargetObject);
            }
        }
    }

    private void EvaluateTriggeredEffects() {
        var effects = effectRegistry.GetAllTriggeredEffects();
        foreach (var effect in effects) {
            if (ConditionEvaluator.Check(effect.Conditions)) {
                if (executedEffectsId.Contains(effect.Id)) {
                    continue;
                }
                ApplyTriggeredEffect(effect.Id);
                executedEffectsId.Add(effect.Id);
            }
        }
    }

    private void ApplyInstantEffect(InstantEffect effect) {

        if (effect == null) { return; }
        if (registry == null) {
            logger.Warning("ExperimentObjectRegistry instance is null. Attempting to rebuild index.");
            // tenta pegar novamente a instância do registry
            registry = ExperimentObjectRegistry.Instance;
            if (registry == null) {
                logger.Error("ExperimentObjectRegistry instance is still null after attempting to rebuild index.");
                return;
            }
        }
        logger.Log($"Applying instant effect: {effect}"); 
        var source = registry.TryGetInteractable(effect.SourceObject, out var obj);
        if (!source && effect.HasSource) {
            logger.Error($"Could not find source object with identifier {effect.SourceObject}");
            return;
        }
        var target = registry.TryGetInteractable(effect.TargetObject, out var targetObj);
        if (!target) {
            logger.Error($"Could not find target object with identifier {effect.TargetObject}");
            return;
        }
        if (targetObj == null) {
            logger.Error($"Target object with identifier {effect.TargetObject} is null");
            return;
        }
        foreach (var operation in effect.Operations) {
            logger.Log($"Applying instant effect from {effect.SourceObject} to {effect.TargetObject} on property {operation.Property} with value {operation.Value}");
            targetObj.Set(operation.Property, operation.Value);
            logger.Log($"Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {operation.Property} with value {operation.Value}");
        }
    }

    public void ApplyValueAction(PlayerAction action) {
        if (registry.TryGetInteractable(action.Target, out var obj)) {

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
                logger.Log("Applying operation on target: " + operation.TargetObject + " for property: " + operation.Property + " with value: " + operation.Value);
                registry.TryGetInteractable(operation.TargetObject, out var targetObj);
                if (targetObj == null) {
                    logger.Error($"Object with identifier {operation.TargetObject} not found in registry.");
                    continue;
                }
                targetObj.Set(operation.Property, operation.Value);
            }
        }
    }

    public void Update(List<ActiveInteraction> activeInteractions) {
        foreach (var interaction in activeInteractions) {
            var effects = GetMatchingContinuousEffects(interaction);

            foreach (var effect in effects) {
                //logger.Log("Evaluating continuous effect with source: " + effect.SourceObject + " and target: " + effect.TargetObject);
                if (ConditionEvaluator.Check(effect.Conditions)) {
                    ApplyContinuousEffect(effect, Time.deltaTime);
                }
            }
            EvaluateTriggeredEffects();
        }
    }
}