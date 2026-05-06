
using UnityEngine;

class InteractionClassifier {
    EffectRegistry effectRegistry;
    public bool IsContinuous(InteractionRequest request) {
        if (effectRegistry == null) {
            Debug.LogError("EffectRegistry is not set in InteractionClassifier");
            throw new System.Exception("EffectRegistry is not set in InteractionClassifier");
        }
            return effectRegistry.GetContinuousEffects(request).Count > 0;
    }
}