using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem {
    private ExperimentObjectRegistry registry;
    private EffectRegistry effectRegistry;
    private readonly LoggerService logger = new ("EffectSystem");
    private readonly HashSet<string> executedTriggeredEffectsId; // armazena os IDs dos TriggeredEffects já executados para evitar execuções repetidas

    public EffectSystem(EffectRegistry effectRegistry) {
        this.effectRegistry = effectRegistry;
        registry = ExperimentObjectRegistry.Instance;
        executedTriggeredEffectsId = new HashSet<string>();
    }

    public void HandleInstantEffect(InteractionRequest request) {
        logger.Log("HandleInstantEffect called with request: " + request);
        var effects = effectRegistry.GetEffects(request.Context);
        effects = effects.FindAll(e => !e.IsContinuous);
        foreach (var effect in effects) {
            if (ConditionEvaluator.Check(effect.Conditions)) {
                ApplyInstantEffect(effect, request.Context);
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
                ApplyTriggeredEffect(effect);
                executedTriggeredEffectsId.Add(effect.Id);
                logger.Log($"[EvaluateTriggeredEffects] TriggeredEffect with ID {effect.Id} has been applied and will not be applied again.");
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

    /* aplicação de efeitos */

    private void ApplyContinuousEffect(EffectDefinition effect, InteractionContext ctx, float deltatime) {
        if (effect == null) { return; }
        if (!CheckRegistry()) { return; }

        foreach (var operation in effect.Operations) {
            if (!TryResolveTarget(operation, ctx, out var obj)) {
                logger.Error($"Failed to resolve target for operation: {operation}");
                continue;
            }
            var currentValue = obj.GetProperty<float>(operation.Property);
            obj.SetProperty(operation.Property, currentValue + effect.Rate.Value * deltatime);
            ExperimentHudPresenter.Instance.HandleFeedback($"Applied {obj.Identifier}.{operation.Property}= {currentValue + effect.Rate * deltatime}");
        }//logger.Log($"EffectSystem: Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {effect.Property} with rate {effect.Rate * deltatime}");

    }

    private void ApplyInstantEffect(EffectDefinition effect, InteractionContext ctx) {
        if (effect == null) { return; }
        if (!CheckRegistry()) { return; }
        
        foreach (var operation in effect.Operations) {
            var obj = ResolveTarget(operation, ctx);
            obj.SetProperty(operation.Property, operation.Value);
            logger.Log($"Applied {obj.Identifier}.{operation.Property}= {operation.Value}");

        }
    }

    public void ApplyTriggeredEffect(ReactiveEffect effect) {
        if (effect == null) {
            logger.Error("TriggeredEffect is null.");
            return;
        }
        if (!CheckRegistry()) {
            logger.Error("ExperimentObjectRegistry is not available.");
            return;
        }
        logger.Log($"Evaluating triggered effect with ID: {effect.Id} and conditions: {effect.Conditions}");
        if (ConditionEvaluator.Check(effect.Conditions)) {
            foreach (var operation in effect.Operations) {
                //logger.Log("Applying operation on target: " + operation.TargetObject + " for property: " + operation.Property + " with value: " + operation.Value);
                ExperimentHudPresenter.Instance.HandleFeedback($"Applying operation on target: {operation.TargetObject} for property: {operation.Property} with value: {operation.Value}");
                registry.TryGetObject(operation.TargetObject, out var targetObj);
                if (targetObj == null) {
                    logger.Error($"Object with identifier {operation.TargetObject} not found in registry.");
                    continue;
                }
                targetObj.SetProperty(operation.Property, operation.Value);
                logger.Log("[ApplyTriggeredEffect] Applied operation on target: " + operation.TargetObject + " for property: " + operation.Property + " with value: " + operation.Value);
            }
        }
    }

    public void ApplyActionEffect(ActionEffect effect) {
        if (effect == null) {
            logger.Error("ActionEffect is null.");
            return;
        }
        if (!CheckRegistry()) {
            logger.Error("ExperimentObjectRegistry is not available.");
            return;
        }
        foreach (var operation in effect.Operations) {
            registry.TryGetObject(operation.TargetObject, out var targetObj);
            if (targetObj == null) {
                logger.Error($"Object with identifier {operation.TargetObject} not found in registry.");
                continue;
            }
            targetObj.SetProperty(operation.Property, operation.Value);
            logger.Log($"Applied ActionEffect on target: {operation.TargetObject} for property: {operation.Property} with value: {operation.Value}");
        }
    }

    /* */

    public void ApplyValueAction(PlayerAction action) {
        if (!CheckRegistry() || action == null) { return; }
        var obj = action.Context.Roles[RoleType.TARGET];
        if (obj == null) {
            logger.Error("Failed to resolve target from context.");
            return;
        }
        obj.SetProperty(action.Property, action.Value);
    }

    private bool TryResolveTarget(EffectOperation operation, InteractionContext ctx, out LaboratoryObject obj) {
        obj = null;
        if (operation.TargetRole.HasValue) {
            return ctx.Roles.TryGetValue(operation.TargetRole.Value, out obj);
        }
        return registry.TryGetObject(operation.TargetObject, out obj);
    }

    private LaboratoryObject ResolveTarget(EffectOperation operation, InteractionContext ctx) {
        if (operation.TargetRole.HasValue) {
            return ctx.Roles[operation.TargetRole.Value];
        }
        registry.TryGetObject(operation.TargetObject, out var obj);
        return obj;
    }

    public void Update(List<ActiveInteraction> activeInteractions) {
        foreach (var interaction in activeInteractions) {
            foreach (var effect in interaction.Effects) {
                //logger.Log("Evaluating continuous effect with source: " + effect.SourceObject + " and target: " + effect.TargetObject);
                if (effect.IsContinuous) {
                    if (ConditionEvaluator.Check(effect.Conditions)) {
                        ApplyContinuousEffect(effect, interaction.Context, Time.deltaTime);
                    }
                }
            }            
        }
        EvaluateTriggeredEffects();
    }
}