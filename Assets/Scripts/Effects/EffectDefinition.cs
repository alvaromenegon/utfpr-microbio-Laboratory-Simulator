using System.Collections.Generic;
using static Interaction;

public class EffectDefinition : IEffect{
    public InteractionType Type { get; set; }

    public Dictionary<RoleType, string> Roles;

    public EffectExecutionType ExecutionType;
    public List<Condition> Conditions;

    public bool IsContinuous => ExecutionType == EffectExecutionType.CONTINUOUS;

    public string Property;
    public float? Rate;

    public override string ToString() {
        return $"EffectDefinition('{DisplayName}',Type={Type}, Roles=[{string.Join(", ", Roles)}], ExecutionType={ExecutionType}, Property={Property}, Rate={Rate}, Operations=[{string.Join(", ", Operations)}])";
    }
}

public enum EffectExecutionType {
    INSTANT,
    CONTINUOUS
}