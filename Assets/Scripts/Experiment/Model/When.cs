using Assets.Scripts;
using System;

public enum QueryMatchMode {
    ANY,
    ALL,
    NONE,
    EXACT
}

[Serializable]
public class When {
    public ObjectReference Target; // o objeto alvo da condição, pode ser um objeto específico ou um tipo de objeto (ex: "any door")
    public QueryMatchMode MatchMode;
    public string Property; // a propriedade do objeto que será comparada, equivale a uma chave do Dictionary
    public object Value; // o valor com o qual a propriedade será comparada
    public ComparisonType Comparison; // o tipo de comparação a ser realizada (igual, diferente, maior que, etc.)

    public override string ToString() {
        return $"When(Target: {Target}, Property: {Property}, Value: {Value}, Comparison: {Comparison})";
    }
}