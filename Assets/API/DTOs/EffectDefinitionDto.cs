using System;
using System.Collections.Generic;

[Serializable]
public class EffectDefinitionDto {
    public string Type{ get; set; }
    public Dictionary<string, string> Roles { get; set; }
    public string ExecutionType { get; set; }

    public string Property { get; set; }
    public float Rate { get; set; }
    public WhenDto[] Conditions { get; set; }
    public EffectOperationDto[] Operations { get; set; }
}
