using System;
using static Interaction;
using UnityEngine;

public class EquipmentInteractable : StepInteractable{
    private WorkingState workingState = WorkingState.Idle;
    private long timeUsed = 0;
    private long startTime; // tempo de início da utilização do equipamento, em milissegundos desde a época Unix
    private long endTime; // tempo de término da utilização do equipamento, em milissegundos desde a época Unix
    private float value; // valor obtido por entrada do usuário, como uma temperatura ou tempo.
    public BackgroundProcess backgroundProcess; // o processo em segundo plano a ser iniciado em caso de sucesso (se aplicável)
    public GameObject retainedObject; // objeto que será mantido no equipamento durante a interação, se aplicável (ex: um item mantido dentro da estufa enquanto ela está ligada)

    public enum WorkingState {
        Idle, // equipamento está disponível para uso
        InUse, // equipamento está sendo usado
        Ended // encerrou a utilização
    }

    public void MoveObjectToEquipment(GameObject obj) {
        Debug.Log($"Attempting to move object {obj.name} to equipment {gameObject.name}");
        if (retainedObject != null) {
            Debug.LogWarning("Equipment already has a retained object. Cannot move another object to it.");
            return;
        }
        obj.transform.SetParent(transform);
        obj.transform.localPosition = Vector3.zero; // opcional: centraliza o objeto no equipamento
        retainedObject = obj;
    }

    public void RemoveObjectFromEquipment() {
        if (retainedObject == null) {
            Debug.LogWarning("No retained object to remove from equipment.");
            return;
        }
        retainedObject.transform.SetParent(null); // desanexa o objeto do equipamento
        retainedObject = null;
    }

    public override void Interact() {
        if (workingState == WorkingState.Ended) {
            workingState = WorkingState.Idle; // na primeira interação após o uso, o equipamento volta a ficar disponível
        }
        if (stepType == InteractionType.USE) {
            if (workingState == WorkingState.Idle) {
                TurnOnEquipment();
                PlayerAction action = new () {
                    InteractionType = stepType,
                    Target = gameObject,
                };
                ExperimentManager.Instance.ProcessInteraction(action);
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
        backgroundProcess?.Start();
        if (retainedObject != null) {
            if (retainedObject.TryGetComponent<MoveObjects>(out var move)) {
                move.SetCanMove(false);
            }
        }
    }

    public void TurnOffEquipment() {
        workingState = WorkingState.Ended;
        endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        timeUsed = (endTime - startTime) / 1000; // tempo de uso em segundos
        backgroundProcess?.Stop();
        if (retainedObject != null) {
            if (retainedObject.TryGetComponent<MoveObjects>(out var move)) {
                move.SetCanMove(true);
            }
        }
        ExperimentManager.Instance.ProcessBackgroundWork(backgroundProcess, timeUsed);
    }

     public long GetTimeUsed() {
        return timeUsed;
     }

    public override void OnHover() {
        Debug.Log($"Hovering over equipment {gameObject.name} with state {workingState}");
        HUDController.Instance.UpdateObjectName(GetObjectName());
        if (workingState == WorkingState.InUse) {
            HUDController.Instance.UpdateInfo($"Equipamento {GetObjectName()} em uso por {timeUsed} segundos");
        }
    }

    public override void OnExitHover() {
        HUDController.Instance.UpdateObjectName("");
            if (workingState == WorkingState.InUse) {
                HUDController.Instance.UpdateInfo("");
        }
    }

    public void Update() {
        if (workingState == WorkingState.InUse) {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            timeUsed = (currentTime - startTime) / 1000; // atualiza o tempo de uso em segundos
        }
    }

}