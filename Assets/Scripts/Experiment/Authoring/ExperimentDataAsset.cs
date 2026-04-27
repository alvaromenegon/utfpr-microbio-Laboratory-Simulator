using UnityEngine;

[CreateAssetMenu(fileName = "ExperimentDataAsset", menuName = "Lab/Authoring/Experiment Data")]
public class ExperimentDataAsset : ScriptableObject {
    public string description;
    public int time;
    public string[] objects;
}
