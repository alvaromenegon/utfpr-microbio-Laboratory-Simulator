using System.Collections.Generic;
using static Interaction;

struct InteractionEffectKey {
    public InteractionType Type;
    public string Source;   // pode ser "*"
    public string Target;   // pode ser "*"

    public InteractionEffectKey(InteractionType type, string source, string target) {
        Type = type;
        Source = source;
        Target = target;
    }
}

public class EffectRegistry {

    private Dictionary<InteractionEffectKey, List<ContinuousEffect>> continuousEffects = new();
    private Dictionary<InteractionEffectKey, List<InstantEffect>> instantEffects = new();
    private Dictionary<string, TriggeredEffect> triggeredEffects = new(); // efeitos sem interação direta do usuário
    Dictionary<(InteractionType, string, string), List<ContinuousEffect>> cache; // (type, source, target)


    private void AddIfExists(
        List<ContinuousEffect> results,
        InteractionType type,
        string source,
        string target) {
        var key = new InteractionEffectKey(type, source, target);

        if (continuousEffects.TryGetValue(key, out var list)) {
            results.AddRange(list);
        }
    }

    private void AddIfExists(
        List<InstantEffect> results,
        InteractionType type,
        string source,
        string target) {
        var key = new InteractionEffectKey(type, source, target);

        if (instantEffects.TryGetValue(key, out var list)) {
            results.AddRange(list);
        }
    }

    public List<ContinuousEffect> GetContinuousEffects(InteractionRequest request) {
        var results = new List<ContinuousEffect>();

        AddIfExists(results, request.Type, request.PrimaryObject, request.SecondaryObject);
        AddIfExists(results, request.Type, request.PrimaryObject, "*");
        AddIfExists(results, request.Type, "*", request.SecondaryObject);
        AddIfExists(results, request.Type, "*", "*");

        return results;
    }

    public List<InstantEffect> GetInstantEffects(InteractionRequest request) {
        var results = new List<InstantEffect>();

        AddIfExists(results, request.Type, request.PrimaryObject, request.SecondaryObject);
        AddIfExists(results, request.Type, request.PrimaryObject, "*");
        AddIfExists(results, request.Type, "*", request.SecondaryObject);
        AddIfExists(results, request.Type, "*", "*");

        return results;
    }

    public TriggeredEffect GetTriggeredEffect(string effectId) {
        if (triggeredEffects.TryGetValue(effectId, out var effect)) {
            return effect;
        }
        return null;
    }

    public void SetupRegistry(ExperimentData experimentData) {
        continuousEffects.Clear();
        instantEffects.Clear();
        if (experimentData == null || experimentData.Steps == null) {
            return;
        }
        foreach (var effect in experimentData.ContinuousEffects) {
            var key = new InteractionEffectKey(effect.InteractionType, effect.SourceObject, effect.TargetObject);
            if (!continuousEffects.ContainsKey(key)) {
                continuousEffects[key] = new List<ContinuousEffect>();
            }
            continuousEffects[key].Add(effect);
        }
        foreach (var effect in experimentData.InstantEffects) {
            var key = new InteractionEffectKey(effect.InteractionType, effect.SourceObject, effect.TargetObject);
            if (!instantEffects.ContainsKey(key)) {
                instantEffects[key] = new List<InstantEffect>();
            }
            instantEffects[key].Add(effect);
        }
        foreach (var effect in experimentData.TriggeredEffects) {
            triggeredEffects[effect.Id] = effect;
        }

    }
}