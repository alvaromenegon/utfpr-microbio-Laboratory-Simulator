using System;
using System.Collections.Generic;

[Serializable]
public class ExperimentObjectDto {
    public string Identifier { get; set; }
    public List<InstanceObjectDto> Instances { get; set; }
}

[Serializable]
public class InstanceObjectDto {
    public string InstanceId { get; set; }
    public string SpawnLocation { get; set; }
}
