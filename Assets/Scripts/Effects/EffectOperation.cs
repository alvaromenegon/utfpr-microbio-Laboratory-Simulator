public class EffectOperation {
    public EffectOperationType OperationType;
    public ObjectReference Target;
    public string Property;
    public object Value;
    public string ProcessId;
    private string _displayMessageTemplate;

    public void SetDisplayMessageTemplate(string message) {
        _displayMessageTemplate = message;
    }

    private object FormatValueForDisplay(object value) {
        if (value is float f) {
            return f.ToString("0.##");
        }
        return value.ToString();
    }

    // permite exibir uma mensagem mais amigável para o usuário, se houver um template definido, ou não exibir nada caso o template seja "{null}"
    public string GetDisplayMessageForTarget(string target,object value, bool isContinuous = false) {        
        if (string.IsNullOrEmpty(_displayMessageTemplate)) {
            var propertyCatalog = PropertyCatalog.GetInstance();
            // buscamos detalhes da propriedade para criar uma mensagem mais informativa, caso o catálogo esteja disponível
            if (propertyCatalog.TryGetPropertyDefinition(Property, out PropertyDefinition definition)) {
                return $"{(isContinuous ? "Aplicando" : "Aplicado")} {FormatValueForDisplay(value)} {definition.Unit} na propriedade {definition.Label} de {target}";
                // ex: temperature -> "Aplicado 37 °C na propriedade Temperatura de Incubadora"
            }
            return $"{ (isContinuous ? "Aplicando" : "Aplicado")} {FormatValueForDisplay(value)} na propriedade {Property} de {target}";
            // ex: temperature -> "Aplicado 37 na propriedade temperature de Incubadora"
        }
        if (_displayMessageTemplate.Equals("{null}")) { // não deve exibir mensagem para esta operação
            return "";
        }
        return _displayMessageTemplate.Replace("{value}", FormatValueForDisplay(value).ToString()).Replace("{property}", Property).Replace("{target}", target);
    }

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