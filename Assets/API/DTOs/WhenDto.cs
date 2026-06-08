using System;

[Serializable]
public class WhenDto {
    public ObjectReferenceDto Target { get; set; }
    public string MatchMode { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
    public string Comparison { get; set; }
}
