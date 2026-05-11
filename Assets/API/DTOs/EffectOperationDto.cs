using System;

[Serializable]
public class EffectOperationDto {
    public string TargetObject { get; set; }
    public string TargetInstanceId { get; set; }
    public string TargetRole { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
}
