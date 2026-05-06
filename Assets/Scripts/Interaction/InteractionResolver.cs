using Assets.Scripts;
using UnityEngine;
using static Interaction;

class InteractionResolver {
    public LaboratoryObject PrimaryObject { get; private set; }
    private EffectRegistry EffectRegistry;

    public IMovable HeldObject { get; private set; }

    public InteractionResolver(LaboratoryObject primary, IMovable held) {
        PrimaryObject = primary;
        HeldObject = held;
        EffectRegistry = ExperimentManager.Instance.GetEffectRegistry();
    }

    // interação com apenas 1 objeto, sem nada na mão
    private InteractionRequest BuildSimpleRequest() {
        return new InteractionRequest {
            Type = ResolveInteractionType(PrimaryObject.GetIdentifier()),
            PrimaryObject = PrimaryObject.GetIdentifier(),
            SecondaryObject = null
        };
    }

    // interação com 2 objetos, um na mão e outro alvo
    private InteractionRequest BuildComplexRequest() {
        var heldLabObject = HeldObject.GetLaboratoryObject();
        if (heldLabObject == null)
            return null;
        // primary é o objeto que faz algo
        // secondary é o objeto que recebe a ação
        return new InteractionRequest {
            Type = ResolveInteractionType(PrimaryObject.GetIdentifier(), heldLabObject.GetIdentifier()),
            PrimaryObject = PrimaryObject.GetIdentifier() ,
            SecondaryObject = heldLabObject.GetIdentifier(),
        };
    }
    

    public InteractionRequest BuildRequest() {
       if (HeldObject == null && PrimaryObject != null)
            return BuildSimpleRequest();
       if (HeldObject != null && PrimaryObject != null)
            return BuildComplexRequest();
       return null;
    }

    public InteractionType ResolveInteractionType(string interacting, string held = null) {
        Debug.Log($"Resolving interaction type for object {interacting} {(held != null ? $"while holding {held}" : "")}");
        foreach (var type in System.Enum.GetValues(typeof(InteractionType))) {
            if (EffectRegistry.HasEffect((InteractionType)type, held ?? "*", interacting)) {
                Debug.Log($"Found interaction type {(InteractionType)type} for object {interacting} {(held != null ? $"while holding {held}" : "")}");
                return (InteractionType)type;
            }
        }
        // todo: melhorar a exceção
        throw new System.Exception($"No interaction type found for object {interacting} {(held != null ? $"while holding {held}" : "")}"); 
    }
}