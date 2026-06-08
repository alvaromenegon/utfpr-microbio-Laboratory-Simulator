using System.Collections.Generic;

public class ReactiveEffect : IEffect {
    public List<Condition> Conditions;

    public ObjectReference Subject;
    public bool ExecuteOncePerSubject = true;

    public override string ToString() {
        return $"ReactiveEffect('{DisplayName}', Id={Id}, Subject={Subject}, Conditions=[{string.Join(", ", Conditions)}], Operations=[{string.Join(", ", Operations)}])";
    }
}