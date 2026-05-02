public class ExperimentHudPresenter {
    private readonly HUDController hud;
    private InteractionSystem interactionSystem;

    public ExperimentHudPresenter(HUDController hudController) {
        hud = hudController;
    }

    public void Bind(ExperimentRuntime runtime) {
        runtime.OnTimerUpdated += HandleTimerUpdated;
        runtime.OnStepChanged += HandleStepChanged;
        runtime.OnFeedback += HandleFeedback;
        interactionSystem.OnValueRequired += request =>
        {
            InputPanelController.Instance.Show(request);
        };
    }

    public void Unbind(ExperimentRuntime runtime) {
        runtime.OnTimerUpdated -= HandleTimerUpdated;
        runtime.OnStepChanged -= HandleStepChanged;
        runtime.OnFeedback -= HandleFeedback;
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

    private void HandleFeedback(string message) {
        if (!string.IsNullOrEmpty(message)) {
            hud.UpdateInfo(message);
        }
    }
}
