using Assets.Scripts;
using UnityEngine;

public class MaterialFactory {
    private readonly LoggerService logger = new ("MaterialFactory");

    public Material Create(string shaderTemplate, Texture2D texture) {
        logger.Log($"Creating material with shader '{shaderTemplate}' and texture '{texture.name}'");
        Shader shader = Shader.Find(shaderTemplate);
        if (shader == null) {
            logger.Warning($"Shader '{shaderTemplate}' não encontrado. Usando shader padrão.");           
            shader = Shader.Find("Standard");
        }
        Material material = new(shader) {
            mainTexture = texture
        };
        return material;
    }
}