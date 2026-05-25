using System.Collections.Generic;

public class ReactiveEffect {
    public string Id;

    public List<When> Conditions;

    public List<EffectOperation> Operations;
    public ObjectReference Subject;
    public bool ExecuteOncePerSubject = true;

    public override string ToString() {
        return $"ReactiveEffect(Id={Id}, Subject={Subject}, Conditions=[{string.Join(", ", Conditions)}], Operations=[{string.Join(", ", Operations)}])";
    }
}