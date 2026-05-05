using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Interaction;

struct InteractionEffectKey : IEquatable<InteractionEffectKey> {
    public InteractionType Type;
    public string Source;   // pode ser "*"
    public string Target;   // pode ser "*"

    public InteractionEffectKey(InteractionType type, string source, string target) {
        Type = type;
        Source = source ?? "*";
        Target = target ?? "*";
    }

    public bool Equals(InteractionEffectKey other) {
        return Type == other.Type &&
              Source == other.Source &&
              Target == other.Target;

    }
    public override int GetHashCode() {
        return HashCode.Combine(Type, Source, Target);
    }

    public override string ToString() => $"(type={Type}, source={Source}, target={Target})";
}

public class EffectRegistry {
    private LoggerService logger = new LoggerService("EffectRegistry");
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

    public List<ContinuousEffect> GetContinuousEffects(InteractionType type, string source, string target) {
        var results = new List<ContinuousEffect>();
        AddIfExists(results, type, source, target);
        AddIfExists(results, type, source, "*");
        AddIfExists(results, type, "*", target);
        AddIfExists(results, type, "*", "*");
        return results;
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

    public List<InstantEffect> GetInstantEffects(InteractionType type, string source, string target) {
        var results = new List<InstantEffect>();
        AddIfExists(results, type, source, target);
        AddIfExists(results, type, source, "*");
        AddIfExists(results, type, "*", target);
        AddIfExists(results, type, "*", "*");
        return results;
    }

    public List<TriggeredEffect> GetAllTriggeredEffects() { // retorna todos os efeitos acionados, sem filtro
        return new List<TriggeredEffect>(triggeredEffects.Values);
    }

    public TriggeredEffect GetTriggeredEffect(string effectId) {
        if (triggeredEffects.TryGetValue(effectId, out var effect)) {
            return effect;
        }
        return null;
    }

    public bool IsContinuous(InteractionRequest request) {
        logger.Log($"Checking for continuous effects with request: {request}");
        return GetContinuousEffects(request).Count > 0;
    }

    // Verifica se existe algum efeito (contínuo ou instantâneo), baseado no tipo de interação e nos objetos envolvidos
    public bool HasEffect(InteractionType type, string source, string target) {
        Debug.Log($"Checking for effects with type={type}, source={source}, target={target}");
        return GetContinuousEffects(type, source, target).Count > 0 ||
               GetInstantEffects(type, source, target).Count > 0;
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
        logger.Log($"EffectRegistry setup with {continuousEffects.Count} continuous effects, {instantEffects.Count} instant effects and {triggeredEffects.Count} triggered effects.");
        logger.Log($"Continuous Effects: {string.Join(", ", continuousEffects.Keys)}");
        logger.Log($"Instant Effects: {string.Join(", ", instantEffects.Keys)}");
        logger.Log($"Triggered Effects: {string.Join(", ", triggeredEffects.Keys)}");

    }
}