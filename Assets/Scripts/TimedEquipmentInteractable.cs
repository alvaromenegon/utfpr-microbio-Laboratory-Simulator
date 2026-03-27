using System;
using static Interaction;
using UnityEngine;

// Subclasse de EquipmentInteractable, com a funcionalidade de "timer" definido,
// que "desliga" o equipamento após um tempo definido, ou quando o usuário clicar novamente para "desligar" o equipamento.
public class TimedEquipmentInteractable : EquipmentInteractable{
    public long timerDurationSeconds = 0; // duração do timer em segundos

    private void Update() {
        if (timerDurationSeconds > 0) {
            timerDurationSeconds -= (long)(Time.deltaTime);
            if (timerDurationSeconds <= 0) {
                timerDurationSeconds = 0;
                TurnOffEquipment();
            }
        }
    }

}