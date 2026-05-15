using System;
using System.Collections.Generic;

[Serializable]
public class ExperimentData {
    public List<Step> Steps { get; set; }
    public List<BackgroundProcess> BackgroundProcesses { get; set; }
    public List<ExperimentObject> Objects { get; set; }
    public string Description { get; set; }
    public int Time { get; set; }
    public List<EffectDefinition> Effects { get; set; }
    public List<ReactiveEffect> ReactiveEffects { get; set; }
    public List<ActionEffect> ActionEffects { get; set; }
    public List<DerivedProperty> DerivedProperties { get; set; }

    public bool HasTimeLimit() {
        return Time > 0;
    }

    public override string ToString() {
        string result = $"Experiment: {Description}\nTime Limit: {(HasTimeLimit() ? Time.ToString() + " seconds" : "None")}\nObjects: {string.Join(", ", Objects)}\nSteps:\n";
        for (int i = 0; i < Steps.Count; i++) {
            var step = Steps[i];
            result += $"  Step {i + 1}: {step.Description}, Interaction: {step.Interaction}, Target: {step.InteractionTarget}, Instruction: {step.Instruction}, Value: {step.Value}, OnSuccess: {step.OnSuccessMessage}, OnFailure: {step.OnFailureMessage}, ActionOnSuccess: {step.ActionOnSuccess}, ActionOnFailure: {step.ActionOnFailure}, OnFailureReturnToStep: {step.OnFailureReturnToStep}\n";
        }
        return result;
    }
}