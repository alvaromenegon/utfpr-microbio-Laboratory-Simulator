using System;
using System.Collections.Generic;

[Serializable]
public class InstantEffectDto {
    public string InteractionType { get; set; }
    public string SourceObject { get; set; }
    public string TargetObject { get; set; }
    public List<EffectOperationDto> Operations { get; set; }
    public WhenDto[] Conditions { get; set; }
}
