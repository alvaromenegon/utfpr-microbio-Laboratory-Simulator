using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorTrigger : MonoBehaviour {
    public CursorChanger.CursorType SetToThisCursor = CursorChanger.CursorType.Pointer;

    GameObject _tooltip;

    CursorChanger _cursorChanger;

    public void Start() {
        _cursorChanger = GameObject.Find("CursorChanger").GetComponent<CursorChanger>();
        _tooltip = GameObject.Find("Player Tooltip");
    }
    private void OnMouseEnter() {
        _tooltip.GetComponent<Text>().text = "Press E to interact";
        _cursorChanger.ChangeTo(SetToThisCursor);
        //gameObject.GetComponent<MoveObjects>();
    }

    private void OnMouseExit() {
        _tooltip.GetComponent<Text>().text = "";
        _cursorChanger.ChangeTo(CursorChanger.CursorType.Pointer);
    }
}
