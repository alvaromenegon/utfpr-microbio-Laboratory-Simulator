using System.Collections.Generic;

public class ReactiveEffect {
    public string Id;

    public List<When> Conditions;

    public List<EffectOperation> Operations;

    public override string ToString() {
        return $"ReactiveEffect(Id={Id}, Conditions=[{string.Join(", ", Conditions)}], Operations=[{string.Join(", ", Operations)}])";
    }
}