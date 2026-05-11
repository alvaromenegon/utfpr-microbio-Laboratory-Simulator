public class EffectOperation {
    public string TargetObject;
    public string TargetInstanceId;
    public RoleType? TargetRole;
    public string Property;
    public object Value;

    public override string ToString() {
        return $"EffectOperation(TargetObject={TargetObject}, TargetInstanceId={TargetInstanceId}, TargetRole={TargetRole}, Property={Property}, Value={Value})";
    }
}