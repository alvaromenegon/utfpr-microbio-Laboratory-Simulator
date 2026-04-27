using System;

[Serializable]
public class StepDto {
    public string Description { get; set; }
    public int Score { get; set; }
    public string Interaction { get; set; }
    public string InteractionTarget { get; set; }
    public string InteractionTargetSecondary { get; set; }
    public string Instruction { get; set; }
    public double? Value { get; set; }
    public double? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public OnStepFinished ActionOnSuccess { get; set; }
    public string OnSuccessStartBackgroundWorkId { get; set; }
}
