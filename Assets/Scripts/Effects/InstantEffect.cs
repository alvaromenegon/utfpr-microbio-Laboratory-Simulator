using System.Collections.Generic;
using static Interaction;

public class InstantEffect {
    public InteractionType InteractionType;

    public string SourceObject;
    public string TargetObject;

    public List<EffectOperation> Operations;

    public List<When> Conditions;

    public bool HasSource => !string.IsNullOrWhiteSpace(SourceObject) && SourceObject != "*";

    public override string ToString() {
        return $"InstantEffect(InteractionType={InteractionType}, SourceObject={SourceObject}, TargetObject={TargetObject}, Operations=[{string.Join(", ", Operations)}], Conditions=[{string.Join(", ", Conditions)}])";
    }
}