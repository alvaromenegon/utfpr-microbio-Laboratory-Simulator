using System.Collections;
using TMPro;
using UnityEngine;
using static Interaction;

public class InputPanelController : MonoBehaviour {
    public static InputPanelController Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;
    private PlayerAction pendingAction;
    private EffectSystem effectSystem;

    void Awake() => Instance = this;

    public void Show(InteractionRequest request) {
        Debug.Log($"Showing input panel for request: {request}");
        pendingAction = new PlayerAction { InteractionType = request.Type, Context = request.Context, Value = null, Property = null};
        panel.SetActive(true);
        inputField.text = string.Empty;
        StartCoroutine(ActivateInputNextFrame());
    }

    public void Bind(EffectSystem effectSystem) {
        this.effectSystem = effectSystem;
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
            pendingAction.Property = PropertyCatalog.Instance.Resolve(pendingAction.InteractionType, pendingAction.Context);
            Debug.Log($"Resolved property: {pendingAction.Property}");
            pendingAction.Value = value;
            effectSystem.ApplyValueAction(pendingAction);
            Hide();
        }
        else {
            Debug.Log("Valor inválido.");
            // todo: mostrar feedback de erro na UI
        }
    }
}