using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTrigger : MonoBehaviour {
    public CursorChanger.CursorType SetToThisCursor = CursorChanger.CursorType.Pointer;

    CursorChanger _cursorChanger;

    public void Start() {
        _cursorChanger = GameObject.Find("CursorChanger").GetComponent<CursorChanger>();
    }
    private void OnMouseEnter() {
        _cursorChanger.ChangeTo(SetToThisCursor);
    }

    private void OnMouseExit() {
        _cursorChanger.ChangeTo(CursorChanger.CursorType.Pointer);
    }
}
