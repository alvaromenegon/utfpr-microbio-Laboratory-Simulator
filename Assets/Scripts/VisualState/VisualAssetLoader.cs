using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualAssetLoader {
    private readonly VisualAssetRegistry registry;
    private readonly TextureLoader textureLoader;
    private readonly MaterialFactory materialFactory;
    private readonly LoggerService logger = new LoggerService("VisualAssetLoader");
    public bool IsReady { get; private set; }

    public VisualAssetLoader(VisualAssetRegistry registry, TextureLoader textureLoader, MaterialFactory materialFactory) {
        this.registry = registry;
        this.textureLoader = textureLoader;
        this.materialFactory = materialFactory;
    }


    public IEnumerator LoadAssets(List<VisualAsset> assets) {
        IsReady = false;
        foreach (var asset in assets) {
            if (asset.Type == VisualAssetType.MATERIAL) {
                yield return LoadMaterial(asset);
            } else if (asset.Type == VisualAssetType.SPRITE) {
                yield return LoadSprite(asset);
            }
        }
        IsReady = true;
    }

    private IEnumerator LoadMaterial(VisualAsset asset) {
        logger.Log($"Loading material asset: {asset.Id}");
        Material material = null;
     
        Texture2D texture = null;

        yield return textureLoader.LoadFromUrl(asset.AssetPath, t => texture = t);
        if (texture == null) {
            logger.Error($"Failed to load texture for material asset: {asset.Id}");
            yield break;
        }

        material = materialFactory.Create(asset.ShaderTemplate.ToString(), texture);        

        registry.RegisterMaterial(asset.Id, material);
    }

    private IEnumerator LoadSprite(VisualAsset asset) {
        logger.Log($"Loading sprite asset: {asset.Id}");
        Sprite sprite = null;
     
        Texture2D texture = null;
        yield return textureLoader.LoadFromUrl(asset.AssetPath, t => texture = t);
        if (texture == null) {
            logger.Error($"Failed to load texture for sprite asset: {asset.Id}");
            yield break;
        }
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        registry.RegisterSprite(asset.Id, sprite);
    }
}