using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUDController : MonoBehaviour{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _objectName;
    [SerializeField] private TMP_Text _userName;
    [SerializeField] private Image _userPhoto;
    [SerializeField] private TMP_Text _info;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _stepInstructions;
    [SerializeField] private TMP_Text _stepInformation;
    [SerializeField] private TMP_Text _feedback;
    [SerializeField] private TMP_Text _history; // Orientar o jogador sobre o que já foi feito

    private List<string> feedbackHistory = new List<string>();

    public static HUDController Instance;
        void Awake() => Instance = this;

        public void ShowGameOverPanel() => _gameOverPanel.SetActive(true);

        public void UpdateTimer(string timerText) => _timer.text = timerText;
        public void UpdateObjectName(string name){ _objectName.text = $"{name}"; _objectName.gameObject.SetActive(true); }

        public void UpdateUserName(string name) => _userName.text = $"{name}";

        public void UpdateInfo(string info) => _info.text = $"{info}";
        public void UpdateScore(int score) => _score.text = $"Pontuação: {score}";
        public void UpdateStepInstructions(string stepInstructions) => _stepInstructions.text = $"{stepInstructions}";
        public void UpdateStepInformation(string stepInfo) => _stepInformation.text = $"{stepInfo}";

    public void ShowFeedback(string feedback) {
        // Exibe o feedback por um curto período de tempo
        _feedback.text = feedback;
        _feedback.gameObject.SetActive(true);
        UpdateFeedbackHistory(feedback);
        Invoke(nameof(HideFeedback), 2f); // Esconde o feedback após 2 segundos
    }
    public void UpdateUserInfo ( string name, string photoURL){
        UpdateUserName(name);
        ImageLoader imageLoader = new (photoURL);
        imageLoader.AssignToImageComponent(_userPhoto);
    }

    public void UpdateDescription(string description) {
        _stepInformation.text = description;
    }

    public void HideObjectName() {
        _objectName.text = "";
        _objectName.gameObject.SetActive(false);
    }


    private void HideFeedback() {
        _feedback.gameObject.SetActive(false);
    }

    private void UpdateFeedbackHistory(string feedback) {
        // remove o feedback mais antigo se a lista já tiver 5 itens
        if (feedbackHistory.Count >= 5) {
            feedbackHistory.RemoveAt(0);
        }
        feedbackHistory.Add(feedback);
        _history.text = "Histórico:\n" + string.Join("\n", feedbackHistory);
    }
}
