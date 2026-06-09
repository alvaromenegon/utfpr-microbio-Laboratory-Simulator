using System.Collections;
using TMPro;
using UnityEngine;

public class InputPanelController : MonoBehaviour {
    public static InputPanelController Instance;

    [SerializeField] private TMP_Text titleText;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;
    private PlayerAction pendingAction;
    private EffectSystem effectSystem;

    void Awake() => Instance = this;

    public void Show(InteractionRequest request) {
        Debug.Log($"Showing input panel for request: {request}");
        pendingAction = new PlayerAction { InteractionType = request.Type, Context = request.Context, Value = null, Property = null};
        string targetName = request.Context.Roles.ContainsKey(RoleType.TARGET) ? request.Context.Roles[RoleType.TARGET].DisplayName : "objeto";
        switch (request.Type) {
            case Interaction.InteractionType.SET_TEMPERATURE:
                titleText.text = $"Defina a Temperatura do {targetName}";
                inputField.placeholder.GetComponent<TMP_Text>().text = "Digite a temperatura em °C";
                break;
            case Interaction.InteractionType.SET_QUANTITY:
                titleText.text = $"Defina o Volume do {targetName}";
                inputField.placeholder.GetComponent<TMP_Text>().text = "Digite o volume em mL";
                break;
            case Interaction.InteractionType.SET_TIMER:
                titleText.text = $"Defina o Tempo do {targetName}";
                inputField.placeholder.GetComponent<TMP_Text>().text = "Digite o tempo em segundos";
                break;
            case Interaction.InteractionType.SET_PROPERTY:
                titleText.text = $"Defina a Propriedade do {targetName}";
                inputField.placeholder.GetComponent<TMP_Text>().text = "Digite o valor da propriedade";
                break;
            default:
                Debug.LogWarning($"Tipo de interação {request.Type} não suportado para input.");
                break;
        }
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
            pendingAction.Property = PropertyCatalog.GetInstance().Resolve(pendingAction.InteractionType, pendingAction.Context);
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