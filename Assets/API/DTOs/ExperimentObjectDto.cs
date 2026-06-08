using System;
using System.Collections.Generic;

[Serializable]
public class ExperimentObjectDto {
    public string CanonicalIdentifier { get; set; }
    public string InstanceId { get; set; }
    public string DisplayName { get; set; }
    public string[] Tags { get; set; }
    public ObjectBindingDto Binding { get; set; }
    public Dictionary<string, object> InitialProperties { get; set; } = new Dictionary<string, object>();
}

[Serializable]
public class ObjectBindingDto {
    public string Type;
    public string SceneObjectId;
    public string SpawnLocation;
}