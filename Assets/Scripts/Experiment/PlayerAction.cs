using Assets.Scripts;

public class PlayerAction {
    public Interaction.InteractionType InteractionType { get; set; }
    public InteractionContext Context;
    public object Value { get; set; } = default;
    public string Property { get; set; } = default;

    public PlayerAction() { }

}