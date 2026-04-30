using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem {
    public List<ActiveInteraction> activeInteractions = new();
    private ExperimentObjectRegistry registry;
    private LoggerService logger = new ("EffectSystem");

    private List<ContinuousEffect> GetMatchingEffects(ActiveInteraction interaction) {
        // todo: implementar lógica para encontrar efeitos correspondentes 
        throw new System.NotImplementedException();

    }

    private bool CheckConditions(List<When> conditions) {
        // todo: implementar lógica para verificar condições
        throw new System.NotImplementedException();
    }

    private void ApplyEffect(ContinuousEffect effect, float deltatime) {
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

        public void RegisterActiveInteraction(ActiveInteraction interaction) {
            activeInteractions.Add(interaction);
        }
    
        public void UnregisterActiveInteraction(ActiveInteraction interaction) {
            activeInteractions.Remove(interaction);
        }
    
        public IEnumerable<ActiveInteraction> GetActiveInteractions() {
            return activeInteractions;
    }

    public void Update() {
        foreach (var interaction in activeInteractions) {
            var effects = GetMatchingEffects(interaction);

            foreach (var effect in effects) {
                if (CheckConditions(effect.Conditions)) {
                    ApplyEffect(effect, Time.deltaTime);
                }
            }
        }
    }
}