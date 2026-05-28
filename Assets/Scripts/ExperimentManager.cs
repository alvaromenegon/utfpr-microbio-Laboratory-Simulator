using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
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
    private BackgroundProcessSystem BackgroundProcessSystem;
    private StepManager StepManager;
    private VisualSlotRegistry visualSlotRegistry;
    private VisualAssetRegistry visualAssetRegistry;
    private VisualAssetLoader visualAssetLoader;
    private VisualStateSystem visualStateSystem;

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
        effectRegistry = new EffectRegistry();
        EffectSystem = new EffectSystem(effectRegistry);
        InteractionSystem = new InteractionSystem(effectRegistry, EffectSystem);
        InteractionSystem.OnValueRequired += request => InputPanelController.Instance.Show(request);
        visualSlotRegistry = VisualSlotRegistry.GetInstance();
        visualAssetRegistry = VisualAssetRegistry.GetInstance();
        visualStateSystem = new VisualStateSystem(visualSlotRegistry);

        BackgroundProcessSystem = new BackgroundProcessSystem(BackgroundProcessRegistry, EffectSystem, effectRegistry);
        EffectSystem.BindBackgroundProcessSystem(BackgroundProcessSystem);
        visualAssetLoader = new VisualAssetLoader(visualAssetRegistry, new TextureLoader(), new MaterialFactory());        

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
        if (IsExperimentRunning && currentState == ExperimentState.Running && visualAssetLoader.IsReady) {
            runtime?.Tick(DateTime.Now);
            InteractionSystem?.Update();
            EffectSystem?.Update(InteractionSystem.GetActiveInteractions());
            StepManager?.Update();
            BackgroundProcessSystem?.Update();
            visualStateSystem?.Update();
        }       
    }

    public void StartExperiment(ExperimentDefinition experimentDefinition) {
        CurrentExperiment = experimentDefinition;
        //experimentObjectRegistry.SetupRegistry(experimentDefinition.experimentData.Objects);
        effectRegistry.SetupRegistry(experimentDefinition.experimentData);
        BackgroundProcessRegistry.SetupRegistry(experimentDefinition.experimentData.BackgroundProcesses);
        SetupVisualSystem(experimentDefinition.experimentData);
        SetupScene(experimentDefinition.experimentData.Objects);        

        runtime.Start(experimentDefinition);
        StepManager.Start();
        currentState = ExperimentState.Running;
        Debug.Log($"Experiment '{experimentDefinition.title}' is running");
    }

    private void SetupVisualSystem(ExperimentData data) {
        VisualStateRegistry.Instance.SetupRegistry(data.VisualStateRules);
        StartCoroutine(visualAssetLoader.LoadAssets(data.VisualAssets));
        Debug.Log($"Visual system configured with {data.VisualStateRules.Count} state rules and {data.VisualAssets.Count} assets.");

    }

    private void SetupScene(List<ExperimentInstanceObject> objectsDefinition) {
        foreach (var def in objectsDefinition) {
            LaboratoryObject obj = def.Binding.Type switch {
                ObjectBindingType.SCENE => BindSceneObject(def),
                ObjectBindingType.SPAWN => SpawnRuntimeObject(def),
                _ => null
            };

            if (obj == null) {
                Debug.LogError($"Failed to load object {def.InstanceId}");
                continue;
            }

            ConfigureObject(obj, def);
            ExperimentObjectRegistry.GetInstance().Register(obj);
        }
    }

    private LaboratoryObject BindSceneObject(ExperimentInstanceObject def) {
        if (!SceneObjectRegistry.GetInstance().TryGet(def.Binding.SceneObjectId, out var obj)) {
            Debug.LogError($"Scene object not found: {def.Binding.SceneObjectId}");
            return null;
        }

        return obj;
    }

    private LaboratoryObject SpawnRuntimeObject(ExperimentInstanceObject def) {
        var prefab = PrefabRegistry.GetInstance().GetPrefab(def.CanonicalIdentifier);


        if (!SpawnLocationRegistry.GetInstance().TryResolve(def.Binding.SpawnLocation, out var spawn)) {
            Debug.LogError($"Spawn point not found: {def.Binding.SpawnLocation}");
            return null;
        }

        var instance = Instantiate(prefab, spawn.position, spawn.rotation);

        var labObject = instance.GetComponent<LaboratoryObject>();

        if (labObject == null) {
            Debug.LogError($"Prefab {def.CanonicalIdentifier} has no LaboratoryObject.");
            return null;
        }

        return labObject;
    }

    private void ConfigureObject(
    LaboratoryObject obj,
    ExperimentInstanceObject def) {
        obj.InstanceId = def.InstanceId;
        obj.canonicalIdentifier = def.CanonicalIdentifier;
        Debug.Log($"Configuring object {obj}");

        if (def.Tags != null) {
            // apenas adiciona as tags definidas, sem sobrescrever
            if (obj.Tags == null) {
                obj.Tags = new List<string>(def.Tags);
            } else {
                foreach (var tag in def.Tags) {
                    if (!obj.Tags.Contains(tag)) {
                        obj.Tags.Add(tag);
                    }
                }
            }
        }

        if (def.InitialProperties != null) {
            foreach (var pair in def.InitialProperties) {
                obj.SetProperty(pair.Key, pair.Value);
            }
        }

        visualSlotRegistry.RegisterSlotsByObject(obj);
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
