using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _objectName;
    [SerializeField] private TMP_Text _userName;
    [SerializeField] private TMP_Text _info;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _stepInstructions;
    [SerializeField] private TMP_Text _stepInformation;
    [SerializeField] private TMP_Text _feedback;

    public static HUDController Instance;
        void Awake() => Instance = this;

        public void ShowGameOverPanel() => _gameOverPanel.SetActive(true);

        public void UpdateTimer(string timerText) => _timer.text = timerText;
        public void UpdateObjectName(string name) => _objectName.text = $"{name}";

        public void UpdateUserName(string name) => _userName.text = $"{name}";

        public void UpdateInfo(string info) => _info.text = $"{info}";
        public void UpdateScore(string score) => _score.text = $"Pontuação: {score}";
        public void UpdateStepInstructions(string stepInstructions) => _stepInstructions.text = $"{stepInstructions}";
        public void UpdateStepInformation(string stepInfo) => _stepInformation.text = $"{stepInfo}";

    public void ShowFeedback(string feedback) {
        // Exibe o feedback por um curto período de tempo
        _feedback.text = feedback;
        _feedback.gameObject.SetActive(true);
        Invoke(nameof(HideFeedback), 2f); // Esconde o feedback após 2 segundos
    }

    private void HideFeedback() {
        _feedback.gameObject.SetActive(false);
    }
}
