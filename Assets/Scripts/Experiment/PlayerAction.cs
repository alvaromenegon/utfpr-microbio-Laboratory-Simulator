using UnityEngine;

public class PlayerAction {
    public Interaction.InteractionType InteractionType { get; set; }
    public GameObject Target { get; set; }
    public double? Value { get; set; } = default;
}