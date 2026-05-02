using System.Collections.Generic;

public class TriggeredEffect {
    public string Id;

    public List<When> Conditions;

    public List<EffectOperation> Operations;
}