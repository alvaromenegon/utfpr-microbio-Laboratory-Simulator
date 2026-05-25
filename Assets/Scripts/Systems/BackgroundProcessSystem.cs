using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

class BackgroundProcessInstance {
    public string ProcessId;
    public LaboratoryObject BoundObject;
    public float ElapsedTime;
    public InteractionContext Context;
    public bool IsActive;
}

public class BackgroundProcessSystem  {
    BackgroundProcessRegistry registry;
    private List<BackgroundProcessInstance> activeProcesses = new List<BackgroundProcessInstance>();
    private LoggerService logger = new LoggerService("BackgroundProcessSystem");
    private EffectSystem effectSystem;
    private EffectRegistry effectRegistry;

    public BackgroundProcessSystem(BackgroundProcessRegistry registry, EffectSystem effectSystem, EffectRegistry effectRegistry) {
        this.registry = registry;
        this.effectSystem = effectSystem;
        this.effectRegistry = effectRegistry;
    }  

    private void CompleteProcess(BackgroundProcessInstance instance, BackgroundProcess process) {
        if (ConditionEvaluator.Check(process.SuccessConditions, instance.Context)) {
            HandleEffects(process.OnSuccessEffectIds, instance.Context);
            logger.Log($"Completed background process successfully: {process.Id}");
        } else {
            HandleEffects(process.OnFailureEffectIds, instance.Context);
            logger.Log($"Completed background process with failure: {process.Id}");
        }          
    }

    private void HandleEffects(List<string> effectIds, InteractionContext context) {
        if (effectIds == null) return;
        foreach (var effectId in effectIds) {
            var effect = effectRegistry.GetActionEffectById(effectId);
            if (effect != null) {
                logger.Log($"Applying effect {effectId}");
                effectSystem.ApplyActionEffect(effect, context);
            } else {
                logger.Log($"No effect found with ID: {effectId}");
            }
        }
    }

    private bool ShouldCancelProcess(List<When> cancelConditions, InteractionContext context) {
        if (cancelConditions == null || cancelConditions.Count == 0) {
            return false;
        }
        return ConditionEvaluator.Check(cancelConditions, context);
    }

    private void UpdateActiveProcesses(float deltaTime) {
        for (int i = activeProcesses.Count - 1; i >= 0; i--) { 
            var instance = activeProcesses[i];
            if (!registry.TryGetProcess(instance.ProcessId, out var process)) {
                logger.Error($"Background process not found: {instance.ProcessId}");
                activeProcesses.RemoveAt(i);
                continue;
            }
            if (ShouldCancelProcess(process.CancelConditions, instance.Context)) {
                activeProcesses.RemoveAt(i);
                logger.Log($"Canceled background process: {instance.ProcessId}");
                continue;
            }
            instance.ElapsedTime += deltaTime;
            if (instance.ElapsedTime >= process.Duration) {
                CompleteProcess(instance, process);
                activeProcesses.RemoveAt(i);
            }
        }

    }

    private InteractionContext CreateProcessContext( BackgroundProcess definition, InteractionContext originalContext,LaboratoryObject boundObject) {
        var processContext = new InteractionContext();

        foreach (var pair in originalContext.Roles)
            processContext.Roles[pair.Key] = pair.Value;

        processContext.Roles[RoleType.PROCESS_SUBJECT] = boundObject;

        if (originalContext.Roles.TryGetValue(RoleType.TARGET, out var device))
            processContext.Roles[RoleType.PROCESS_DEVICE] = device;

        return processContext;
    }

    private bool IsAlreadyRunning(string processId, string instanceId) {
        return activeProcesses.Exists(p => p.ProcessId == processId && p.BoundObject.InstanceId == instanceId);
    }

    public bool TryStartProcessById(string processId, InteractionContext context) {
        if (!registry.TryGetProcess(processId, out var definition)) {
            logger.Log($"No background process found with ID: {processId}");
            return false;
        }
        logger.Log($"Attempting to start background process: {definition} with context: {context}");

        if (!ConditionEvaluator.Check(definition.StartConditions, context))
            return false;
        logger.Log($"Background process {processId} with BoundObject {definition.BoundObject} start conditions met.");
        var objectResolver = new ObjectResolver(definition.BoundObject, context);
        var boundObjects = objectResolver.Resolve();

        if (boundObjects.Count == 0)
            return false;

        var boundObject = boundObjects[0];

        if (IsAlreadyRunning(processId, boundObject.InstanceId))
            return false;
        Debug.Log($"Original TOOL: {context.Roles.GetValueOrDefault(RoleType.TOOL)?.InstanceId}");
        Debug.Log($"Original TARGET: {context.Roles.GetValueOrDefault(RoleType.TARGET)?.InstanceId}");
        var instance = new BackgroundProcessInstance {
            ProcessId = processId,
            BoundObject = boundObject,
            Context = CreateProcessContext(definition, context, boundObject),
            ElapsedTime = 0f,
            IsActive = true
        };
        foreach (var role in instance.Context.Roles) {
            Debug.Log($"ProcessContext role {role.Key}: {role.Value.InstanceId}");
        }

        activeProcesses.Add(instance);

        foreach (var effectId in definition.OnStartEffectIds) {
            var effect = effectRegistry.GetActionEffectById(effectId);
            effectSystem.ApplyActionEffect(effect, instance.Context);
        }

        return true;
    }

    public void Update() {
        float deltaTime = Time.deltaTime;    

        UpdateActiveProcesses(deltaTime);
    }
}