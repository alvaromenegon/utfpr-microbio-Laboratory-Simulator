using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour {
    [SerializeField] GameObject Pointer;
    [SerializeField] GameObject Selector;
    [SerializeField] GameObject Interact;
    [SerializeField] GameObject NoGo;

    public enum CursorType {  
        Pointer,
        Selector,
        Interact,
        NoGo
    }

    public void ChangeTo(CursorType selectedCursor) {

        DeactivateAllCursors();

        switch (selectedCursor) {

            case CursorType.Pointer:
                Pointer.SetActive(true);
                break;

            case CursorType.Selector:
                Selector.SetActive(true);
                break;
            case CursorType.Interact:
                Interact.SetActive(true);
                break;
            case CursorType.NoGo:
                NoGo.SetActive(true);
                break;
            default:
                Debug.LogError("Invalid cursor type selected: " + selectedCursor);
                break;

        }
    }

    private void DeactivateAllCursors() {

        Pointer.SetActive(false);
        Selector.SetActive(false);
        NoGo.SetActive(false);
        Interact.SetActive(false);
    }

}
