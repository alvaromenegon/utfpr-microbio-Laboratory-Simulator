using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TextureLoader {
    public IEnumerator LoadFromUrl(string url, Action<Texture2D> onComplete) {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url)) {
            yield return uwr.SendWebRequest();
            if (uwr.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"Failed to load texture from {url}: {uwr.error}");
                onComplete(null);
            } else {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                onComplete(texture);
            }
        }
    }

    public IEnumerator LoadSprite(string url, Action<Sprite> onComplete) {
        yield return LoadFromUrl(url, texture => {
            if (texture != null) {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                onComplete(sprite);
            } else {
                onComplete(null);
            }
        });
    }

}