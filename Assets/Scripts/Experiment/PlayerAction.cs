using Assets.Scripts;

public class PlayerAction {
    public Interaction.InteractionType InteractionType { get; set; }
    public LaboratoryObject Target { get; set; }
    public double? Value { get; set; } = default;
    public string Property { get; set; } = default;
}