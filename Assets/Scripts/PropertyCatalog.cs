using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Interaction;

public class PropertyCatalog {
    private static PropertyCatalog _instance;

    public List<PropertyDefinition> Properties { get; private set; } = new List<PropertyDefinition>();
    public List<PropertyBinding> Bindings { get; private set; } = new List<PropertyBinding>();

    private PropertyCatalog() {
        LoadProperties();
    }

    private void LoadProperties() {
        string json = File.ReadAllText("property_catalog.json");
        var catalogData = JsonConvert.DeserializeObject<PropertyCatalogData>(json);

        Properties = catalogData.Properties;
        Bindings = catalogData.Bindings;
        Debug.Log($"Loaded {Properties.Count} properties and {Bindings.Count} bindings.");
    }

    private bool Matches(PropertyBinding binding, InteractionContext context) {
        var actor = context.Roles.GetValueOrDefault(RoleType.ACTOR);
        if (actor == null) return false;
        if (binding.RequiredTags != null) {
            foreach (var tag in binding.RequiredTags) {
                if (!actor.HasTag(tag))
                    return false;
            }
        }
        if (binding.AllowedIdentifiers != null) {
            if (!binding.AllowedIdentifiers.Contains(actor.Identifier))
                return false;
        }

        return true;
    }

    public string Resolve(InteractionType interaction, InteractionContext context) {
        foreach (var binding in Bindings) {
            if (binding.InteractionType != interaction) continue;
            if (!Matches(binding, context)) continue;
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