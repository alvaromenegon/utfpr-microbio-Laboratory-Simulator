using static Interaction;

public class InteractionRequest {
    public InteractionType Type;
    public string PrimaryObject;     // sempre existe
    public string SecondaryObject;   // opcional

        public override string ToString() {
            return $"InteractionRequest(Type={Type}, PrimaryObject={PrimaryObject}, SecondaryObject={SecondaryObject})";
    }
}