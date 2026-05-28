using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualAssetRegistry {
    
    private readonly Dictionary<string, Material> materials = new();
    private readonly Dictionary<string, Sprite> sprites = new();
    private VisualAssetRegistry() { }
    private static VisualAssetRegistry _instance;
    public static VisualAssetRegistry GetInstance() {
        _instance ??= new VisualAssetRegistry();
        return _instance;
    }

    public void RegisterSprite(string assetId, Sprite sprite) {
        Debug.Log($"Registering sprite asset: {assetId} with sprite: {sprite}");
        sprites[assetId] = sprite;
    }

    public bool TryGetSprite(string assetId, out Sprite sprite) {
        return sprites.TryGetValue(assetId, out sprite);
    }

    public void RegisterMaterial(string assetId, Material material) {
        Debug.Log($"Registering material asset: {assetId} with material: {material}");
        materials[assetId] = material;
    }

    public bool TryGetMaterial(string assetId, out Material material) {
        return materials.TryGetValue(assetId, out material);
    }

}