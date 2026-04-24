using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static Interaction;



public class ExperimentManager : MonoBehaviour {
    [SerializeField] private GameObject label;
    [SerializeField] private GameObject secondaryTargetBeam;
    [SerializeField] private TMP_Text inputLabel;
    public static ExperimentManager Instance { get; private set; }
    public ExperimentDefinition CurrentExperiment { get; private set; }
	private int currentStepIndex = 0;
    private Step CurrentStep => CurrentExperiment.experimentData.Steps[currentStepIndex];
    private int score = 0;
    private DateTime StartTime { get; set; }

    private BackgroundProcess[] backgroundProcesses = new BackgroundProcess[0];

    public void AddBackgroundWork(BackgroundProcess work) {
        Array.Resize(ref backgroundProcesses, backgroundProcesses.Length + 1);
        backgroundProcesses[^1] = work;
    }

    public bool IsExperimentRunning { get; private set; } = false;

    public void StartExperiment(ExperimentDefinition experimentDefinition) {
		CurrentExperiment = experimentDefinition;
		currentStepIndex = 0;
		IsExperimentRunning = true;
        score = 0;
        SetupExperiment();
        StartTime = DateTime.Now;
    }

    private void SetupStep() {
        var step = CurrentStep;
        var gameObject = GameObject.Find(step.InteractionTarget);
        if (gameObject == null) {
            Debug.LogError($"Target '{step.InteractionTarget}' not found in the scene.");
            return;
        }
        var interactable = gameObject.GetComponent<StepInteractable>();
        interactable.stepType = step.Interaction;
        if (step.Interaction == InteractionType.MOVE && !string.IsNullOrEmpty(step.InteractionTargetSecondary)) {
            var gameObjectSecondary = GameObject.Find(step.InteractionTargetSecondary);
            if (gameObjectSecondary == null) {
                Debug.LogError($"Secondary target '{step.InteractionTargetSecondary}' not found in the scene.");
                return;
            }
            secondaryTargetBeam.transform.position = gameObjectSecondary.transform.position;
            secondaryTargetBeam.SetActive(true);
        }
        if (step.Interaction == InteractionType.USE && !string.IsNullOrEmpty(step.OnSuccessStartBackgroundWorkId)) {

            // verifica se é um EquipmentInteractable
            var equipment = interactable as EquipmentInteractable;
            if (equipment != null) {
                var _bgWork = Array.Find(CurrentExperiment.experimentData.BackgroundProcesses, w => w.Id == step.OnSuccessStartBackgroundWorkId);
                AddBackgroundWork(_bgWork);
                equipment.backgroundProcess = _bgWork;
                Debug.Log($"Background work '{_bgWork.Description}' will be started when using '{gameObject.name}'.");
            }
        }
        HUDController.Instance.UpdateStepInformation($"Step {currentStepIndex + 1}/{CurrentExperiment.experimentData.Steps.Length}: {step.Description} - {step.Instruction}");
    }

    private void SetupExperiment() {
        if (CurrentExperiment == null) {
            Debug.LogError("No experiment definition set.");
            return;
        }
        var stepsDescription = "";
        foreach (var step in CurrentExperiment.experimentData.Steps) {
            stepsDescription += $"- {step.Description}\n";
        }
        label.GetComponent<Text>().text = $"{CurrentExperiment.experimentData.Description}\n{stepsDescription}";

        // verifica se a cena têm os objetos necessários para o experimento
        // com o componente StepInteractable para processar as interações
        // se não tiver, adiciona o componente e exibe um aviso
        foreach (var objName in CurrentExperiment.experimentData.Objects) {
            GameObject obj = GameObject.Find(objName);
            if (obj == null) {
                Debug.LogError($"Object '{objName}' not found in the scene.");
                continue;
            }
            var interactable = obj.GetComponent<StepInteractable>();
            if (interactable == null) {
                Debug.LogWarning($"Object '{objName}' does not have a StepInteractable component. Adding one.");
                obj.AddComponent<StepInteractable>();
            }
        }
        SetupStep();
    }

