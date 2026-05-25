public class VisualAsset {
    public string Id;

    public VisualAssetType Type;

    public string AssetPath;

    public string Slot;

    public bool HideWhenInactive;
}

public enum VisualAssetType {
    SPRITE,
    TEXTURE,
    MATERIAL,
    PREFAB,
    UI_IMAGE
}