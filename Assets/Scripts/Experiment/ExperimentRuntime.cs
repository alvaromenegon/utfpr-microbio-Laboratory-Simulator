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

    public void End(bool withSuccess) {
        Finish();
    }

    public void AddScore(int score) {
        Score += score;
    }

    private void Finish(int? finishScore = null) {
        IsRunning = false;
        if (finishScore.HasValue) {
            Score = finishScore.Value;
        }

        double elapsedSeconds = (DateTime.Now - StartTime).TotalSeconds;
        OnExperimentFinished?.Invoke(Score, elapsedSeconds);
    }
}
