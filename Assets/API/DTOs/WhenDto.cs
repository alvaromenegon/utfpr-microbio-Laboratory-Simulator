using System;

[Serializable]
public class WhenDto {
    public string Identifier { get; set; }
    public string InstanceId { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
    public string Comparison { get; set; }
}
