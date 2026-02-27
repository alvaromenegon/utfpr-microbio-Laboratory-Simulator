using System.Collections.Generic;
using UnityEngine;

public class BiologyExperimentOne : ExperimentRunner
{
    [Header("Pilot migration data")]
    public string experimentId = "bio-exp-001";
    public string[] instructions;
    public string[] stepTargetIds;
    public string[] stepIds;
    public GameObject process;
    public GameObject finalObject;

    private void Awake()
    {
        if (definition != null && definition.steps != null && definition.steps.Count > 0)
        {
            return;
        }

        List<ExperimentStep> steps = new List<ExperimentStep>();
        int count = Mathf.Min(instructions == null ? 0 : instructions.Length, stepTargetIds == null ? 0 : stepTargetIds.Length);

        for (int i = 0; i < count; i++)
        {
            steps.Add(new ExperimentStep
            {
                stepId = stepIds != null && i < stepIds.Length && !string.IsNullOrEmpty(stepIds[i])
                    ? stepIds[i]
                    : string.Format("{0}-step-{1}", experimentId, i + 1),
                instruction = instructions[i],
                expectedTargetId = stepTargetIds[i],
                validationType = StepValidationType.Collision,
                successFeedback = "Done",
                failureFeedback = "Try the expected object"
            });
        }

        definition = new ExperimentDefinition
        {
            experimentId = experimentId,
            title = "Biology Experiment One",
            steps = steps,
            completionText = "Done"
        };

        objectsToEnableOnFinish = Merge(objectsToEnableOnFinish, process, finalObject);
    }

    private static GameObject[] Merge(GameObject[] current, params GameObject[] toAppend)
    {
        List<GameObject> result = new List<GameObject>();

        if (current != null)
        {
            result.AddRange(current);
        }

        for (int i = 0; i < toAppend.Length; i++)
        {
            if (toAppend[i] != null && !result.Contains(toAppend[i]))
            {
                result.Add(toAppend[i]);
            }
        }

        return result.ToArray();
    }
}
