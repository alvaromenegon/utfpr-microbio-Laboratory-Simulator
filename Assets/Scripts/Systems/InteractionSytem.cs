using Assets.Scripts;
using System;
using System.Collections.Generic;
using static Interaction;

public class InteractionSystem {
    private List<ActiveInteraction> activeInteractions = new();
    private LoggerService logger = new ("InteractionSystem");
    private EffectRegistry EffectRegistry;
    private EffectSystem effectSystem;
    public event Action<InteractionRequest> OnValueRequired;

    public InteractionSystem(EffectRegistry effectRegistry, EffectSystem effectSystem) {
        this.EffectRegistry = effectRegistry;
        this.effectSystem = effectSystem;
    }

    public void HandleInteractionRequestStart(InteractionRequest request) {
        if (request == null) {
            logger.Log("Received null interaction request.");
            return;
        }
        if (RequireValue(request.Type)) {
            logger.Log($"Interaction requires a value from user: {request}");
            OnValueRequired?.Invoke(request);
            return;
        }

        if (EffectRegistry.IsContinuous(request)) {
            logger.Log($"{request} is continuous.");
            if (IsAlreadyActive(request)) {
                logger.Log($"Interaction already active: {request}");
                return;
            }
            var interaction = new ActiveInteraction {
                Type = request.Type,
                Source = request.PrimaryObject,
                Target = request.SecondaryObject
            };
            StartActiveInteraction(interaction);
            logger.Log($"Registered new interaction: {interaction}");
        } else {
            ExecuteInstantInteraction(request);
        }
    }

    public void HandleInteractionRequestEnd(InteractionRequest request) {
        var interaction = activeInteractions.Find(i => 
            i.Type == request.Type && 
            i.Source == request.PrimaryObject && 
            i.Target == request.SecondaryObject);
        
        if (interaction != null) {
            StopActiveInteraction(interaction);
            logger.Log($"Unregistered interaction: {interaction}");
        } else {
            logger.Log($"Attempted to unregister non-existent interaction: {request}");
        }
    }

    private void StartActiveInteraction(ActiveInteraction interaction) {
        logger.Log($"Starting interaction: {interaction}");
        activeInteractions.Add(interaction);
    }

    private void StopActiveInteraction(ActiveInteraction interaction) {
        logger.Log($"Stopping interaction: {interaction}");
        activeInteractions.Remove(interaction);
    }

    private void ExecuteInstantInteraction(InteractionRequest request) {
        logger.Log($"Executing instant interaction: {request}");
        effectSystem.HandleInstantEffect(request);
    }

    private bool IsAlreadyActive(InteractionRequest request) {
        return activeInteractions.Exists(i => 
            i.Type == request.Type && 
            i.Source == request.PrimaryObject && 
            i.Target == request.SecondaryObject);
    }

    public List<ActiveInteraction> GetActiveInteractions() {
        return new List<ActiveInteraction>(activeInteractions);
    }

    public void Update() {
    }
}