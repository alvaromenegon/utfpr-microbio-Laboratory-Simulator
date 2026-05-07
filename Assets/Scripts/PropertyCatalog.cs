using System.Collections.Generic;
using static Interaction;

public class PropertyCatalog {
    public static PropertyCatalog Instance { get; private set; }

    public List<PropertyDefinition> Properties { get; private set; } = new List<PropertyDefinition>();

    private PropertyCatalog() {
        LoadProperties();
    }

    private void LoadProperties() {
        // todo: implementar lógica para carregar propriedades de um arquivo de configuração
    }

    public string Resolve(InteractionType interaction, InteractionContext context) {
        throw new System.NotImplementedException("Método Resolve(InteractionType, InteractionContext) ainda não implementado.");
    }

    public string Resolve(InteractionType interaction, string identifier) {
        foreach (var property in Properties) {
            if (property.AppliesTo.Contains(interaction.ToString()) && property.Id == identifier) {
                return property.MapsTo.ContainsKey(interaction.ToString()) ? property.MapsTo[interaction.ToString()] : property.Id;
            }
        }
        return null; // ou lançar uma exceção, dependendo do caso
    }

    public static void Initialize() {
        if (Instance == null) {
            Instance = new PropertyCatalog();
        }
    }
}

public class PropertyDefinition {
    public string Id { get; set; }
    public string Label { get; set; }
    public object Description { get; set; }
    public List<string> AppliesTo{ get; set; }
    public string DataType { get; set; }
    public Dictionary<string,string> MapsTo { get; set; } = new Dictionary<string,string>();
}