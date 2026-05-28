public class VisualAsset {
    public string Id;

    public VisualAssetType Type;

    public string AssetPath;

    public bool HideWhenInactive;

    public MaterialTemplateType ShaderTemplate;
}

public enum VisualAssetType {
    SPRITE,
    TEXTURE,
    MATERIAL,
    PREFAB,
    UI_IMAGE
}

public enum MaterialTemplateType {
    Opaque,
    TransparentOverlay,
    Emissive,
    UnlitImage
}