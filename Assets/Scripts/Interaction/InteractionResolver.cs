using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using static Interaction;

class InteractionResolver {
    private readonly LaboratoryObject _hovered;
    private readonly IMovable _held;
    private EffectRegistry _effectRegistry;

    public InteractionResolver(LaboratoryObject hovered,IMovable held) {
        _hovered = hovered;
        _held = held;
        _effectRegistry = ExperimentManager.Instance.GetEffectRegistry();
    }

    private InteractionType ResolveInteractionType(InteractionContext baseContext) {
        foreach (var type in GetCandidateTypes(baseContext)) {
            baseContext.Type = type;
            Debug.Log($"Trying interaction type: {type}");
            var effects = _effectRegistry.GetValidEffects(baseContext);
            foreach (var effect in effects) {
                Debug.Log($"Checking effect: {effect.Type} - Conditions: [{string.Join(", ", effect.Conditions)}]");
                if (!ConditionEvaluator.Check(effect.Conditions, baseContext)) {
                    Debug.Log($"Effect {effect.Type} conditions ({string.Join(", ", effect.Conditions)}) not met");
                    continue;
                }
                Debug.Log($"Effect {effect.Type} is valid");
                return effect.Type;
            }
        }

        throw new System.Exception($"No valid interaction type found for context: {baseContext}");
    }

    private InteractionContext BuildContext() {
        if (_hovered == null && _held == null)
            return null;

        var ctx = new InteractionContext();
        Debug.Log($"Building context with hovered: {_hovered?.Identifier}, held: {_held?.GetLaboratoryObject()?.Identifier}");
        Debug.Log($"Hovered properties = [{string.Join(", ", _hovered?.GetAllProperties() ?? new Dictionary<string, object>())}]");
        Debug.Log($"Held properties = [{string.Join(", ", _held?.GetLaboratoryObject()?.GetAllProperties() ?? new Dictionary<string, object>())}]");

        var heldObj = _held?.GetLaboratoryObject();

        //  CASO 1: tool + target
        if (heldObj != null && _hovered != null) {
            ctx.Roles[RoleType.TOOL] = heldObj;
            ctx.Roles[RoleType.TARGET] = _hovered;
            ctx.Roles[RoleType.ACTOR] = heldObj;

            // heurística útil
            if (_hovered.HasTag("device"))
                ctx.Roles[RoleType.DEVICE] = _hovered;
        }
        //  CASO 2: interação simples
        else if (_hovered != null) {
            ctx.Roles[RoleType.TARGET] = _hovered;
            ctx.Roles[RoleType.ACTOR] = _hovered;

            if (_hovered.HasTag("device"))
                ctx.Roles[RoleType.DEVICE] = _hovered;
        }
        //  CASO 3: só objeto na mão (opcional)
        else if (heldObj != null) {
            ctx.Roles[RoleType.TOOL] = heldObj;
            ctx.Roles[RoleType.ACTOR] = heldObj;
        }

        return ctx;
    }

    public InteractionRequest BuildRequest() {
        var context = BuildContext();
        if (context == null)
            return null;
        try {
            var type = ResolveInteractionType(context);
            context.Type = type;
            return new InteractionRequest {
                Context = context,
                Type = type
            };
        }
        catch (System.Exception ex) {
            Debug.LogWarning($"Failed to resolve interaction: {ex.Message}");
            return null;
        }
    }

    private IEnumerable<InteractionType> GetCandidateTypes(InteractionContext ctx) {
        var actor = ctx.Roles.GetValueOrDefault(RoleType.ACTOR);
        Debug.Log($"Resolving interaction for actor: {actor?.Identifier}, roles: [{string.Join(", ", ctx.Roles)}]");
        Debug.Log($"Contains TOOL: {ctx.Roles.ContainsKey(RoleType.TOOL)}");
        Debug.Log($"Contains TARGET: {ctx.Roles.ContainsKey(RoleType.TARGET)}");

        if (ctx.Roles.ContainsKey(RoleType.TOOL) && ctx.Roles.ContainsKey(RoleType.TARGET)) {
            Debug.Log("Actor has both TOOL and TARGET roles, yielding USE");
            yield return InteractionType.USE;
        }
        Debug.Log($"{actor.Identifier} ({actor.InstanceId}) tags = [{string.Join(", ", actor.Tags)}]");

        if (actor.HasTag("toggleable")) {
            yield return InteractionType.ACTIVATE;
            yield return InteractionType.DEACTIVATE;
        }

        if (actor.HasTag("temperature_control"))
            yield return InteractionType.SET_TEMPERATURE;

        if (actor.HasTag("timed"))
            yield return InteractionType.SET_TIMER;

        if (actor.HasTag("measurable"))
            yield return InteractionType.SET_QUANTITY;

        yield return InteractionType.EXAMINE;
    }
}