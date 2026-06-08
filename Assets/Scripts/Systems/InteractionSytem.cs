using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Interaction;

public class InteractionSystem {
    private List<ActiveInteraction> activeInteractions = new();
    private LoggerService logger = new ("InteractionSystem");
    private EffectRegistry EffectRegistry;
    private EffectSystem effectSystem;
    public event Action<InteractionRequest> OnValueRequired;

    public InteractionSystem(EffectRegistry effectRegistry, EffectSystem effectSystem) {
        EffectRegistry = effectRegistry;
        this.effectSystem = effectSystem;
    }

    public void HandleInteractionRequestStart(InteractionCandidate selectedInteraction) {
        var request = selectedInteraction?.Request;
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
            var interaction = new ActiveInteraction(request.Context);
            StartActiveInteraction(interaction);
        } else {
            ExecuteInstantInteraction(selectedInteraction);
        }
    }

    public void HandleInteractionRequestEnd(InteractionRequest request) {
        var interaction = activeInteractions.Find(i => i.Context == request.Context);
        
        if (interaction != null) {
            StopActiveInteraction(interaction);
        }
    }

    private void StartActiveInteraction(ActiveInteraction interaction) {
        logger.Log($"Starting interaction: {interaction}");
        interaction.Effects = EffectRegistry.GetEffects(interaction.Context);
        logger.Log($"Found {interaction.Effects.Count} effects for interaction.");
        activeInteractions.Add(interaction);
    }

    private void StopActiveInteraction(ActiveInteraction interaction) {
        logger.Log($"Stopping interaction: {interaction}");
        activeInteractions.Remove(interaction);
    }

    private void ExecuteInstantInteraction(InteractionCandidate selectedInteraction) {
        Debug.Log($"Executing instant interaction: {selectedInteraction}");
        effectSystem.HandleCandidate(selectedInteraction);
    }

    private bool IsAlreadyActive(InteractionRequest request) {
        return activeInteractions.Exists(i => i.Context == request.Context);
    }

    public List<ActiveInteraction> GetActiveInteractions() {
        return new List<ActiveInteraction>(activeInteractions);
    }

    public void Update() {
    }
}