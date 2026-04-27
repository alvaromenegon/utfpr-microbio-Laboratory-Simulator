using UnityEngine;

[CreateAssetMenu(fileName = "ExperimentDefinitionAsset", menuName = "Lab/Authoring/Experiment Definition")]
public class ExperimentDefinitionAsset : ScriptableObject {
    public string id;
    public string title;
    public ExperimentDataAsset experimentData;
    public int userId;
}
