using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExperimentRunner : MonoBehaviour
{
    [Header("Data")]
    public ExperimentDefinition definition;

    [Header("UI")]
    public Text nextStepCanvas;
    public Text[] progress;
    public GameObject playerTooltip;
    public GameObject nextStep;
    public GameObject congrats;

    [Header("Feedback")]
    public AudioSource right;
    public AudioSource wrong;
    public AudioSource explain;

    [Header("Scene actions")]
    public GameObject[] blackboardTexts;
    public GameObject[] objectsToDisableOnFinish;
    public GameObject[] objectsToEnableOnFinish;

    [Header("Optional effect maps")]
    public StepVisualEffect[] visualEffects;
    public StepAudioEffect[] audioEffects;

    [Header("Events")]
    public UnityEvent onExperimentCompleted;

    private int currentStepIndex;
    private Dictionary<string, GameObject> visualEffectMap = new Dictionary<string, GameObject>();
    private Dictionary<string, AudioSource> audioEffectMap = new Dictionary<string, AudioSource>();

    private void Start()
    {
        BuildEffectMaps();
        currentStepIndex = 0;
        RenderProgress();
        RenderNextInstruction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryAdvanceStep(collision.gameObject, StepValidationType.Collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAdvanceStep(other.gameObject, StepValidationType.Trigger);
    }

    private void TryAdvanceStep(GameObject interactingObject, StepValidationType validationType)
    {
        if (definition == null || definition.steps == null || definition.steps.Count == 0 || IsCompleted())
        {
            return;
        }

        ExperimentStep step = definition.steps[currentStepIndex];
        if (step.validationType != validationType)
        {
            return;
        }

        if (IsValidObject(step.expectedTargetId, interactingObject))
        {
            ApplyStepSuccess(step);
            currentStepIndex++;
            RenderProgress();

            if (IsCompleted())
            {
                CompleteExperiment();
            }
            else
            {
                RenderNextInstruction();
            }

            return;
        }

        ApplyStepFailure(step);
    }

    private bool IsValidObject(string expectedTargetId, GameObject interactingObject)
    {
        if (string.IsNullOrEmpty(expectedTargetId) || interactingObject == null)
        {
            return false;
        }

        ExperimentObjectIdentifier identifier = interactingObject.GetComponent<ExperimentObjectIdentifier>();
        return identifier != null && identifier.objectId == expectedTargetId;
    }

    private void ApplyStepSuccess(ExperimentStep step)
    {
        if (right != null)
        {
            right.Play();
        }

        PlayStepAudio(step.audioEffectId);
        ShowStepVisual(step.visualEffectId);
    }

    private void ApplyStepFailure(ExperimentStep step)
    {
        if (playerTooltip != null)
        {
            playerTooltip.SetActive(true);
        }

        if (wrong != null)
        {
            wrong.Play();
        }
    }

    private void RenderProgress()
    {
        if (progress == null || definition == null || definition.steps == null)
        {
            return;
        }

        int stepsCount = definition.steps.Count;
        int labelsCount = Math.Min(progress.Length, stepsCount);

        for (int i = 0; i < labelsCount; i++)
        {
            if (i < currentStepIndex)
            {
                progress[i].text = string.IsNullOrEmpty(definition.steps[i].successFeedback)
                    ? "Done"
                    : definition.steps[i].successFeedback;
            }
            else
            {
                progress[i].text = definition.steps[i].instruction;
            }
        }
    }

    private void RenderNextInstruction()
    {
        if (nextStepCanvas == null)
        {
            return;
        }

        if (definition == null || definition.steps == null || definition.steps.Count == 0)
        {
            nextStepCanvas.text = string.Empty;
            return;
        }

        if (IsCompleted())
        {
            nextStepCanvas.text = string.IsNullOrEmpty(definition.completionText) ? "Done" : definition.completionText;
            return;
        }

        nextStepCanvas.text = definition.steps[currentStepIndex].instruction;
    }

    private bool IsCompleted()
    {
        return definition != null && definition.steps != null && currentStepIndex >= definition.steps.Count;
    }

    private void CompleteExperiment()
    {
        DisableObjects(blackboardTexts);
        DisableObjects(objectsToDisableOnFinish);
        EnableObjects(objectsToEnableOnFinish);

        if (congrats != null)
        {
            congrats.SetActive(true);
        }

        if (nextStep != null)
        {
            nextStep.SetActive(true);
        }

        if (explain != null)
        {
            explain.Play();
        }

        RenderNextInstruction();
        onExperimentCompleted?.Invoke();
    }

    private static void DisableObjects(GameObject[] objects)
    {
        if (objects == null)
        {
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].SetActive(false);
            }
        }
    }

    private static void EnableObjects(GameObject[] objects)
    {
        if (objects == null)
        {
            return;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].SetActive(true);
            }
        }
    }

    private void BuildEffectMaps()
    {
        visualEffectMap.Clear();
        audioEffectMap.Clear();

        if (visualEffects != null)
        {
            for (int i = 0; i < visualEffects.Length; i++)
            {
                if (!string.IsNullOrEmpty(visualEffects[i].effectId) && visualEffects[i].target != null)
                {
                    visualEffectMap[visualEffects[i].effectId] = visualEffects[i].target;
                }
            }
        }

        if (audioEffects != null)
        {
            for (int i = 0; i < audioEffects.Length; i++)
            {
                if (!string.IsNullOrEmpty(audioEffects[i].effectId) && audioEffects[i].target != null)
                {
                    audioEffectMap[audioEffects[i].effectId] = audioEffects[i].target;
                }
            }
        }
    }

    private void ShowStepVisual(string effectId)
    {
        if (string.IsNullOrEmpty(effectId))
        {
            return;
        }

        GameObject effect;
        if (visualEffectMap.TryGetValue(effectId, out effect))
        {
            effect.SetActive(true);
        }
    }

    private void PlayStepAudio(string effectId)
    {
        if (string.IsNullOrEmpty(effectId))
        {
            return;
        }

        AudioSource effect;
        if (audioEffectMap.TryGetValue(effectId, out effect))
        {
            effect.Play();
        }
    }
}

[Serializable]
public struct StepVisualEffect
{
    public string effectId;
    public GameObject target;
}

[Serializable]
public struct StepAudioEffect
{
    public string effectId;
    public AudioSource target;
}
