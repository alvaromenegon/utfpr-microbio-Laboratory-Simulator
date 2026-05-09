using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

class BackgroundProcessInstance {
    public string ProcessId;

    public float ElapsedTime;

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

    private void StartProcess(BackgroundProcess process) {
        logger.Log($"Attempting to start background process: {process}");
        activeProcesses.Add(new BackgroundProcessInstance { ProcessId = process.Id, ElapsedTime = 0, IsActive = true });
        if (!string.IsNullOrEmpty(process.OnStartEffectId)) {
            logger.Log($"Applying start effect for background process: {process.Id}");
            var startEffect = effectRegistry.GetActionEffectById(process.OnStartEffectId);
            effectSystem.ApplyActionEffect(startEffect);
        } else {
            logger.Log($"No start effect defined for background process: {process.Id}");
        }
        logger.Log($"Started background process: {process.Id}");
    }

    private void CancelProcess(string processId) {
        var process = activeProcesses.Find(p => p.ProcessId == processId);
        if (process != null) {
            activeProcesses.Remove(process);
            logger.Log($"Canceled background process: {processId}");
        }
    }

    private void CompleteProcess(BackgroundProcessInstance instance, BackgroundProcess process) {
        activeProcesses.Remove(instance);
        if (ConditionEvaluator.Check(process.SuccessConditions)) {
            var effect = effectRegistry.GetActionEffectById(process.OnSuccessEffectId);
            effectSystem.ApplyActionEffect(effect);
            logger.Log($"Completed background process successfully: {process.Id}");
        } else {
            var effect = effectRegistry.GetActionEffectById(process.OnFailureEffectId);
            effectSystem.ApplyActionEffect(effect);
            logger.Log($"Completed background process with failure: {process.Id}");
        }

    }


    private bool ShouldStartProcess(BackgroundProcess process) {
        return ConditionEvaluator.Check(process.StartConditions);
    }

    private bool ShouldCancelProcess(BackgroundProcess process) {
        if (process.CancelConditions == null || process.CancelConditions.Count == 0) {
            return false;
        }
        return ConditionEvaluator.Check(process.CancelConditions);
    }

    private void UpdateActiveProcesses(float deltaTime) {
        foreach (var instance in activeProcesses) {
            instance.ElapsedTime += deltaTime;
            registry.TryGetProcess(instance.ProcessId, out var process);
            if (ShouldCancelProcess(process)) {
                CancelProcess(instance.ProcessId);
                continue;
            }
            if (instance.ElapsedTime >= process.Duration) {
                CompleteProcess(instance, process);
            }
        }

    }

    public void Update() {
        float deltaTime = Time.deltaTime;
        foreach (var processData in registry.GetAllProcesses()) {
            if (ShouldStartProcess(processData) && !activeProcesses.Exists(p => p.ProcessId == processData.Id)) {
                StartProcess(processData);
            }
        }

        UpdateActiveProcesses(deltaTime);
    }
}