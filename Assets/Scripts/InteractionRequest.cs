using static Interaction;

public class InteractionRequest {
    public InteractionContext Context;
    public InteractionType Type;

    public static InteractionRequest FromCandidate(InteractionCandidate candidate) {
        return new InteractionRequest {
            Context = candidate.Context,
            Type = candidate.Type
        };
    }

    public override string ToString() {
            return $"InteractionRequest(Type={Type}, Context={Context})";
    }
}