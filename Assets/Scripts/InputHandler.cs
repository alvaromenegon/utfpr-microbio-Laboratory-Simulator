using UnityEngine;
using UnityEngine.UI; // Required for the legacy UI InputField
using TMPro; // Required for TextMeshPro InputField

public class InputHandler : MonoBehaviour {
    // For legacy UI:
    public InputField legacyInputField;

    // For TextMeshPro UI:
    public TMP_InputField tmpInputField;

    public void ReadInput() {
        // Get the text from the legacy input field
        string legacyText = legacyInputField.text;
        Debug.Log("Legacy Input: " + legacyText);

        // Get the text from the TextMeshPro input field
        string tmpText = tmpInputField.text;
        Debug.Log("TMP Input: " + tmpText);
    }
}