    public void ValidateValue(double? value) {
        double? expectedValue = CurrentStep.Value;
        double tolerance = CurrentStep.Tolerance ?? 0.001;
        if (expectedValue == null) {
            Debug.LogError($" requires a value, but no expected value is defined for the current step.");
            InvalidStep();
            return;
        }
        if (Math.Abs(value.Value - expectedValue.Value) > tolerance) {
            Debug.LogError($"requires a value of {expectedValue}, but received {value}.");
            InvalidStep();
            return;
        }
        ValidStep();
    }

    private bool ValidateInteraction(PlayerAction interaction, double? value = null) {
        if (interaction.InteractionType != CurrentStep.Interaction)
            return false;

        if (interaction.Target.name != CurrentStep.InteractionTarget)
            return false;

        return true;
    }

    private void ValidStep() {
        if (CurrentStep == null) {
            Debug.LogError("Current step is null.");
            return;
        }
        ShowFeedback(CurrentStep.OnSuccessMessage);
        score += CurrentStep.Score;
        if (!string.IsNullOrEmpty(CurrentStep.OnSuccessStartBackgroundWorkId)) {
            var work = Array.Find(CurrentExperiment.experimentData.BackgroundProcesses, w => w.Id == CurrentStep.OnSuccessStartBackgroundWorkId);
            if (work == null) {
                Debug.LogError($"Background work with ID '{CurrentStep.OnSuccessStartBackgroundWorkId}' not found in the experiment definition.");
            } else {
                AddBackgroundWork(work);
                work.Start();
            }
        }
        NextStep();
    }

    public void OnSecondaryTargetInteraction(GameObject gameObjectPrimary) {
        if (CurrentStep == null) {
            Debug.LogError("Current step is null.");
            return;
        }
        var gameObjectSecondary = GameObject.Find(CurrentStep.InteractionTargetSecondary);
        if (gameObjectSecondary == null) {
            Debug.LogError($"Secondary target '{CurrentStep.InteractionTargetSecondary}' not found in the scene.");
            return;
        }
        var equipment = gameObjectSecondary.GetComponent<EquipmentInteractable>();
        if (equipment == null) {
            // não é um equipamento, então não precisa mover para "dentro" dele, deixa o usuário soltar e processa a operação normalmente
            return;
        }
        if (CurrentStep.Interaction != InteractionType.MOVE) {
            Debug.LogError($"Secondary target interaction is only valid for MOVE interactions. Current step interaction: {CurrentStep.Interaction}");
            return;
        }
        equipment.MoveObjectToEquipment(gameObjectPrimary);
    }

    private bool HasBackgroundWorkRunning() {
        foreach (var work in backgroundProcesses) {
            if (work.IsRunning) {
                return true;
            }
        }
        return false;
    }

    private void ValidBackgroundWork(string workId) {
        var work = Array.Find(backgroundProcesses, w => w.Id == workId);
        if (work == null) {
            Debug.LogError($"Background work with ID '{workId}' not found in the experiment definition.");
            return;
        }
        ShowFeedback(work.OnSuccessMessage);
        score += work.Score;
        if (currentStepIndex >= CurrentExperiment.experimentData.Steps.Length) {
            FinishExperiment();
        }
    }
    private void InvalidBackgroundWork(string workId) { 
        var work = Array.Find(backgroundProcesses, w => w.Id == workId);
        if (work == null) {
            Debug.LogError($"Background work with ID '{workId}' not found in the experiment definition.");
            return;
        }
        ShowFeedback(work.OnFailureMessage);
        switch (work.ActionOnFailure) {
            case OnStepFinished.BACK_TO_STEP:
                ReturnToStep(work.OnFailureReturnToStep);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_FAILURE:
                FinishExperiment(0);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_STEP:
                FinishExperiment();
                break;
            default:
                break;
        }

    }
    private void InvalidStep() {
        if (CurrentStep == null) {
            Debug.LogError("Current step is null.");
            return;
        }
        ShowFeedback(CurrentStep.OnFailureMessage);
        OnFailure();
    }

