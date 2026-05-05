using System;
using System.Collections.Generic;

[Serializable]
public class DerivedPropertyDto {
    public string Identifier { get; set; }
    public WhenDto[] When { get; set; }
    public Dictionary<string, object> SetProperty { get; set; }
    public int Priority { get; set; }
}
