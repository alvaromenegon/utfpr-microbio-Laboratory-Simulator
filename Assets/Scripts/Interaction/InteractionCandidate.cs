using static Interaction;

public class InteractionCandidate {
    public InteractionType Type;
    public EffectDefinition Effect;
    public InteractionContext Context;
    public InteractionRequest Request;

    public string DisplayName;
    public int Priority;

    public override string ToString() {
            return $"InteractionCandidate(Type={Type}, Effect={Effect.Id}, Context={Context}, DisplayName={DisplayName}, Priority={Priority})";
    }
}