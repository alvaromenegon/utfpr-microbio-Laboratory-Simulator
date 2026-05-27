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

    public void RegisterMaterial(string assetId, Material material) {
        Debug.Log($"Registering material asset: {assetId} with material: {material}");
        materials[assetId] = material;
    }

    public bool TryGetMaterial(string assetId, out Material material) {
        return materials.TryGetValue(assetId, out material);
    }

    public Material GetMaterial(string assetId) {
        if (!materials.TryGetValue(assetId, out var material))
            throw new Exception($"Material visual não carregado: {assetId}");

        return material;
    }

    public void SetupRegistry(List<VisualAsset> assetDefinitions) {
        foreach (var def in assetDefinitions) {
            if (def.Type == VisualAssetType.MATERIAL) {
                var material = new MaterialFactory().Create(def.ShaderTemplate.ToString(), null);
                RegisterMaterial(def.Id, material);
            }
            
        }
    }
}