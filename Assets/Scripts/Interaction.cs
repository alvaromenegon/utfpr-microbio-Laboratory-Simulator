using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour {
    InteractionType interactionType;

    public static bool RequireValue(InteractionType interactionType) {
        return interactionType == InteractionType.SET_TIMER ||
               interactionType == InteractionType.SET_TEMPERATURE ||
               interactionType == InteractionType.SET_QUANTITY;
    }



    public enum InteractionType {
        PICK_UP, // "PICK_UP"
        USE, // "USE"
        EXAMINE, // "EXAMINE"
        MOVE, // "MOVE"
        SET_TIMER, // "SET_TIMER"
        SET_TEMPERATURE, // "SET_TEMPERATURE"
        SET_QUANTITY, // "SET_QUANTITY"
    }
}