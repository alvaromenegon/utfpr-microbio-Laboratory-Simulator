using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoader {
    string _url;
    Sprite _sprite;
    public ImageLoader(string url) {
        _url = url;
    }
    public async void LoadImage() {
        using var request = UnityWebRequestTexture.GetTexture(_url);
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success) {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            _sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }

    public async void AssignToImageComponent(Image imageComponent) {
        if (_sprite == null) {
            await Task.Run(() => LoadImage());
        }
        imageComponent.sprite = _sprite;
    }

}