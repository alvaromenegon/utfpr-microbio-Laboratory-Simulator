using TMPro;
using UnityEngine;

public class InputPanelController : MonoBehaviour {
    public static InputPanelController Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;

    void Awake() => Instance = this;

    public void Show() {
        inputField.text = "";
        panel.SetActive(true);
        inputField.ActivateInputField(); // foca automaticamente
    }

    public void Hide() => panel.SetActive(false);

    // Chamado pelo botão Confirmar (via OnClick no Inspector)
    public void OnConfirm() {
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