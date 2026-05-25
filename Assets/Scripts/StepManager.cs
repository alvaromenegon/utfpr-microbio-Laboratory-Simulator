using System;
using UnityEngine;

namespace Assets.Scripts {
    public class StepManager {
        private ExperimentRuntime experimentRuntime;
        private Step currentStep = null;
        public event Action<Step, int, int> OnStepChanged;
        public event Action<int> OnScoreUpdated;
        public event Action<string> OnFeedback;

        public StepManager(ExperimentRuntime experimentRuntime) {
            this.experimentRuntime = experimentRuntime;
        }

        private void AddScore(int score) {
            experimentRuntime.AddScore(score);
            OnScoreUpdated?.Invoke(experimentRuntime.Score);
        }

        private void CompleteStepSuccess() {
            AddScore(currentStep.Score);
            Debug.Log($"Step '{currentStep.Description}' completed successfully. Added {currentStep.Score} points.");
            Debug.Log(currentStep.OnSuccessMessage);
            OnFeedback?.Invoke(currentStep.OnSuccessMessage);
            HandleStepAction(currentStep.ActionOnSuccess);

        }

        void HandleStepAction(OnStepFinished action) {
            switch (action) {
                case OnStepFinished.COMPLETE_EXPERIMENT_SUCCESS:
                    CompleteExperiment(true);
                    break;

                case OnStepFinished.COMPLETE_EXPERIMENT_FAILURE:
                    CompleteExperiment(false);
                    break;

                case OnStepFinished.BACK_TO_STEP:
                    GoToStep(currentStep.OnFailureReturnToStep);
                    break;

                case OnStepFinished.BACK_TO_START:
                    RestartExperiment();
                    break;

                case OnStepFinished.COMPLETE_EXPERIMENT_STEP:
                case OnStepFinished.DEFAULT:
                default:
                    NextStep();
                    break;
            }
        }

        private void RestartExperiment() {
            currentStep = experimentRuntime.Definition.experimentData.Steps[0];
            OnStepChanged?.Invoke(currentStep, experimentRuntime.Score, 0);
        }

        private void GoToStep(int stepIndex) {
            currentStep = experimentRuntime.Definition.experimentData.Steps[stepIndex];
            OnStepChanged?.Invoke(currentStep, experimentRuntime.Score, 0);
        }

        private void CompleteExperiment(bool withSuccess = true) {
            experimentRuntime.End(withSuccess);
        }

        private void NextStep() {
            if (currentStep == null) {
                currentStep = experimentRuntime.Definition.experimentData.Steps[0];
            } else {
                int currentIndex = experimentRuntime.Definition.experimentData.Steps.IndexOf(currentStep);
                if (currentIndex < experimentRuntime.Definition.experimentData.Steps.Count - 1) {
                    currentStep = experimentRuntime.Definition.experimentData.Steps[currentIndex + 1];
                    OnStepChanged?.Invoke(currentStep, experimentRuntime.Score, currentIndex + 1);
                } else {
                    // No more steps, experiment finished
                    currentStep = null;
                }
            }
        }

        public void Start() {
            currentStep = experimentRuntime.Definition.experimentData.Steps[0];
            OnStepChanged?.Invoke(currentStep, experimentRuntime.Score, 0);
        }

        public void Update() {
            if (currentStep == null) return;            
            if (ConditionEvaluator.Check(currentStep.Goals)) {
                Debug.Log($"Step '{currentStep.Description}' goals achieved.");
                CompleteStepSuccess();
            }

        }
    }
}