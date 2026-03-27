using System;
using static Interaction;
using UnityEngine;

public class EquipmentInteractable : StepInteractable{
    private WorkingState workingState = WorkingState.Idle;
    private long timeUsed = 0;
    private long startTime; // tempo de início da utilização do equipamento, em milissegundos desde a época Unix
    private long endTime; // tempo de término da utilização do equipamento, em milissegundos desde a época Unix
    private float value; // valor obtido por entrada do usuário, como uma temperatura ou tempo.

    public enum WorkingState {
        Idle, // equipamento está disponível para uso
        InUse, // equipamento está sendo usado
        Ended // encerrou a utilização
    }

    public override void Interact() {
        if (workingState == WorkingState.Ended) {
            workingState = WorkingState.Idle; // na primeira interação após o uso, o equipamento volta a ficar disponível
        }
        if (stepType == InteractionType.USE) {
            if (workingState == WorkingState.Idle) {
                TurnOnEquipment();
            }
            else if (workingState == WorkingState.InUse) {
                TurnOffEquipment();
            }
        } else if (RequireValue(stepType)) {
            // tipo de interação que requer um valor obtido
            // por entrada do usuário, como uma temperatura ou tempo.
            InputPanelController.Instance.Show();
            return;
        }
    }

    public virtual void SetValue(float newValue) {
        value = newValue;
    }

    public virtual float GetValue() { return value; }

    public void TurnOnEquipment() {
        workingState = WorkingState.InUse;
        startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public void TurnOffEquipment() {
        workingState = WorkingState.Ended;
        endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        timeUsed = (endTime - startTime) / 1000; // tempo de uso em segundos
    }

     public long GetTimeUsed() {
        return timeUsed;
    }

}