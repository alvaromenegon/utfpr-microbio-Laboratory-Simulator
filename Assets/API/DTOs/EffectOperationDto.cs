using System;

[Serializable]
public class EffectOperationDto {
    public string TargetObject { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
}
