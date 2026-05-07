public class EffectOperation {
    public string TargetObject;
    public RoleType? TargetRole;
    public string Property;
    public object Value;

    public override string ToString() {
        return $"EffectOperation(TargetObject={TargetObject}, TargetRole={TargetRole}, Property={Property}, Value={Value})";
    }
}