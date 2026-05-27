using UnityEngine;

public class MaterialFactory {
    public Material Create(string shaderTemplate, Texture2D texture) {
        Shader shader = Shader.Find(shaderTemplate);
        if (shader == null) {
            Debug.LogError($"Shader '{shaderTemplate}' não encontrado. Usando shader padrão.");           
            shader = Shader.Find("Standard");
        }
        Material material = new(shader) {
            mainTexture = texture
        };
        return material;
    }
}