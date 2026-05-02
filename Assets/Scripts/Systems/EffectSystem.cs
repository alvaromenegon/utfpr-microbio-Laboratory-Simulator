using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem {
    private ExperimentObjectRegistry registry;
    private EffectRegistry effectRegistry;
    private LoggerService logger = new ("EffectSystem");

    private List<ContinuousEffect> GetMatchingContinuousEffects(ActiveInteraction interaction) {
        // todo: implementar lógica para encontrar efeitos correspondentes 
        throw new System.NotImplementedException();

    }

    private void ApplyContinuousEffect(ContinuousEffect effect, float deltatime) {
        if (effect == null) { return; }
        var source = registry.TryGetInteractable(effect.SourceObject, out var obj);
        if (!source) {
            logger.Error($"EffectSystem: Could not find object with identifier {effect.SourceObject}");
            return;
        }
        var target = registry.TryGetInteractable(effect.TargetObject, out var targetObj);
        if (!target) {
            logger.Error($"EffectSystem: Could not find object with identifier {effect.TargetObject}");
            return;
        }
        if (targetObj == null) {
            logger.Error($"EffectSystem: Target object with identifier {effect.TargetObject} is null");
            return;
        }
        targetObj.Set(effect.Property, effect.Rate * deltatime);
        logger.Log($"EffectSystem: Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {effect.Property} with rate {effect.Rate * deltatime}");

    }

    public void HandleInstantEffect(InteractionRequest request) {
        var effects = effectRegistry.GetInstantEffects(request);
        foreach (var effect in effects) {
           if (ConditionEvaluator.Check(effect.Conditions)) {
                ApplyInstantEffect(effect);
            }
        }
    }

    private void ApplyInstantEffect(InstantEffect effect) {
        if (effect == null) { return; }
        var source = registry.TryGetInteractable(effect.SourceObject, out var obj);
        if (!source) {
            logger.Error($"EffectSystem: Could not find object with identifier {effect.SourceObject}");
            return;
        }
        var target = registry.TryGetInteractable(effect.TargetObject, out var targetObj);
        if (!target) {
            logger.Error($"EffectSystem: Could not find object with identifier {effect.TargetObject}");
            return;
        }
        if (targetObj == null) {
            logger.Error($"EffectSystem: Target object with identifier {effect.TargetObject} is null");
            return;
        }
        targetObj.Set(effect.Property, effect.Value);
        logger.Log($"EffectSystem: Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {effect.Property} with value {effect.Value}");
    }

    public void ApplyValueAction(PlayerAction action) {
        if (registry.TryGetInteractable(action.Target, out var obj)) {

            obj.Set(action.Property, action.Value);
        }
        else {
            logger.Error($"Object with identifier {action.Target} not found in registry.");
        }
    }

    public void Update(List<ActiveInteraction> activeInteractions) {
        foreach (var interaction in activeInteractions) {
            var effects = GetMatchingContinuousEffects(interaction);

            foreach (var effect in effects) {
                if (ConditionEvaluator.Check(effect.Conditions)) {
                    ApplyContinuousEffect(effect, Time.deltaTime);
                }
            }
        }
    }
}