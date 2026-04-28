using System;

[Serializable]
public class BackgroundProcessDto {
    public string Id { get; set; }
    public string Description { get; set; }
    public int Duration { get; set; }
    public int? Tolerance { get; set; }
    public string OnSuccessMessage { get; set; }
    public string OnFailureMessage { get; set; }
    public OnStepFinished ActionOnFailure { get; set; }
    public int OnFailureReturnToStep { get; set; }
    public int Score { get; set; }
    public int?[] ShouldCompleteBetweenSteps { get; set; }
}
