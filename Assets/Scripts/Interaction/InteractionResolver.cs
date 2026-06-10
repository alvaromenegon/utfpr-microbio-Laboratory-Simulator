using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
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

    public List<InteractionCandidate> ResolveCandidates() {
        var baseContext = BuildContext();

        var candidates = new List<InteractionCandidate>();

        foreach (var type in GetCandidateTypes(baseContext)) {
            baseContext.Type = type;

            var effects = _effectRegistry.GetEffects(baseContext);

            foreach (var effect in effects) {
                if (!ConditionEvaluator.Check(effect.Conditions, baseContext))
                    continue;
                var candidateContext = baseContext.CloneAndCast();
                candidateContext.Type = type;

                candidates.Add(new InteractionCandidate {
                    Type = type,                    
                    Effect = effect,
                    Context = candidateContext,
                    DisplayName = effect.DisplayName,
                    Priority = effect.Priority,
                    Request = BuildRequest()
                });
            }
        }

        return candidates
            .OrderByDescending(c => c.Priority)
            .ToList();
    }

    private InteractionType ResolveInteractionType(InteractionContext baseContext) {
        foreach (var type in GetCandidateTypes(baseContext)) {
            //Debug.Log($"Evaluating candidate type: {type}");
            baseContext.Type = type;
            var effects = _effectRegistry.GetValidEffects(baseContext);
            foreach (var effect in effects) {
                //Debug.Log($"Checking effect: {effect.Type} - Conditions: [{string.Join(", ", effect.Conditions)}]");
                if (!ConditionEvaluator.Check(effect.Conditions, baseContext)) {
                    continue;
                }
                return effect.Type;
            }
        }

        throw new System.Exception($"No valid interaction type found for context: {baseContext}");
    }

    private InteractionContext BuildContext() {
        if (_hovered == null && _held == null)
            return null;

        var ctx = new InteractionContext();
        //Debug.Log($"Building context with hovered: {_hovered?.InstanceId}, held: {_held?.GetLaboratoryObject()?.InstanceId}");
        //if (_hovered != null)
        //    Debug.Log($"Hovered properties = [{_hovered.FormatProperties()}], zones=[{string.Join(", ",_hovered.CurrentZoneTags)}]");
        //if (_held != null) 
        //    Debug.Log($"Held properties = [{_held?.GetLaboratoryObject().FormatProperties()}],  zones=[{string.Join(", ", _held?.GetLaboratoryObject().CurrentZoneTags)}");

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
        else if (_hovered != null && _held == null) { // verificar se o held é nulo
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

        if (ctx.Roles.ContainsKey(RoleType.TOOL) && ctx.Roles.ContainsKey(RoleType.TARGET)) {
            //Debug.Log("Actor has both TOOL and TARGET roles, yielding USE");
            yield return InteractionType.USE;
        }
        //Debug.Log($"{actor.DisplayName} ({actor.InstanceId}) tags = [{string.Join(", ", actor.Tags)}]");

        if (actor.HasAnyTag(new List<string> { "toggleable", "openable", "coverable" })) { // todo: criar o tipo de interação "TOGGLE" e usar ele aqui
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