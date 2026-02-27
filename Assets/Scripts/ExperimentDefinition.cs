using System;
using System.Collections.Generic;

[Serializable]
public class ExperimentDefinition
{
    public string experimentId;
    public string title;
    public List<ExperimentStep> steps = new List<ExperimentStep>();
    public string completionText = "Done";
}

[Serializable]
public class ExperimentStep
{
    public string stepId;
    public string instruction;
    public string expectedTargetId;
    public StepValidationType validationType = StepValidationType.Collision;
    public string successFeedback = "Done";
    public string failureFeedback = "Try again";
    public string visualEffectId;
    public string audioEffectId;
}

public enum StepValidationType
{
    Collision,
    Trigger
}
