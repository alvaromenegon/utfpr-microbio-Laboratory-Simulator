using System.Collections;
using TMPro;
using UnityEngine;

public class InputPanelController : MonoBehaviour {
    public static InputPanelController Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;

    void Awake() => Instance = this;

    public void Show() {
        panel.SetActive(true);
        inputField.text = string.Empty;
        StartCoroutine(ActivateInputNextFrame());
    }

    private IEnumerator ActivateInputNextFrame() {
        // Aguarda o fim do frame para garantir que o panel esteja ativo e renderizado
        yield return new WaitForEndOfFrame();

        inputField.Select();
        inputField.ActivateInputField();
    }

    public void Hide() => panel.SetActive(false);

    // Chamado pelo botão Confirmar (via OnClick no Inspector)
    public void OnConfirm() {
        Debug.Log($"Input value: {inputField.text}");
        if (double.TryParse(inputField.text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, // evita bug de vírgula/ponto
                out double value)) {
            ExperimentManager.Instance.ValidateValue(value);
        }
        else {
            Debug.Log("Valor inválido.");
            // Opcional: mostrar feedback de erro na UI
        }

        Hide();
    }
}