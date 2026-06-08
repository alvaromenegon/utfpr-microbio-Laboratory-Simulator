using System.Linq;
using TMPro;
using UnityEngine;

public class ScaleDisplayController : MonoBehaviour {
    public EnvironmentZone weighingZone;
    public TMP_Text displayText;

    void Update() {
        float totalWeight = weighingZone
            .GetObjectsInside()
            .Sum(obj => obj.TryGetProperty<float>("weight", out var weight) ? weight : 0);

        displayText.text = $"{totalWeight/100:0.00}g";
    }
}