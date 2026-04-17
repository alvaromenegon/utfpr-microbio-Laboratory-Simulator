using System;
using UnityEngine;
using static Interaction;

[Serializable]
public class ExperimentData : ScriptableObject {
    public Step[] Steps { get; set; }
    public BackgroundProcess[] BackgroundProcesses { get; set; }
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
    /**
     * O nome do segundo GameObject com o qual o usuário deve interagir, se aplicável (ex: para interações que envolvem dois objetos, como "USE A ON B")
     * 
     * Exemplo: Interaction: "MOVE",
     * InteractionTarget: "flask",
     * InteractionTargetSecondary: "cupboard"
     * O passo será considerado válido se o usuário mover a "flask" para dentro do "cupboard"
     * Na cena, o secondaryTargetBeam (antigo teleportBeam) será exibido em volta do "cupboard"
     */
    public string InteractionTargetSecondary { get; set; }
    public string Instruction { get; set; } // Instrução detalhada para o passo
    public double? Value { get; set; } // Valor associado à interação, se aplicável (ex: temperatura, tempo, quantidade)
    public double? Tolerance { get; set; } // Tolerância para validação do valor 
    public string OnSuccessMessage { get; set; } // Mensagem ou ação a ser executada em caso de sucesso
    public string OnFailureMessage { get; set; } // Mensagem ou ação a ser executada em caso de falha
    public OnStepFinished ActionOnFailure { get; set; } // Ação a ser executada em caso de falha
    public int OnFailureReturnToStep { get; set; } // Passo para o qual o usuário deve retornar em caso de falha (se aplicável)
    public OnStepFinished ActionOnSuccess { get; set; } // Ação a ser executada em caso de sucesso
    public string OnSuccessStartBackgroundWorkId { get; set; } // Id do processo em segundo plano a ser iniciado em caso de sucesso (se aplicável)

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

[Serializable]
/**
 * Classe que representa ações que ocorrem em segundo plano durante a execução do experimento.
 * Por exemplo, um equipamento que precisa ser mantido ligado por um determinado tempo, 
 * ou um processo que precisa ocorrer enquanto o usuário realiza os passos do experimento.
 * Isso permite que o usuário realize outras ações no experimento enquanto o processo em segundo plano ocorre, 
 * sem precisar esperar o processo terminar para avançar para o próximo passo.
 */
public class BackgroundProcess {
    public string Id { get; set; }
    public string Description { get; set; } // Descrição do processo em segundo plano
    public int Duration { get; set; } // Duração do processo em segundos
    public int? Tolerance { get; set; } // Tolerância para validação do processo em segundos
    public string OnSuccessMessage { get; set; } // Mensagem ou ação a ser executada em caso de sucesso
    public string OnFailureMessage { get; set; } // Mensagem ou ação a ser executada em caso de falha
    public OnStepFinished ActionOnFailure { get; set; } // Ação a ser executada em caso de falha
    public int OnFailureReturnToStep { get; set; } // Passo para o qual o usuário deve retornar em caso de falha (se aplicável)
    public int Score { get; set; } // Pontuação associada ao processo em segundo plano, se aplicável
    public int?[] ShouldCompleteBetweenSteps { get; set; } // Índices dos passos entre os quais o processo em segundo plano deve ser completado (se aplicável)

    public bool IsRunning { get; private set; } = false;

    public void Start() {
        if (IsRunning) {
            Debug.LogWarning($"BackgroundProcess {Id} is already running.");
            return;
        }
        IsRunning = true;
        // Lógica para iniciar o processo em segundo plano (ex: ligar um equipamento, iniciar uma contagem de tempo, etc.)
        Debug.Log($"Started BackgroundProcess: {Description}");
    }

    public void Stop() {
        if (!IsRunning) {
            Debug.LogWarning($"BackgroundProcess {Id} is not running.");
            return;
        }
        IsRunning = false;
        // Lógica para parar o processo em segundo plano (ex: desligar um equipamento, parar uma contagem de tempo, etc.)
        Debug.Log($"Stopped BackgroundProcess: {Description}");
    }

