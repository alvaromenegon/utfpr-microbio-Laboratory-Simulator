using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DerivedPropertySystem {
    List<DerivedProperty> derivedProperties = new();
    ExperimentObjectRegistry registry;
    // todo

    public void Update() {
        foreach(var rule in derivedProperties.OrderBy(dp => dp.Priority)) {
            ApplySet(rule);
        }
    }

    /**
     * Busca o objeto com o identificador especificado na regra e aplica as propriedades definidas em setProperty.
     * 
     */
    private void ApplySet(DerivedProperty rule) {
        registry.TryGetInteractable(rule.identifier, out var obj);
        if(obj == null) {
            Debug.LogError($"DerivedPropertySystem: Could not find object with identifier {rule.identifier}");
            return;
        }
        foreach(var kvp in rule.setProperty) {
            obj.Set(kvp.Key, kvp.Value);
        }
    }
}