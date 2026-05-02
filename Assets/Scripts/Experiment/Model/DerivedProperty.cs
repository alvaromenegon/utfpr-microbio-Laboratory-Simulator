using Assets.Scripts;
using System;
using System.Collections.Generic;

[Serializable]
public class DerivedProperty {
    public string Identifier;
    public When[] When;
    public Dictionary<string, object> SetProperty; // a propriedade do objeto que será modificada

    public int Priority { get; set; } // prioridade para resolver conflitos entre múltiplas DerivedProperties aplicáveis

}

[Serializable]
public class When {
    public string Identifier;
    public string Property; // a propriedade do objeto que será comparada, equivale a uma chave do Dictionary
    public object Value; // o valor com o qual a propriedade será comparada
    public ComparisonType Comparison; // o tipo de comparação a ser realizada (igual, diferente, maior que, etc.)
}