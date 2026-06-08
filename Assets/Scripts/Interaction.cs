using UnityEngine;

public class Interaction : MonoBehaviour {

    public static bool RequireValue(InteractionType interactionType) {
        return interactionType == InteractionType.SET_TIMER ||
               interactionType == InteractionType.SET_TEMPERATURE ||
               interactionType == InteractionType.SET_QUANTITY ||
               interactionType == InteractionType.SET_PROPERTY;
    }

    public enum InteractionType {
        USE, // "USE"
        EXAMINE, // "EXAMINE"
        SET_TIMER, // "SET_TIMER"
        SET_TEMPERATURE, // "SET_TEMPERATURE"
        SET_QUANTITY, // "SET_QUANTITY"
        ACTIVATE,
        DEACTIVATE,
        SET_PROPERTY
    }
}