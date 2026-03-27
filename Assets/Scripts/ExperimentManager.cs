using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        gameObject.GetComponent<StepInteractable>().stepType = step.Interaction;
        if (step.Interaction == InteractionType.MOVE && !string.IsNullOrEmpty(step.InteractionTargetSecondary)) {
            var gameObjectSecondary = GameObject.Find(step.InteractionTargetSecondary);
            if (gameObjectSecondary == null) {
                Debug.LogError($"Secondary target '{step.InteractionTargetSecondary}' not found in the scene.");
                return;
            }
            secondaryTargetBeam.transform.position = gameObjectSecondary.transform.position;
            secondaryTargetBeam.SetActive(true);
        }
        else if (RequireValue(step.Interaction)) {
            inputLabel.text = $"Enter value for {step.Description}:";
        }
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
            interactable.stepType = CurrentExperiment.experimentData.Steps[0].Interaction; // definir o tipo de interação esperado para o primeiro passo
        }
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
        NextStep();
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
        IsExperimentRunning = false;
        if (finishScore != null) {
            score = finishScore.Value;
        }

        End();
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
        GUI.Label(new Rect(10, 90, 400, 20), $"Instructions: {(IsExperimentRunning ? CurrentStep.Instruction : "No experiment running")}");
        GUI.Label(new Rect(10, 110, 400, 20), $"Score: {score}/100");
    }

    private void Start() {
        // iniciar com experimento de teste
        
        
        ExperimentLoader loader = new ();
        //var data = loader.Load("Assets/ExperimentData/test_experiment-2.json");
        var data = loader.Load("Assets/ExperimentData/experiment_definition-2.json");
        ExperimentDefinition testExperiment = ScriptableObject.CreateInstance<ExperimentDefinition>();
        testExperiment.experimentData = data;
        Debug.Log($"Loaded experiment: {testExperiment}");
        StartExperiment(testExperiment);
    }

    private void End() {
        // encerrar o experimento, enviar dados para servidor, etc.
        // apenas mostrar uma mensagem por enquanto
        Debug.Log("Experiment ended. Final score: " + score);
    }
}
