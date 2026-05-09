using UnityEngine;

public class ExperimentHudPresenter {
    private readonly HUDController hud;
    public static ExperimentHudPresenter Instance { get; private set; }

    private ExperimentHudPresenter(HUDController hudController) {        
        hud = hudController;
        Instance = this;
    }

    public static void New(HUDController hudController) {
        if (Instance == null) {
            new ExperimentHudPresenter(hudController);
        }
        else {
            Debug.LogWarning("ExperimentHudPresenter instance already exists. New instance will not be created.");
        }
    }

    public void Bind(ExperimentRuntime runtime, Assets.Scripts.StepManager stepManager) {
        runtime.OnTimerUpdated += HandleTimerUpdated;
        runtime.OnFeedback += HandleFeedback;   
        stepManager.OnStepChanged += HandleStepChanged;
    }

    public void Unbind(ExperimentRuntime runtime, Assets.Scripts.StepManager stepManager) {
        runtime.OnTimerUpdated -= HandleTimerUpdated;
        runtime.OnFeedback -= HandleFeedback;
        stepManager.OnStepChanged -= HandleStepChanged;
    }

    public void UpdateUserInfo(string username, string userPhotoUrl) {
        hud.UpdateUserInfo(username, userPhotoUrl);
    }

    private void HandleTimerUpdated(double elapsedSeconds, double limitSeconds) {
        hud.UpdateTimer($"Tempo: {elapsedSeconds:F0}/{limitSeconds:F0} s");
    }

    private void HandleStepChanged(Step step, int index, int total) {
        if (step == null) {
            return;
        }

        hud.UpdateStepInformation($"Step {index + 1}/{total}: {step.Description} - {step.Instruction}");
        hud.UpdateDescription(step.Description);
    }

    public void HandleFeedback(string message) {
        if (!string.IsNullOrEmpty(message)) {
            hud.UpdateInfo(message);
        }
    }
}
