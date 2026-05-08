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