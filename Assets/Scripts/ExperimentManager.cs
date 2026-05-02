using Assets.Scripts;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Interaction;

public class ExperimentManager : MonoBehaviour {
    [SerializeField] private GameObject label;
    [SerializeField] private GameObject secondaryTargetBeam;
    [SerializeField] private TMP_Text inputLabel;
    [SerializeField] private ExperimentApiClient apiClient;

    public static ExperimentManager Instance { get; private set; }
    public ExperimentDefinition CurrentExperiment { get; private set; }

    private ExperimentRuntime runtime;
    private ExperimentHudPresenter hudPresenter;

    public bool IsExperimentRunning => runtime != null && runtime.IsRunning;
    private InteractionSystem InteractionSystem;
    private EffectSystem EffectSystem;
    private EffectRegistry effectRegistry;
    private DerivedPropertySystem DerivedPropertySystem;

    private void Awake() {
        Instance = this;

        if (!SessionBootstrapper.IsSessionReady) {
            Debug.LogError("[ExperimentManager] Sessão indisponível. Manager interrompido.");
            return;
        }

        InteractionSystem = new InteractionSystem();
        DerivedPropertySystem = new DerivedPropertySystem();

       if (apiClient == null) {
            apiClient = GetComponent<ExperimentApiClient>();
        }

        if (apiClient == null) {
            apiClient = gameObject.AddComponent<ExperimentApiClient>();
        }
       runtime = new ExperimentRuntime();
        runtime.OnStepChanged += OnRuntimeStepChanged;
        runtime.OnExperimentFinished += OnRuntimeFinished;
    }

    private void Start() {
        hudPresenter = new ExperimentHudPresenter(HUDController.Instance);
        hudPresenter.Bind(runtime);
        StartCoroutine(BootstrapAndStart());
    }

    private void Update() {
        runtime?.Tick(DateTime.Now);
        InteractionSystem?.Update();
        EffectSystem?.Update(InteractionSystem.GetActiveInteractions());
        DerivedPropertySystem?.Update();
        
        if (IsExperimentRunning && runtime.CurrentStep != null) {
            if (runtime.IsStepComplete(runtime.CurrentStep)) {
                runtime.NextStep(); // Or handle the step completion properly according to runtime logic
            }
        }
    }

    public void StartExperiment(ExperimentDefinition experimentDefinition) {
        CurrentExperiment = experimentDefinition;
        SetupExperimentObjects(experimentDefinition);
        effectRegistry.SetupRegistry(experimentDefinition.experimentData);
        runtime.Start(experimentDefinition);
    }

    public void NextStep() {
        if (!IsExperimentRunning) {
           runtime.NextStep();
        }
    } 

    public void End(int? finishScore = null) {
        if (!IsExperimentRunning) {
            return;
        }

        runtime.End(finishScore);
    }

    public void ProcessBackgroundWork(BackgroundProcess work, float timeUsed) {
        if (!IsExperimentRunning) {
            return;
        }

        runtime.ProcessBackgroundWork(work, timeUsed);
    }

    public void OnSecondaryTargetInteraction(GameObject gameObjectPrimary) {
        if (!IsExperimentRunning || runtime.CurrentStep == null) {
            return;
        }

        var currentStep = runtime.CurrentStep;
        var gameObjectSecondary = GameObject.Find(currentStep.InteractionTargetSecondary);
        if (gameObjectSecondary == null) {
            Debug.LogError($"Secondary target '{currentStep.InteractionTargetSecondary}' not found in the scene.");
            return;
        }

        var equipment = gameObjectSecondary.GetComponent<EquipmentInteractable>();
        if (equipment == null || currentStep.Interaction != InteractionType.MOVE) {
            return;
        }

        equipment.MoveObjectToEquipment(gameObjectPrimary);
    }

    private IEnumerator BootstrapAndStart() {
        Debug.Log($"Fetching experiment data for practice ID: {SessionData.Current.PracticeId}");

        yield return StartCoroutine(apiClient.FetchAndStartExperiment(
            SessionData.Current,
            onSuccess: data => {
                SessionData.Current.Username = data.Username;
                SessionData.Current.UserPhotoURL = data.UserPhotoURL;
                hudPresenter.UpdateUserInfo(data.Username, data.UserPhotoURL);
                StartExperiment(ExperimentMapper.ToModel(data.Practice));
            },
            onError: error => Debug.LogError(error)
        ));
    }

    private void SetupExperimentObjects(ExperimentDefinition experimentDefinition) {
        var stepsDescription = "";
        foreach (var step in experimentDefinition.experimentData.Steps) {
            stepsDescription += $"- {step.Description}\n";
        }

        label.GetComponent<Text>().text = $"{experimentDefinition.experimentData.Description}\n{stepsDescription}";

        foreach (var objName in experimentDefinition.experimentData.Objects) {
            GameObject obj = GameObject.Find(objName);
            if (obj == null) {
                Debug.LogError($"Object '{objName}' not found in the scene.");
                continue;
            }

            var interactable = obj.GetComponent<StepInteractable>();
            if (interactable == null) {
                obj.AddComponent<StepInteractable>();
            }
        }
    }

    private void OnRuntimeStepChanged(Step step, int index, int total) {
        secondaryTargetBeam.SetActive(false);
        inputLabel.text = "";

        if (step == null) {
            return;
        }

        var gameObject = GameObject.Find(step.InteractionTarget);
        if (gameObject == null) {
            Debug.LogError($"Target '{step.InteractionTarget}' not found in the scene.");
            return;
        }

        var interactable = gameObject.GetComponent<StepInteractable>();
        if (interactable == null) {
            interactable = gameObject.AddComponent<StepInteractable>();
        }

        interactable.stepType = step.Interaction;

    }   

    private void OnRuntimeFinished(int score, double timeTakenSeconds) {
        CurrentExperiment = null;
        Debug.Log($"Experiment ended. Final score: {score}");
        StartCoroutine(apiClient.SendDataToServer(SessionData.Current, score, timeTakenSeconds));
    }
}
