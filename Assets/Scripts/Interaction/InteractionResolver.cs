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

            if (_effectRegistry.HasEffect(baseContext))
                return type;
        }

        throw new System.Exception($"No valid interaction type found for context: {baseContext}");
    }

    private InteractionContext BuildContext() {
        if (_hovered == null && _held == null)
            return null;

        var ctx = new InteractionContext();

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

        if (ctx.Roles.ContainsKey(RoleType.TOOL) && ctx.Roles.ContainsKey(RoleType.TARGET))
            yield return InteractionType.USE;

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