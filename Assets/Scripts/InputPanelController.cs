using System.Collections;
using TMPro;
using UnityEngine;

public class InputPanelController : MonoBehaviour {
    public static InputPanelController Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;
    private PlayerAction pendingAction;

    void Awake() => Instance = this;

    public void Show(PlayerAction action) {
        pendingAction = action;
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

        if (pendingAction == null || pendingAction.Target == null) {
            Debug.LogError("No pending action available for input confirmation.");
            Hide();
            return;
        }

        PlayerAction action = new() {
            InteractionType = pendingAction.InteractionType,
            Target = pendingAction.Target,
            Value = value
        };
        ExperimentManager.Instance.HandlePlayerAction(action);
        pendingAction = null;
    }
}