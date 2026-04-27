using System;
using System.Collections.Generic;

public class ExperimentRuntime {
    public ExperimentDefinition Definition { get; private set; }
    public bool IsRunning { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public int Score { get; private set; }
    public DateTime StartTime { get; private set; }
    public Step CurrentStep => Definition?.experimentData?.Steps?[CurrentStepIndex];

    private readonly List<BackgroundProcess> backgroundProcesses = new();

    public event Action<Step, int, int> OnStepChanged;
    public event Action<string> OnFeedback;
    public event Action<int, double> OnExperimentFinished;
    public event Action<double, double> OnTimerUpdated;

    public void Start(ExperimentDefinition definition) {
        Definition = definition;
        CurrentStepIndex = 0;
        Score = 0;
        backgroundProcesses.Clear();
        StartTime = DateTime.Now;
        IsRunning = true;

        PublishStep();
    }

    public void Tick(DateTime now) {
        if (!IsRunning || Definition == null) {
            return;
        }

        double elapsedSeconds = (now - StartTime).TotalSeconds;
        OnTimerUpdated?.Invoke(elapsedSeconds, Definition.experimentData.Time);

        if (Definition.experimentData.HasTimeLimit() && elapsedSeconds > Definition.experimentData.Time) {
            Finish();
        }
    }

    public void ProcessInteraction(PlayerAction interaction) {
        if (!IsRunning || CurrentStep == null) {
            return;
        }

        bool valid = interaction.InteractionType == CurrentStep.Interaction && interaction.Target.name == CurrentStep.InteractionTarget;
        if (valid) {
            ValidStep();
            return;
        }

        InvalidStep();
    }

    public void ValidateValue(double? value) {
        if (!IsRunning || CurrentStep == null) {
            return;
        }

        double? expectedValue = CurrentStep.Value;
        double tolerance = CurrentStep.Tolerance ?? 0.001;
        if (expectedValue == null || value == null) {
            InvalidStep();
            return;
        }

        if (Math.Abs(value.Value - expectedValue.Value) > tolerance) {
            InvalidStep();
            return;
        }

        ValidStep();
    }

    public void ProcessBackgroundWork(BackgroundProcess work, float timeUsed) {
        if (!IsRunning || work == null) {
            return;
        }

        var tolerance = work.Tolerance ?? 0.5f;
        if (Math.Abs(timeUsed - work.Duration) > tolerance || !work.ValidateCompletion(CurrentStepIndex)) {
            InvalidBackgroundWork(work);
            return;
        }

        ValidBackgroundWork(work);
    }

    public void RegisterBackgroundWork(BackgroundProcess work) {
        if (work == null) {
            return;
        }

        if (!backgroundProcesses.Contains(work)) {
            backgroundProcesses.Add(work);
        }
    }

    public void ReturnToStep(int stepIndex) {
        if (!IsRunning || Definition == null) {
            return;
        }

        if (stepIndex < 0 || stepIndex >= Definition.experimentData.Steps.Length) {
            return;
        }

        CurrentStepIndex = stepIndex;
        PublishStep();
    }

    private void ValidStep() {
        OnFeedback?.Invoke(CurrentStep.OnSuccessMessage);
        Score += CurrentStep.Score;

        if (!string.IsNullOrEmpty(CurrentStep.OnSuccessStartBackgroundWorkId)) {
            var work = Array.Find(Definition.experimentData.BackgroundProcesses, w => w.Id == CurrentStep.OnSuccessStartBackgroundWorkId);
            RegisterBackgroundWork(work);
            work?.Start();
        }

        AdvanceStep();
    }

    private void InvalidStep() {
        var step = CurrentStep;
        OnFeedback?.Invoke(step.OnFailureMessage);
        switch (step.ActionOnFailure) {
            case OnStepFinished.BACK_TO_STEP:
                ReturnToStep(step.OnFailureReturnToStep);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_FAILURE:
                Finish(0);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_STEP:
                Finish();
                break;
        }
    }

    private void ValidBackgroundWork(BackgroundProcess work) {
        OnFeedback?.Invoke(work.OnSuccessMessage);
        Score += work.Score;
    }

    private void InvalidBackgroundWork(BackgroundProcess work) {
        OnFeedback?.Invoke(work.OnFailureMessage);
        switch (work.ActionOnFailure) {
            case OnStepFinished.BACK_TO_STEP:
                ReturnToStep(work.OnFailureReturnToStep);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_FAILURE:
                Finish(0);
                break;
            case OnStepFinished.COMPLETE_EXPERIMENT_STEP:
                Finish();
                break;
        }
    }

    public void NextStep() {
        if (!IsRunning || CurrentStep == null) {
            return;
        }

        AdvanceStep();
    }

    public void End(int? finishScore = null) {
        Finish(finishScore);
    }

    private void AdvanceStep() {
        if (CurrentStep.ActionOnSuccess == OnStepFinished.COMPLETE_EXPERIMENT_SUCCESS) {
            Finish();
            return;
        }

        CurrentStepIndex++;
        if (CurrentStepIndex >= Definition.experimentData.Steps.Length) {
            Finish();
            return;
        }

        PublishStep();
    }

    private bool HasBackgroundWorkRunning() {
        foreach (var work in backgroundProcesses) {
            if (work.IsRunning) {
                return true;
            }
        }

        return false;
    }

    private void PublishStep() {
        var total = Definition?.experimentData?.Steps?.Length ?? 0;
        OnStepChanged?.Invoke(CurrentStep, CurrentStepIndex, total);
    }

    private void Finish(int? finishScore = null) {
        if (HasBackgroundWorkRunning()) {
            return;
        }

        IsRunning = false;
        if (finishScore.HasValue) {
            Score = finishScore.Value;
        }

        double elapsedSeconds = (DateTime.Now - StartTime).TotalSeconds;
        OnExperimentFinished?.Invoke(Score, elapsedSeconds);
    }
}
