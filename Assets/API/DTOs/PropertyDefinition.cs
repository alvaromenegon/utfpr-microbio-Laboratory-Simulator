public class PropertyDefinitionDto {
    public string Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public object Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public PropertyDataType DataType;
    public object DefaultValue { get; set; }
}

public class PropertyBindingDto {
    public string InteractionType { get; set; }
    public string[] RequiredTags { get; set; } = new string[0];
    public string[] AllowedIdentifiers { get; set; } = new string[0];
    public string PropertyId { get; set; } = string.Empty;
    public RoleType? RequiredRole { get; set; }
}

public class PropertyCatalogDataDto {
    public PropertyDefinitionDto[] Properties { get; set; } = new PropertyDefinitionDto[0];
    public PropertyBindingDto[] Bindings { get; set; } = new PropertyBindingDto[0];
}