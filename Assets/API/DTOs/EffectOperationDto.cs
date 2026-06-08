using System;

[Serializable]
public class EffectOperationDto {
    public string OperationType { get; set; }
    public ObjectReferenceDto Target { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
    public string ProcessId { get; set; } = string.Empty;
    public string DisplayMessageTemplate { get; set; } = string.Empty;
}