    public void ProcessInteraction(PlayerAction interaction, double? value = null) {
		if (!IsExperimentRunning) return;
        bool valid = ValidateInteraction(interaction, value);

        if (valid) {
            ValidStep();
        }else {
            InvalidStep();
        }
    }

    public void ProcessBackgroundWork(BackgroundProcess work, float timeUsed) {
        if (!IsExperimentRunning) return;
        var expectedTime = work.Duration;
        var tolerance = work.Tolerance ?? 0.5f;
        if (Math.Abs(timeUsed - expectedTime) > tolerance) {
            Debug.LogError($"O Processo deveria ter sido executado por {expectedTime} (+-{tolerance}) segundos, mas foi executado por {timeUsed} segundos.");
            InvalidBackgroundWork(work.Id);
            return;
        }   
        if (!work.ValidateCompletion(currentStepIndex)) {
            Debug.LogError($"O Processo não foi completado no momento esperado. Verifique se ele deveria ter sido completado entre os passos {string.Join(", ", work.ShouldCompleteBetweenSteps)}.");
            InvalidBackgroundWork(work.Id);
            return;
        }
        
        
        ValidBackgroundWork(work.Id);
    }

    public void NextStep() {
        secondaryTargetBeam.SetActive(false);
        inputLabel.text = "";
        if (CurrentStep.ActionOnSuccess == OnStepFinished.COMPLETE_EXPERIMENT_SUCCESS) {
            FinishExperiment();
			return;
        }
		currentStepIndex++;
		if (currentStepIndex >= CurrentExperiment.experimentData.Steps.Length) {
            FinishExperiment();
			return;
        }
        SetupStep();
		ShowStepInstruction();
    }

    public void ReturnToStep(int stepIndex) {
        if (stepIndex < 0 || stepIndex >= CurrentExperiment.experimentData.Steps.Length) {
            Debug.LogError("Invalid step index");
            return;
        }
        currentStepIndex = stepIndex;
        ShowStepInstruction();
    }

    private void RestartExperiment() {
        currentStepIndex = 0;
        score = 0;
        StartTime = DateTime.Now;
        ShowStepInstruction();
    }

    private void OnFailure() {
        var currentStep = CurrentStep;
        switch (currentStep.ActionOnFailure) {            
            case OnStepFinished.BACK_TO_STEP:
                ReturnToStep(currentStep.OnFailureReturnToStep);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_FAILURE:
                FinishExperiment(0);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_STEP:
                FinishExperiment();
                break;
            default:
                break;
        }
        ShowFeedback(currentStep.OnFailureMessage);
    }

    private void ShowStepInstruction() {
        Debug.Log($"Step {currentStepIndex + 1}: {CurrentStep.Instruction}");
    }

	private void ShowFeedback(string message) {
		Debug.Log(message);
        // todo: implement UI feedback
    }

    private void FinishExperiment(int? finishScore = null) {
        if (HasBackgroundWorkRunning()) {
            Debug.LogWarning("There are still background works running");

            // TODO: implementar a lógica para lidar com os processos em segundo plano que ainda estão em execução
        }
        else {
            IsExperimentRunning = false;

            if (finishScore != null) {
                score = finishScore.Value;
            }

            End();
        }
        // TODO: exibir resultados, enviar dados para servidor, etc.
    }

    private void Update() {
        if (IsExperimentRunning) {
            HUDController.Instance.UpdateTimer($"Tempo: {(DateTime.Now - StartTime).TotalSeconds:F0}/{CurrentExperiment.experimentData.Time} s");
        }
            if (IsExperimentRunning && CurrentExperiment.experimentData.HasTimeLimit()) {
            double elapsedSeconds = (DateTime.Now - StartTime).TotalSeconds;
            if (elapsedSeconds > CurrentExperiment.experimentData.Time) {
                Debug.Log("Time's up!");
                FinishExperiment();
            }
        }
    }

