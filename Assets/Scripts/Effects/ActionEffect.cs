using System.Collections.Generic;

public class ActionEffect {
    public string Id;
    public List<EffectOperation> Operations;

    public override string ToString() {
        return $"ActionEffect(Id={Id}, Operations=[{string.Join(", ", Operations)}])";
    }
}