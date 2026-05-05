using System;
using System.Collections.Generic;

[Serializable]
public class ContinuousEffectDto {
    public string InteractionType { get; set; }
    public string SourceObject { get; set; }
    public string TargetObject { get; set; }
    public string Property { get; set; }
    public float Rate { get; set; }
    public WhenDto[] Conditions { get; set; }
}
