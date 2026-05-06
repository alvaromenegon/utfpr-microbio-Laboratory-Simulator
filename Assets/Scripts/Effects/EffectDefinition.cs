using System.Collections.Generic;
using static Interaction;

public class EffectDefinition {
    public InteractionType Type { get; set; }

    public Dictionary<RoleType, string> Roles;

    public List<When> Conditions;
    public List<EffectOperation> Operations;

    public EffectExecutionType ExecutionType;

    public bool IsContinuous => ExecutionType == EffectExecutionType.CONTINUOUS;

    public string Property;
    public float? Rate;
}

public enum EffectExecutionType {
    INSTANT,
    CONTINUOUS
}