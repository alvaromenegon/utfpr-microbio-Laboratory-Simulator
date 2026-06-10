using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Interaction;

public class PropertyCatalog {
    private static PropertyCatalog _instance;

    public List<PropertyDefinition> Properties { get; private set; } = new List<PropertyDefinition>();
    public List<PropertyBinding> Bindings { get; private set; } = new List<PropertyBinding>();

    private PropertyCatalog() { }

    
    public void SetCatalogData(PropertyCatalogData catalogData) {
        Properties = catalogData.Properties;
        Bindings = catalogData.Bindings;
        Debug.Log($"Loaded {Properties.Count} properties and {Bindings.Count} bindings.");
        Debug.Log($"Properties: {JsonConvert.SerializeObject(Properties, Formatting.Indented)}");
        Debug.Log($"Bindings: {JsonConvert.SerializeObject(Bindings, Formatting.Indented)}");
    }

    private bool Matches(PropertyBinding binding, InteractionContext context) {
        var actor = context.Roles.GetValueOrDefault(RoleType.ACTOR);
        Debug.Log($"Matching binding for actor: {actor} with context: {context}");
        if (actor == null) return false;
        Debug.Log($"Checking required tags {JsonConvert.SerializeObject(binding.RequiredTags, Formatting.Indented)}");
        if (binding.RequiredTags != null) {
            foreach (var tag in binding.RequiredTags) {
                Debug.Log($"Checking if {actor.InstanceId} has tag '{tag}' among its tags [{string.Join(", ", actor.Tags)}]");
                if (!actor.HasTag(tag)) {
                    Debug.Log($"Actor {actor.InstanceId} does not have required tag: {tag}");
                    return false;
                }
            }
        }
        if (binding.AllowedIdentifiers != null && binding.AllowedIdentifiers.Count > 0) {
            Debug.Log($"Checking allowed identifiers {JsonConvert.SerializeObject(binding.AllowedIdentifiers, Formatting.Indented)}");
            if (!binding.AllowedIdentifiers.Contains(actor.Identifier)) {
                Debug.Log($"Actor {actor.InstanceId} is not in allowed identifiers.");
                return false;
            }
        }

        return true;
    }

    public string Resolve(InteractionType interaction, InteractionContext context) {
        Debug.Log($"Resolving property for interaction {interaction} with context: {context}");
        foreach (var binding in Bindings) {
            Debug.Log($"Checking binding: InteractionType={binding.InteractionType}, PropertyId={binding.PropertyId}");
            if (binding.InteractionType != interaction) continue;
            if (!Matches(binding, context)) continue;
            Debug.Log($"Resolved property: {binding.PropertyId}");
            return binding.PropertyId;
        }
        return null; // ou uma propriedade padrão
    }

    public static PropertyCatalog GetInstance() {
        _instance ??= new PropertyCatalog();
        return _instance;
    }

    public bool TryGetPropertyDefinition(string propertyId, out PropertyDefinition definition) {
        definition = Properties.Find(p => p.Id == propertyId);
        return definition != null;
    }

}

public class PropertyCatalogData {
    public List<PropertyDefinition> Properties;
    public List<PropertyBinding> Bindings;
}

public enum PropertyDataType {
    BOOLEAN,
    INTEGER,
    FLOAT,
    STRING
}

public class PropertyDefinition {
    public string Id { get; set; }
    public string Label { get; set; }
    public object Description { get; set; }
    public string Unit { get; set; }
    public PropertyDataType DataType;
    public object DefaultValue { get; set; }

}

public class PropertyBinding {
    public InteractionType InteractionType { get; set; }
    public string PropertyId { get; set; }
    public List<string> RequiredTags { get; set; }
    public List<string> AllowedIdentifiers { get; set; }
    public RoleType? RequiredRole { get; set; }
}