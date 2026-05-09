using System;
using UnityEngine;

namespace Assets.Scripts {
    public class StepManager {
        private ExperimentRuntime experimentRuntime;
        private Step currentStep = null;
        public event Action<Step, int, int> OnStepChanged;

        public StepManager(ExperimentRuntime experimentRuntime) {
            this.experimentRuntime = experimentRuntime;
        }

        private void AddScore(int score) {
            experimentRuntime.AddScore(score);
        }

        private void CompleteStepSuccess() {
            AddScore(currentStep.Score);
            Debug.Log($"Step '{currentStep.Description}' completed successfully. Added {currentStep.Score} points.");
            Debug.Log(currentStep.OnSuccessMessage);
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
        }

        private void GoToStep(int stepIndex) {
            currentStep = experimentRuntime.Definition.experimentData.Steps[stepIndex];
        }

        private void CompleteExperiment(bool withSuccess = true) {
            AddScore(currentStep.Score);
            experimentRuntime.End(withSuccess);
        }

        private void NextStep() {
            if (currentStep == null) {
                currentStep = experimentRuntime.Definition.experimentData.Steps[0];
            } else {
                int currentIndex = experimentRuntime.Definition.experimentData.Steps.IndexOf(currentStep);
                if (currentIndex < experimentRuntime.Definition.experimentData.Steps.Count - 1) {
                    currentStep = experimentRuntime.Definition.experimentData.Steps[currentIndex + 1];
                } else {
                    // No more steps, experiment finished
                    currentStep = null;
                }
            }
        }

        public void Start() {
            currentStep = experimentRuntime.Definition.experimentData.Steps[0];
        }

        public void Update() {
            if (currentStep == null) return;
            if (ConditionEvaluator.Check(currentStep.Goals)) {
                CompleteStepSuccess();
            }

        }
    }
}