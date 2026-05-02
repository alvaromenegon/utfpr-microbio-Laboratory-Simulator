using Assets.Scripts;

public class PlayerAction {
    public Interaction.InteractionType InteractionType { get; set; }
    public string Target { get; set; }
    public double? Value { get; set; } = default;
    public string Property { get; set; } = default;

    public PlayerAction() { }

    public bool IsComplete() {
        // todos os valores foram preenchidos
        return (Value != null && Property != null && Target != null );
    }
}