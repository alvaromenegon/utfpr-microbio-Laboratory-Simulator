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

    public override string ToString() => $"(type={Type}, source={Source}, target={Target})";
}

public class EffectRegistry {
    private LoggerService logger = new LoggerService("EffectRegistry");
    private Dictionary<string, TriggeredEffect> triggeredEffects = new(); // efeitos sem interação direta do usuário
    private List<EffectDefinition> _effects = new();
    

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

        if (obj.Identifier == value)
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

        foreach (var effect in experimentData.TriggeredEffects) {
            triggeredEffects[effect.Id] = effect;
        }
        logger.Log($"Effects: {string.Join(", ", _effects)}");
        logger.Log($"Triggered Effects: {string.Join(", ", triggeredEffects.Keys)}");

    }
}