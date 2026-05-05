public class EffectOperation {
    public string TargetObject;
    public string Property;
    public object Value;

    public override string ToString() {
        return $"EffectOperation(TargetObject={TargetObject}, Property={Property}, Value={Value})";
    }
}