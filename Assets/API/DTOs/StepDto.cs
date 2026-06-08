using System;

[Serializable]
public class StepDto {
    public string Description { get; set; }
    public int Score { get; set; }
    public string Instruction { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public OnStepFinished ActionOnSuccess { get; set; }
    public WhenDto[] Goals { get; set; }
    public WhenDto[] FailureConditions { get; set; } = new WhenDto[0];
}
