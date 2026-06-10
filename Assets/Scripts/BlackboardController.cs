
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackboardController : MonoBehaviour {
    [SerializeField] private Text title;
    [SerializeField] private Text blackboardText;
    [SerializeField] private Text stepsText;    

    public void SetTitle(string title) {
        this.title.text = title;
    }

    public void SetBlackboardText(string text) {
        blackboardText.text = text;
    }

    public void SetSteps(List<Step> steps, int currentStepIndex=0) {
        Debug.Log($"Setting steps with current step index {currentStepIndex}/{steps.Count}");
        var text = "";
        for (int i = 0; i < steps.Count; i++) {
            var step = steps[i];
            var stepText = $"{i + 1} - {step.Description}";
            if (i < currentStepIndex) {
                stepText = $"{stepText} (completo)";
            }
            else if (i == currentStepIndex) {
                stepText = $"> {stepText} <";
            }
            text += stepText + "\n";
            Debug.Log($"Step {i}: {stepText}");
        }
        this.stepsText.text = text;
        Debug.Log($"Steps text: {this.stepsText.text}");
    }
}