using System;

[Serializable]
public class ExperimentDataDto {
    public StepDto[] Steps { get; set; }
    public BackgroundProcessDto[] BackgroundProcesses { get; set; }
    public string[] Objects { get; set; }
    public string Description { get; set; }
    public int Time { get; set; }
}
