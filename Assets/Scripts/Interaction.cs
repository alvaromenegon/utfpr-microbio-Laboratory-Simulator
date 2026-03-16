using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour {
    [SerializeField] InteractionType interactionType;
    [SerializeField] GameObject target;
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