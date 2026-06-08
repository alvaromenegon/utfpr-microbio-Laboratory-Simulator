using System.Collections.Generic;

public abstract class IEffect {
    public string Id;
    public string DisplayName;
    public int Priority = 1;
    public List<EffectOperation> Operations;
}