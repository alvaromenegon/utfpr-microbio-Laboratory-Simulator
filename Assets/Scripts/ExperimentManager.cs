using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ExperimentManager : MonoBehaviour {
    [SerializeField] private GameObject label;
    public static ExperimentManager Instance { get; private set; }
    public ExperimentDefinition CurrentExperiment { get; private set; }
	private int currentStepIndex = 0;
    private Step CurrentStep => CurrentExperiment.experimentData.Steps[currentStepIndex];
    private int score = 0;
    private DateTime StartTime { get; set; }


    public bool IsExperimentRunning { get; private set; } = false;

    public void StartExperiment(ExperimentDefinition experimentDefinition) {
		CurrentExperiment = experimentDefinition;
		currentStepIndex = 0;
		IsExperimentRunning = true;
        score = 0;
        SetupExperiment();
        StartTime = DateTime.Now;
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
            Debug.Log($"Found object '{objName}' for the experiment. Checking Interactable script");
            var interactable = obj.GetComponent<StepInteractable>();
            if (interactable == null) {
                Debug.LogWarning($"Object '{objName}' does not have a StepInteractable component. Adding one.");
                obj.AddComponent<StepInteractable>();
            }
            interactable.stepType = CurrentExperiment.experimentData.Steps[0].Interaction; // definir o tipo de interação esperado para o primeiro passo
        }
    }

    private bool ValidateInteraction(PlayerAction interaction) {
        if (interaction.InteractionType != CurrentStep.Interaction)
            return false;

        if (interaction.Target.name != CurrentStep.InteractionTarget)
            return false;

        return true;
    }

    public void ProcessInteraction(PlayerAction interaction) {
		if (!IsExperimentRunning) return;
		var currentStep = CurrentStep;
        bool valid = ValidateInteraction(interaction);

        if (valid) {
            ShowFeedback(currentStep.OnSuccessMessage);
            score += currentStep.Score;
            NextStep();
        }else {
            ShowFeedback(currentStep.OnFailureMessage);
            OnFailure();
        }

    }

	public void NextStep() {
		if (CurrentStep.ActionOnSuccess == OnStepFinished.COMPLETE_EXPERIMENT_SUCCESS) {
            FinishExperiment();
			return;
        }
		currentStepIndex++;
		if (currentStepIndex >= CurrentExperiment.experimentData.Steps.Length) {
            FinishExperiment();
			return;
        }
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
        IsExperimentRunning = false;
        if (finishScore != null) {
            score = finishScore.Value;
        }

        Debug.Log("Experiment completed!");
        // TODO: exibir resultados, enviar dados para servidor, etc.
    }

    private void Update() {
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
    }

    private void OnGUI() {
        GUI.Label(new Rect(10, 50, 200, 20), $"Time: {(DateTime.Now - StartTime).TotalSeconds:F0}/{CurrentExperiment.experimentData.Time} seconds");
        GUI.Label(new Rect(10, 70, 400, 20), $"Current Step: {(IsExperimentRunning ? CurrentStep.Description : "None")}");
        GUI.Label(new Rect(10, 90, 400, 20), $"Score: {score}/100");
    }

    private void Start() {
        // iniciar com experimento de teste
        
        
        ExperimentLoader loader = new ();
        var data = loader.Load("Assets/ExperimentData/test_experiment-2.json");
        ExperimentDefinition testExperiment = ScriptableObject.CreateInstance<ExperimentDefinition>();
        testExperiment.experimentData = data;
        Debug.Log($"Loaded experiment: {testExperiment}");
        StartExperiment(testExperiment);
    }
}
