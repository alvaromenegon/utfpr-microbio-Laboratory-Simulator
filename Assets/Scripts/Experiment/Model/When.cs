using Assets.Scripts;
using System;

[Serializable]
public class When {
    public string Identifier;
    public string InstanceId;
        return $"When(Identifier: {Identifier}, InstanceId: {InstanceId}, Property: {Property}, Value: {Value}, Comparison: {Comparison})";
    public object Value; // o valor com o qual a propriedade será comparada
    public ComparisonType Comparison; // o tipo de comparação a ser realizada (igual, diferente, maior que, etc.)

    public override string ToString() {
        return $"When(Identifier: {Identifier}, Property: {Property}, Value: {Value}, Comparison: {Comparison})";
    }
}