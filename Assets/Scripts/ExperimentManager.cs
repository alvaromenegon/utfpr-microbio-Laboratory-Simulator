using Assets.Scripts;
using Assets.Scripts.Registry;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

enum ExperimentState {
    NotStarted,
    Running,
    Finished,
    InternalError
}

public class ExperimentManager : MonoBehaviour {
    [SerializeField] private GameObject label;
    [SerializeField] private GameObject secondaryTargetBeam;
    [SerializeField] private TMP_Text inputLabel;
    [SerializeField] private ExperimentApiClient apiClient;

    public static ExperimentManager Instance { get; private set; }
    public ExperimentDefinition CurrentExperiment { get; private set; }

    private ExperimentRuntime runtime;
    private ExperimentHudPresenter hudPresenter;
    private ExperimentState currentState = ExperimentState.NotStarted;
    public bool IsExperimentRunning => runtime != null && runtime.IsRunning;
    public InteractionSystem InteractionSystem;
    private EffectSystem EffectSystem;
    private EffectRegistry effectRegistry;
    private BackgroundProcessRegistry BackgroundProcessRegistry;
    private DerivedPropertyRegistry DerivedPropertyRegistry;
    private DerivedPropertySystem DerivedPropertySystem;
    private BackgroundProcessSystem BackgroundProcessSystem;
    private StepManager StepManager;

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Awake() {
        Instance = this;

        if (!SessionBootstrapper.IsSessionReady) {
            Debug.LogError("[ExperimentManager] Sessão indisponível. Manager interrompido.");
            return;
        }
                
        if (apiClient == null) {
            apiClient = GetComponent<ExperimentApiClient>();
        }

        if (apiClient == null) {
            apiClient = gameObject.AddComponent<ExperimentApiClient>();
        }
        runtime = new ExperimentRuntime();
        PropertyCatalog.Initialize();
        BackgroundProcessRegistry = new BackgroundProcessRegistry();
        DerivedPropertyRegistry = new DerivedPropertyRegistry();
        DerivedPropertySystem = new DerivedPropertySystem(DerivedPropertyRegistry);
        effectRegistry = new EffectRegistry();
        EffectSystem = new EffectSystem(effectRegistry);
        InteractionSystem = new InteractionSystem(effectRegistry, EffectSystem);
        InteractionSystem.OnValueRequired += request => InputPanelController.Instance.Show(request);
        
        BackgroundProcessSystem = new BackgroundProcessSystem(BackgroundProcessRegistry, EffectSystem, effectRegistry);

        StepManager = new StepManager(runtime);
        runtime.OnExperimentFinished += OnRuntimeFinished;
    }

    public EffectRegistry GetEffectRegistry() { // Interaction Resolver precisa acessar o EffectRegistry
        return effectRegistry;
    }

    private void Start() {
        ExperimentHudPresenter.New(HUDController.Instance);
        hudPresenter = ExperimentHudPresenter.Instance;
        hudPresenter.Bind(runtime, StepManager);
        var inputPanelController = InputPanelController.Instance;
        inputPanelController.Bind(EffectSystem);
        StartCoroutine(BootstrapAndStart());
    }

    private void Update() {
        if (IsExperimentRunning && currentState == ExperimentState.Running) {
            runtime?.Tick(DateTime.Now);
            InteractionSystem?.Update();
            EffectSystem?.Update(InteractionSystem.GetActiveInteractions());
            DerivedPropertySystem?.Update();
            StepManager?.Update();
            BackgroundProcessSystem?.Update();
        }       
    }

    public void StartExperiment(ExperimentDefinition experimentDefinition) {
        CurrentExperiment = experimentDefinition;
        //SetupExperimentObjects(experimentDefinition);
        effectRegistry.SetupRegistry(experimentDefinition.experimentData);
        BackgroundProcessRegistry.SetupRegistry(experimentDefinition.experimentData.BackgroundProcesses);
        DerivedPropertyRegistry.SetupRegistry(experimentDefinition.experimentData.DerivedProperties);

        runtime.Start(experimentDefinition);
        StepManager.Start();
        currentState = ExperimentState.Running;
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
   

    private void OnRuntimeFinished(int score, double timeTakenSeconds) {
        CurrentExperiment = null;
        Debug.Log($"Experiment ended. Final score: {score}");
        currentState = ExperimentState.Finished;
        StartCoroutine(apiClient.SendDataToServer(SessionData.Current, score, timeTakenSeconds, onSuccess: () => {
            Debug.Log("Result data sent successfully.");
        }, onError: error => {
            Debug.LogError($"Error sending result data: {error}");
            currentState = ExperimentState.InternalError;
        }));
    }
}
