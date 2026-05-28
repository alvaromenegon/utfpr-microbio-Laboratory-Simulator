public class VisualAssetDto {
    public string Id { get; set; }
    public VisualAssetType Type { get; set; }
    public string AssetPath { get; set; }
    public bool HideWhenInactive { get; set; }
    public MaterialTemplateType ShaderTemplate { get; set; }
}