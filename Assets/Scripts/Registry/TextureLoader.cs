using System;
using UnityEngine;
using UnityEngine.Networking;

public class TextureLoader {
    public Texture2D LoadFromUrl(string url, Action<Texture2D> onLoaded) {
        var request = UnityWebRequestTexture.GetTexture(url);
        var operation = request.SendWebRequest();
        operation.completed += _ => {
            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Failed to load texture from {url}: {request.error}");
                onLoaded(null);
                return;
            }
            var texture = DownloadHandlerTexture.GetContent(request);
            onLoaded(texture);
        };
        return null; // Return null immediately, the texture will be provided via the callback when loaded
    }

}