using Assets.Scripts.Registry;
using UnityEngine;

public class DerivedPropertySystem {
    DerivedPropertyRegistry propertyRegistry;
    ExperimentObjectRegistry registry;

    public DerivedPropertySystem(DerivedPropertyRegistry propertyRegistry) {
        this.propertyRegistry = propertyRegistry;
        registry = ExperimentObjectRegistry.Instance;
    }

    public void Update() {
        foreach (var rule in propertyRegistry.GetPropertiesByPriority()) {
            ApplySet(rule);
        }
    }

    /**
     * Busca o objeto com o identificador especificado na regra e aplica as propriedades definidas em setProperty.
     * 
     */
    private void ApplySet(DerivedProperty rule) {
        registry.TryGetObject(rule.Identifier, out var obj);
        if(obj == null) {
            Debug.LogError($"DerivedPropertySystem: Could not find object with identifier {rule.Identifier}");
            return;
        }
        foreach(var kvp in rule.SetProperty) {
            obj.SetProperty(kvp.Key, kvp.Value);
        }
    }
}