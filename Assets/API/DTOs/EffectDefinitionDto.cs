using System;
using System.Collections.Generic;

public abstract class IEffectDto {
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public int Priority { get; set; } = 1;
}

[Serializable]
public class EffectDefinitionDto : IEffectDto {
    public string Type{ get; set; }
    public Dictionary<string, string> Roles { get; set; }
    public string ExecutionType { get; set; }

    public string Property { get; set; }
    public float Rate { get; set; }
    public WhenDto[] Conditions { get; set; }
    public EffectOperationDto[] Operations { get; set; }
}
