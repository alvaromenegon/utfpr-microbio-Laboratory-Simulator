using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentZone : MonoBehaviour {
    public string ZoneId;
    public List<string> Tags;

    private Dictionary<string, object> properties = new();

    public bool TryGetProperty(string property, out object value) {
        return properties.TryGetValue(property, out value);
    }

    public void SetProperty(string property, object value) {
        properties[property] = value;
    }

    public List<LaboratoryObject> GetObjectsInside() {
        var objects = ExperimentObjectRegistry.GetInstance().Query(requiredZoneTags: Tags);
        return objects;
    }
}