    private void Awake() {
        Instance = this;
        if (!SessionData.Initialize()) {
            // Exibir tela de erro e interromper o fluxo
            //HUDController.ErrorScreen.SetActive(true);
            return;
        }

        Debug.Log($"Sessão iniciada → Practice: {SessionData.Current.PracticeId} " +
                  $"| User: {SessionData.Current.UserId} " +
                  $"| Course: {SessionData.Current.CourseId}");

        StartCoroutine(FetchAndStartExperiment(SessionData.Current.PracticeId.ToString()));
    }

    //private void OnGUI() {
    //    GUI.Label(new Rect(10, 50, 200, 20), $"Time: {(DateTime.Now - StartTime).TotalSeconds:F0}/{CurrentExperiment.experimentData.Time} seconds");
    //    GUI.Label(new Rect(10, 70, 400, 20), $"Current Step: {(IsExperimentRunning ? CurrentStep.Description : "None")}");
    //    GUI.Label(new Rect(10, 90, 400, 20), $"Instructions: {(IsExperimentRunning ? CurrentStep.Instruction : "No experiment running")}");
    //    GUI.Label(new Rect(10, 110, 400, 20), $"Score: {score}/100");
    //}    

    private IEnumerator FetchAndStartExperiment(string id) {
        Debug.Log($"Fetching experiment data for practice ID: {id}");
        string UNITY_AUTH_TOKEN = "your_unity_auth_token"; //todo: pegar do environment ou de um arquivo de configuração seguro
        string url = $"http://localhost:8000/unity/initialize-session?practiceId={id}&userId={SessionData.Current.UserId}&courseId={SessionData.Current.CourseId}";
        using UnityWebRequest webRequest = UnityWebRequest.Get(url);
        webRequest.SetRequestHeader("x-unity-token", UNITY_AUTH_TOKEN);
        Debug.Log("Sending request to API... (" + url + ")");
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError) {
            Debug.LogError($"Error loading experiment from API: {webRequest.error}");
        }
        else {
            try {
                string json = webRequest.downloadHandler.text;
                Debug.Log($"Experiment data received: {json}");
                var data = JsonConvert.DeserializeObject<APIResult<PracticeDataDTO>>(json);
                if (data.Ok() && data.Data != null) {
                    Debug.Log("Experiment data parsed successfully.");
                } else {
                    Debug.LogError("API returned an error or empty data.");
                    yield break;
                }

                ExperimentDefinition experiment = ScriptableObject.CreateInstance<ExperimentDefinition>();
                HUDController.Instance.UpdateUserInfo(data.Data.Username, data.Data.UserPhotoURL);
                SessionData.Current.Username = data.Data.Username;
                SessionData.Current.UserPhotoURL = data.Data.UserPhotoURL;
                experiment = data.Data.Practice;

                StartExperiment(experiment);
            }
            catch (Exception e) {
                Debug.LogError($"Failed to parse experiment data: {e.Message}");
            }
        }
    }

    // Metódo que envia os dados para o servidor
    private IEnumerator SendDataToServer() {
        Debug.Log("Sending data to server...");
        string UNITY_AUTH_TOKEN = "your_unity_auth_token"; //todo: pegar do environment ou de um arquivo de configuração seguro
        string url = "http://localhost:8000/unity/results";
        var resultData = new {
            practiceId = SessionData.Current.PracticeId,
            userId = SessionData.Current.UserId,
            courseId = SessionData.Current.CourseId,
            score,
            timeTaken = (DateTime.Now - StartTime).TotalSeconds
        };
        string jsonData = JsonConvert.SerializeObject(resultData);
        using UnityWebRequest webRequest = new UnityWebRequest(url, "POST");
        webRequest.SetRequestHeader("x-unity-token", UNITY_AUTH_TOKEN);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "application/json");
        yield return webRequest.SendWebRequest();

    }

    private void End() {
        IsExperimentRunning = false;
        CurrentExperiment = null;
        Debug.Log("Experiment ended. Final score: " + score);
        StartCoroutine(SendDataToServer());
    }
}
