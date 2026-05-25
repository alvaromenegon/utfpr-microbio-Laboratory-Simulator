
using System.Collections.Generic;

public class ExperimentInstanceObject {
    public string CanonicalIdentifier { get; set; }
    public string InstanceId { get; set; }
    public ObjectBinding Binding { get; set; }
    public List<string> Tags { get; set; }
    public Dictionary<string, object> InitialProperties { get; set; } = new Dictionary<string, object>();

    public override string ToString() {
        return $"ExperimentInstanceObject(CanonicalIdentifier={CanonicalIdentifier}, InstanceId={InstanceId}, Binding={Binding}, Tags=[{string.Join(", ", Tags ?? new List<string>())}], InitialProperties=[{string.Join(", ", InitialProperties)}])";
    }
}

public class ObjectBinding {
    public ObjectBindingType Type;
    public string SceneObjectId;
    public string SpawnLocation;
}

public enum ObjectBindingType {
    SCENE,
    SPAWN
}