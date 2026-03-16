using System;
using UnityEngine;

[Serializable]
public class ExperimentData : ScriptableObject {
    public Step[] Steps { get; set; }
    public string[] Objects { get; set; }
    public string Description { get; set; }
    public int Time { get; set; }

    public bool HasTimeLimit() {
        return Time > 0;
    }

    public void Init(ExperimentData experimentData) {
        this.Steps = experimentData.Steps;
        this.Objects = experimentData.Objects;
        this.Description = experimentData.Description;
        this.Time = experimentData.Time;

    }

    public override string ToString() {
        string result = $"Experiment: {Description}\nTime Limit: {(HasTimeLimit() ? Time.ToString() + " seconds" : "None")}\nObjects: {string.Join(", ", Objects)}\nSteps:\n";
        for (int i = 0; i < Steps.Length; i++) {
            var step = Steps[i];
            result += $"  Step {i + 1}: {step.Description}, Interaction: {step.Interaction}, Target: {step.InteractionTarget}, Instruction: {step.Instruction}, Value: {step.Value}, OnSuccess: {step.OnSuccessMessage}, OnFailure: {step.OnFailureMessage}, ActionOnSuccess: {step.ActionOnSuccess}, ActionOnFailure: {step.ActionOnFailure}, OnFailureReturnToStep: {step.OnFailureReturnToStep}\n";
        }
        return result;
    }


}

[Serializable]
public class Step {
    public string Description { get; set; }  // Descrição simples do passo
    public int Score { get; set; }
    public string interaction; // Campo privado para armazenar o valor da interação como string
    public InteractionType Interaction {
        get {
            if (Enum.TryParse(interaction, out InteractionType result)) {
                return result;
            }
            Debug.LogWarning($"Invalid interaction type: {interaction}. Defaulting to NONE.");
            return InteractionType.USE; // Valor padrão caso a string seja inválida
        }
    } //  O tipo de interação com o GameObject
    public string InteractionTarget { get; set; } // O nome do GameObject com o qual o usuário deve interagir, deve implementar Interactable
    public string Instruction { get; set; } // Instrução detalhada para o passo
    public int Value { get; set; } // Valor associado à interação, se aplicável (ex: temperatura, tempo, quantidade)
    public string OnSuccessMessage { get; set; } // Mensagem ou ação a ser executada em caso de sucesso
    public string OnFailureMessage { get; set; } // Mensagem ou ação a ser executada em caso de falha
    public OnStepFinished ActionOnFailure { get; set; } // Ação a ser executada em caso de falha
    public int OnFailureReturnToStep { get; set; } // Passo para o qual o usuário deve retornar em caso de falha (se aplicável)
    public OnStepFinished ActionOnSuccess { get; set; } // Ação a ser executada em caso de sucesso

    public bool IsValid(int numSteps) {
        if (string.IsNullOrEmpty(Description)) {
            Debug.LogError("Step description is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(interaction)) {
            Debug.LogError("Step interaction is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(InteractionTarget)) {
            Debug.LogError("Step interaction target is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(Instruction)) {
            Debug.LogError("Step instruction is null or empty.");
            return false;
        }
        if (ActionOnFailure == OnStepFinished.BACK_TO_STEP && (OnFailureReturnToStep < 0 || OnFailureReturnToStep >= numSteps)) {
            Debug.LogError("Step action on failure is BACK_TO_STEP but OnFailureReturnToStep is not set to a valid step index.");
            return false;
        }
        return true;
    }

    public override string ToString() {
        return $"Step: {Description}, Interaction: {Interaction}, Target: {InteractionTarget}, Instruction: {Instruction}, Value: {Value}, OnSuccess: {OnSuccessMessage}, OnFailure: {OnFailureMessage}, ActionOnSuccess: {ActionOnSuccess}, ActionOnFailure: {ActionOnFailure}, OnFailureReturnToStep: {OnFailureReturnToStep}";
    }
}

public enum OnStepFinished {
   DEFAULT, // segue o fluxo normal do experimento (avança para o próximo passo, ou encerra com sucesso se for o último passo)
    COMPLETE_EXPERIMENT_SUCCESS, //completa imediatamente, com sucesso (100%)
    COMPLETE_EXPERIMENT_FAILURE, // encerra imediatamente, com falha (0%)
    COMPLETE_EXPERIMENT_STEP, //encerra imediatamente no passo atual, pode ou não resultar na falha (se Score < 60),
    BACK_TO_STEP, // retorna a um passo anterior
    BACK_TO_START, // retorna ao primeiro passo - equivale a BACK_TO_STEP com OnFailureReturnToStep=1
}