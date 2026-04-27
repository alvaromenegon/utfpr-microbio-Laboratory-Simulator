using System;

[Serializable]
public class ExperimentDefinitionDto {
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ExperimentDataDto ExperimentData { get; set; }
    public int UserId { get; set; }
}
