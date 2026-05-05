using System;
using System.Collections.Generic;

[Serializable]
public class TriggeredEffectDto {
    public string Id { get; set; }
    public WhenDto[] Conditions { get; set; }
    public EffectOperationDto[] Operations { get; set; }
}
