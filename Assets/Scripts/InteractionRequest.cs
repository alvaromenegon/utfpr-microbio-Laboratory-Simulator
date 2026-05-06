using static Interaction;

public class InteractionRequest {
    public InteractionContext Context;
    public InteractionType Type;

        public override string ToString() {
            return $"InteractionRequest(Type={Type}, Context={Context})";
    }
}