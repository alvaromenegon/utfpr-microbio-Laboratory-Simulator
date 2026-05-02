using static Interaction;

public class InteractionRequest {
    public InteractionType Type;
    public string PrimaryObject;     // sempre existe
    public string SecondaryObject;   // opcional
}