using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAssetLoader {
    private readonly VisualAssetRegistry registry;
    private readonly TextureLoader textureLoader;
    private readonly MaterialFactory materialFactory;

    public VisualAssetLoader(VisualAssetRegistry registry, TextureLoader textureLoader, MaterialFactory materialFactory) {
        this.registry = registry;
        this.textureLoader = textureLoader;
        this.materialFactory = materialFactory;
    }


    public IEnumerator LoadAssets(List<VisualAsset> assets) {
        foreach (var asset in assets) {
            if (asset.Type == VisualAssetType.MATERIAL) {
                yield return LoadMaterial(asset);
            }            
        }
    }

    private IEnumerator LoadMaterial(VisualAsset asset) {
        Material material = null;
     
        Texture2D texture = null;

        yield return textureLoader.LoadFromUrl(asset.AssetPath, t => texture = t);

        material = materialFactory.Create(asset.ShaderTemplate.ToString(), texture);        

        registry.RegisterMaterial(asset.Id, material);
    }
}