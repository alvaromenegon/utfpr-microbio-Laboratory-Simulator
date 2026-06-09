using System.Collections;
using TMPro;
using UnityEngine;

public class InteractionHint : MonoBehaviour {
    public TMP_Text hintText;

    public void Show(string hint) {
        hintText.text = hint;
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}