using Assets.Scripts;
using System;
using System.Collections.Generic;
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

    public override readonly string ToString() => $"(type={Type}, source={Source}, target={Target})";
}

public class EffectRegistry {
    private LoggerService logger = new LoggerService("EffectRegistry");
    private Dictionary<string, ReactiveEffect> reactiveEffects = new(); // efeitos sem interação direta do usuário que reagem a mudanças no estado do experimento
    private Dictionary<string, ActionEffect> actionEffects = new(); // efeitos acionados por outros sistemas, como o BackgroundProcessSystem
    private List<EffectDefinition> _effects = new();
    

    public List<ReactiveEffect> GetAllReactiveEffects() { // retorna todos os efeitos acionados, sem filtro
        return new List<ReactiveEffect>(reactiveEffects.Values);
    }

    public ReactiveEffect GetTriggeredEffect(string effectId) {
        if (reactiveEffects.TryGetValue(effectId, out var effect)) {
            return effect;
        }
        return null;
    }

    public ActionEffect GetActionEffectById(string effectId) {
        if (actionEffects.TryGetValue(effectId, out var effect)) {
            return effect;
        }
        return null;
    }

    public bool IsContinuous(InteractionRequest request) {
        var ctx = request.Context;
        if (ctx == null) {
            logger.Log("InteractionContext is null in IsContinuous check.");
            return false;
        }
        if (HasEffect(ctx)) {
            var effects = GetEffects(ctx);
            return effects.Exists(e => e.IsContinuous);
        }
        return false;
    }


    public bool HasEffect(InteractionContext ctx) {
        foreach (var effect in _effects) {
            if (effect.Type != ctx.Type)
                continue;

            if (MatchesRoles(effect, ctx))
                return true;
        }

        return false;
    }

    // recebe uma lista de efeitos e retorna o de maior prioridade (ou null se a lista estiver vazia)
    public EffectDefinition Prioritize(List<EffectDefinition> effects) {
        if (effects == null || effects.Count == 0)
            return null;
        EffectDefinition highest = effects[0];
        foreach (var effect in effects) {
            if (effect.Priority > highest.Priority) {
                highest = effect;
            }
        }
        return highest;
    }

    // retorna uma lista de efeitos válidos para o contexto
    public List<EffectDefinition> GetValidEffects(InteractionContext ctx) {
        var validEffects = new List<EffectDefinition>();
        foreach (var effect in _effects) {
            if (effect.Type != ctx.Type)
                continue;
            if (MatchesRoles(effect, ctx)) {
                validEffects.Add(effect);
            }
        }
        return validEffects;

    }

    private bool MatchesIdentifierOrTag(LaboratoryObject obj, string value) {
        if (value == "*")
            return true;

        // Try InstanceId first (for multi-instance support)
        if (!string.IsNullOrWhiteSpace(obj.InstanceId) && obj.InstanceId == value)
            return true;

        if (obj.Identifier == value) // útil para objetos únicos na cena, como uma incubadora única
            return true;

        return obj.Tags != null && obj.Tags.Contains(value);
    }

    private bool MatchesRoles(EffectDefinition effect, InteractionContext ctx) {
        foreach (var roleRequirement in effect.Roles) {
            if (!ctx.Roles.TryGetValue(roleRequirement.Key, out var obj))
                return false;

            if (!MatchesIdentifierOrTag(obj, roleRequirement.Value))
                return false;
        }

        return true;
    }

    private void AddRoleMatches(List<EffectDefinition> results, InteractionContext ctx) {
        foreach (var effect in _effects) {
            if (effect.Type != ctx.Type)
                continue;

            if (MatchesRoles(effect, ctx)) {
                results.Add(effect);
            }
        }
    }

    public List<EffectDefinition> GetEffects(InteractionContext ctx) {
        var results = new List<EffectDefinition>();

        AddRoleMatches(results, ctx);

        return results;
    }

    public void Register(EffectDefinition effect) {
        _effects.Add(effect);
    }

    public void SetupRegistry(ExperimentData experimentData) {
        
        if (experimentData == null || experimentData.Steps == null) {
            return;
        }
        foreach (var effect in experimentData.Effects) {
            Register(effect);
        }

        foreach (var effect in experimentData.ReactiveEffects) {
            reactiveEffects[effect.Id] = effect;
        }
        foreach (var effect in experimentData.ActionEffects) {
            actionEffects[effect.Id] = effect;
        }
        logger.Log($"Instant Effects: {string.Join(", ", _effects.FindAll(e=>!e.IsContinuous))}");
        logger.Log($"Continuous Effects: {string.Join(", ", _effects.FindAll(e=>e.IsContinuous))}");
        logger.Log($"Reactive Effects: {string.Join("|| ", reactiveEffects.Values)}");
        logger.Log($"Action Effects: {string.Join("|| ", actionEffects.Values)}");

    }
}