    /// <summary>
    /// Valida se o processo em segundo plano pode ser considerado completo, verificando se ele foi parado e se está dentro do intervalo de passos permitido (se aplicável).
    /// Se o indice 0 for null, considera que pode ser completado a qualquer momento, mas não pode ser completado antes do passo definido no indice 1
    /// Se o índice 0 não for null, mas o índice 1 for null, considera que deve ser completado antes do passo definido no índice 0, mas pode ser completado a qualquer momento antes disso.
    /// Se ambos os índices forem null, considera que pode ser completado a qualquer momento durante o experimento.
    /// Se ambos os índices forem não nulos, considera que deve ser completado entre os passos definidos pelos índices (inclusive).
    /// Isso significa que, por exemplo, se ShouldCompleteBetweenSteps for [2, 4], o processo em segundo plano deve ser completado entre os passos 2 e 4 (inclusive), ou seja, não pode ser completado antes do passo 2, e deve ser completado até o final do passo 4.
    /// </summary>
    /// <param name="currentStepIndex">Índice do passo atual do experimento.</param>
    /// <returns>Retorna true se o processo em segundo plano pode ser considerado completo, caso contrário, retorna false.</returns>
    public bool ValidateCompletion(int currentStepIndex) {
        if (IsRunning) {
            Debug.LogWarning($"BackgroundProcess {Id} is still running. Cannot validate completion.");
            return false;
        }
        Debug.Log($"Validating completion of BackgroundProcess {Id} at step index {currentStepIndex}. ShouldCompleteBetweenSteps: [{(ShouldCompleteBetweenSteps != null && ShouldCompleteBetweenSteps.Length > 0 ? ShouldCompleteBetweenSteps[0]?.ToString() ?? "null" : "null")}, {(ShouldCompleteBetweenSteps != null && ShouldCompleteBetweenSteps.Length > 1 ? ShouldCompleteBetweenSteps[1]?.ToString() ?? "null" : "null")}]");

        if (ShouldCompleteBetweenSteps == null || ShouldCompleteBetweenSteps.Length == 0) {
            // Pode ser completado a qualquer momento
            return true;
        }
        if (ShouldCompleteBetweenSteps[0] == null && ShouldCompleteBetweenSteps[1] != null) {
            // Deve ser completado antes do passo definido no índice 1, mas pode ser completado a qualquer momento antes disso
            return currentStepIndex <= ShouldCompleteBetweenSteps[1];
        }
        if (ShouldCompleteBetweenSteps[0] != null && ShouldCompleteBetweenSteps[1] == null) {
            // Deve ser completado depois do passo definido no índice 0, mas pode ser completado a qualquer momento depois disso
            return currentStepIndex >= ShouldCompleteBetweenSteps[0];
        }
        if (ShouldCompleteBetweenSteps[0] != null && ShouldCompleteBetweenSteps[1] != null) {
            // Deve ser completado entre os passos definidos pelos índices (inclusive)
            return currentStepIndex >= ShouldCompleteBetweenSteps[0] && currentStepIndex <= ShouldCompleteBetweenSteps[1];
        }   
        return true;

    }

    public bool IsValid() {
        if (string.IsNullOrEmpty(Id)) {
            Debug.LogError("BackgroundProcess Id is null or empty.");
            return false;
        }
        if (string.IsNullOrEmpty(Description)) {
            Debug.LogError("BackgroundProcess description is null or empty.");
            return false;
        }
        if (Duration <= 0) {
            Debug.LogError("BackgroundProcess duration must be greater than zero.");
            return false;
        }
        return true;
    }

}

public enum OnStepFinished {
   DEFAULT, // segue o fluxo normal do experimento (avança para o próximo passo, ou encerra com sucesso se for o último passo)
    COMPLETE_EXPERIMENT_SUCCESS, //completa imediatamente, com sucesso (100%)
    COMPLETE_EXPERIMENT_FAILURE, // encerra imediatamente, com falha (0%)
    COMPLETE_EXPERIMENT_STEP, //encerra imediatamente no passo atual, pode ou não resultar na falha (se Score < 60),
    BACK_TO_STEP, // retorna a um passo anterior
    BACK_TO_START, // retorna ao primeiro passo - equivale a BACK_TO_STEP com OnFailureReturnToStep=1
    START_BACKGROUND_WORK, // inicia um processo em segundo plano e avança para o próximo passo se houver. 
}