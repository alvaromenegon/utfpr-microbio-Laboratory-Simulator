using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour{
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private TMP_Text _objectName;
    [SerializeField] private TMP_Text _userName;
    [SerializeField] private Image _userPhoto;
    [SerializeField] private TMP_Text _info;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _stepInformation;

    public static HUDController Instance;
     void Awake() => Instance = this;

        public void ShowGameOverPanel() => _gameOverPanel.SetActive(true);

        public void UpdateTimer(string timerText) => _timer.text = timerText;
        public void UpdateObjectName(string name) => _objectName.text = $"{name}";

        public void UpdateUserName(string name) => _userName.text = $"{name}";

        public void UpdateInfo(string info) => _info.text = $"{info}";
    public void UpdateScore(string score) => _score.text = $"Pontuação: {score}";
    public void UpdateDescription(string description) => _description.text = $"{description}";
        public void UpdateStepInformation(string stepInfo) => _stepInformation.text = $"{stepInfo}";

    public void UpdateUserInfo ( string name, string photoURL){
        UpdateUserName(name);
        ImageLoader imageLoader = new (photoURL);
        imageLoader.AssignToImageComponent(_userPhoto);

    }


}
