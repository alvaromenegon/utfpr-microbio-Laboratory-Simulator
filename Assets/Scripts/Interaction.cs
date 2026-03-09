using UnityEngine;
using UnityEngine.UI;

public class Interaction : MonoBehaviour {
    [SerializeField] InteractionType interactionType;
    [SerializeField] GameObject target;



    enum InteractionType {
        None,
        StartAnimation,
        EndAnimation,
        SetValue
    }

}