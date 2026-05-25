public class EffectOperation {
    public EffectOperationType OperationType;
    public ObjectReference Target;
    public string Property;
    public object Value;
    public string ProcessId;
    public override string ToString() {
        return $"EffectOperation(Target={Target}, Property={Property}, Value={Value}, ProcessId={ProcessId})";
    }
}

public enum EffectOperationType {
    SET_PROPERTY,
    ADD_PROPERTY,
    START_PROCESS,
    STOP_PROCESS
}