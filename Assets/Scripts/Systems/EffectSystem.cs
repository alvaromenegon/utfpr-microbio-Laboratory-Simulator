using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectSystem {
    private ExperimentObjectRegistry registry;
    private EffectRegistry effectRegistry;
    private BackgroundProcessSystem backgroundProcessSystem;
    private readonly LoggerService logger = new ("EffectSystem");
    private readonly HashSet<string> executedReactiveEffects = new();

    public EffectSystem(EffectRegistry effectRegistry) {
        this.effectRegistry = effectRegistry;
        registry = ExperimentObjectRegistry.GetInstance();
    }

    public void BindBackgroundProcessSystem(BackgroundProcessSystem backgroundProcessSystem) {
        this.backgroundProcessSystem = backgroundProcessSystem;
    }

    public void HandleInstantEffect(InteractionRequest request) {
        logger.Log("HandleInstantEffect called with request: " + request);
        var effects = effectRegistry.GetEffects(request.Context);
        effects = effects.FindAll(e => !e.IsContinuous);
        foreach (var effect in effects) {
            if (ConditionEvaluator.Check(effect.Conditions, request.Context)) {
                ApplyInstantEffect(effect, request.Context);
            }           
        }
    }

    private void EvaluateReactiveEffects() {
        var effects = effectRegistry.GetAllReactiveEffects();
        foreach (var effect in effects) {
            var objectResolver = new ObjectResolver(effect.Subject, null);
            var subjects = objectResolver.Resolve();

            foreach (var subject in subjects) {
                var context = new InteractionContext();
                context.Roles[RoleType.SUBJECT] = subject;

                string executionKey = $"{effect.Id}:{subject.InstanceId}";

                if (effect.ExecuteOncePerSubject &&
                    executedReactiveEffects.Contains(executionKey)) {
                    continue;
                }

                if (!ConditionEvaluator.Check(effect.Conditions, context))
                    continue;

                ApplyReactiveEffect(effect, context);
                logger.Log($"[EvaluateReactiveEffects] ReactiveEffect with ID {effect.Id} has been applied.");
                if (effect.ExecuteOncePerSubject)
                    executedReactiveEffects.Add(executionKey);

            }  
        }
    }

    private bool CheckRegistry() {
        if (registry == null) {
            logger.Warning("ExperimentObjectRegistry instance is null. Attempting to rebuild index.");
            // tenta pegar novamente a instância do registry
            registry = ExperimentObjectRegistry.GetInstance();
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
            
            var targets = new ObjectResolver(operation.Target, ctx).Resolve();
            if (targets.Count == 0) {
                logger.Error($"Failed to resolve target for operation: {operation}");
                continue;
            }
            foreach (var obj in targets) {
                var currentValue = obj.GetProperty<float>(operation.Property);
                obj.SetProperty(operation.Property, currentValue + effect.Rate.Value * deltatime);
                ExperimentHudPresenter.Instance.HandleFeedback($"Aplicado {currentValue + effect.Rate * deltatime}% na propriedade {operation.Property} do objeto {obj.DisplayName} ({obj.InstanceId})");
            }
        }//logger.Log($"EffectSystem: Applied effect from {effect.SourceObject} to {effect.TargetObject} on property {effect.Property} with rate {effect.Rate * deltatime}");

    }

    private void ApplyInstantEffect(EffectDefinition effect, InteractionContext ctx) {
        if (effect == null) { return; }
        if (!CheckRegistry()) { return; }
        
        foreach (var operation in effect.Operations) {
            if (operation.OperationType == EffectOperationType.START_PROCESS) {
                logger.Log($"Found START_PROCESS operation for ProcessId: {operation.ProcessId}. Delegating to BackgroundProcessSystem.");
                backgroundProcessSystem.TryStartProcessById(operation.ProcessId, ctx);
                continue;
            }
            logger.Log($"Applying operation: {operation.Property} = {operation.Value}");
            logger.Log($"Target reference: Role={operation.Target?.Role}, InstanceId={operation.Target?.InstanceId}, Canonical={operation.Target?.CanonicalIdentifier}");

            var targets = new ObjectResolver(operation.Target, ctx).Resolve();
            if (targets.Count == 0) {
                logger.Error($"Failed to resolve target for operation: {operation}");
                continue;
            }
            logger.Log($"Resolved targets: {string.Join(", ", targets.Select(t => t.InstanceId))}");
            foreach (var obj in targets) {
                obj.SetProperty(operation.Property, operation.Value);
                logger.Log($"Applied {obj.Identifier}.{operation.Property}= {operation.Value}");
                ExperimentHudPresenter.Instance.HandleFeedback($"Aplicado {operation.Value} na propriedade {operation.Property} do objeto {obj.InstanceId}");
            }
        }
    }

    public void ApplyReactiveEffect(ReactiveEffect effect, InteractionContext context=null) {
        if (effect == null) {
            logger.Error("ReactiveEffect is null.");
            return;
        }
        if (!CheckRegistry()) {
            logger.Error("ExperimentObjectRegistry is not available.");
            return;
        }
        logger.Log($"Evaluating reactive effect with ID: {effect.Id} and conditions: {effect.Conditions}");
        if (ConditionEvaluator.Check(effect.Conditions, context)) {
            foreach (var operation in effect.Operations) {
                var targets = new ObjectResolver(operation.Target, context).Resolve();
                if (targets.Count == 0) {
                    logger.Error($"Failed to resolve target for operation: {operation}");
                    continue;
                }
                logger.Log($"Applying reactive effect operation: {operation} on targets: {string.Join(", ", targets.Select(t => t.InstanceId))}");
                foreach (var targetObj in targets) {
                    targetObj.SetProperty(operation.Property, operation.Value);
                    ExperimentHudPresenter.Instance.HandleFeedback($"Aplicado {operation.Value} na propriedade {operation.Property} do objeto {targetObj.InstanceId}");

                    logger.Log("[ApplyReactiveEffect] Applied operation on target: " + operation.Target + " for property: " + operation.Property + " with value: " + operation.Value);
                }
            }
        }
    }

    public void ApplyActionEffect(ActionEffect effect, InteractionContext context=null) {
        if (effect == null) {
            logger.Error("ActionEffect is null.");
            return;
        }
        if (!CheckRegistry()) {
            logger.Error("ExperimentObjectRegistry is not available.");
            return;
        }
        foreach (var operation in effect.Operations) {
            var targets = new ObjectResolver(operation.Target, context).Resolve();
            if (targets.Count == 0) {
                logger.Error($"Failed to resolve target for operation: {operation}");
            } 
            foreach (var targetObj in targets) {
                targetObj.SetProperty(operation.Property, operation.Value);
                ExperimentHudPresenter.Instance.HandleFeedback($"Aplicado {operation.Value} na propriedade {operation.Property} do objeto {targetObj.DisplayName} ({targetObj.InstanceId})");
                logger.Log($"Applied ActionEffect on target: {targetObj.DisplayName} ({targetObj.InstanceId}) for property: {operation.Property} with value: {operation.Value}");
                logger.Log($"Current properties of {targetObj.DisplayName} ({targetObj.InstanceId}): {string.Join(", ", targetObj.GetAllProperties().Select(kv => $"{kv.Key}: {kv.Value}"))}");
            }
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

    public void Update(List<ActiveInteraction> activeInteractions) {
        if (activeInteractions.Count > 0) logger.Log($"EffectSystem Update called with {activeInteractions.Count} active interactions.");
        foreach (var interaction in activeInteractions) {
            foreach (var effect in interaction.Effects) {
                //logger.Log("Evaluating continuous effect with source: " + effect.SourceObject + " and target: " + effect.TargetObject);
                if (effect.IsContinuous) {                   
                    if (ConditionEvaluator.Check(effect.Conditions, interaction.Context)) {
                        ApplyContinuousEffect(effect, interaction.Context, Time.deltaTime);
                    }
                }
            }            
        }
        EvaluateReactiveEffects();
    }
}