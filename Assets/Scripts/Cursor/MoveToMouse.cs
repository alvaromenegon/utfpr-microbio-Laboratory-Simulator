using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToMouse : MonoBehaviour {
    void Update() {
        this.transform.position = Input.mousePosition;
    }
}
