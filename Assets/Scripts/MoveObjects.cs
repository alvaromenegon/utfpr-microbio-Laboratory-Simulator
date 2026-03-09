using UnityEngine;

public class MoveObjects : MonoBehaviour {
	GameObject _tempParent;

	private GameControls _controls;
	private bool _isHovering;
	private bool _isMoving;

	// Use this for initialization
	void Start () {		
		gameObject.GetComponent<Rigidbody> ().useGravity = true;
	}

	void Awake()
	{
		_controls = new GameControls();
		_controls.Player.Click.performed += _ => PickObject();
		_controls.Player.Click.canceled += _ => DropObject();
		_isMoving = false;
		_isHovering = false;
	}

	void OnEnable() => _controls.Player.Enable();

	void OnDisable() => _controls.Player.Disable();

    void OnMouseEnter() {
		_isHovering = true;
	}
	void OnMouseExit() {
		_isHovering = false;
	}

	private void SetTempParent(string parentName= null) {
		if (parentName == null) {
			_tempParent = null;
			return;
		}
        _tempParent = GameObject.Find(parentName);
        if (_tempParent == null) {
            Debug.LogError("temp parent is null!");
        }
    }

	public void PickObject()
	{
        if (!_isHovering) {
			return;
        }
        SetTempParent("guide");
        GetComponent<Rigidbody> ().useGravity = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		transform.position = _tempParent.transform.position;
        transform.parent = _tempParent.transform;
		_isMoving = true;
	}

	public void DropObject(){
        if (!_isMoving) {
			return;
		}
        GetComponent<Rigidbody> ().useGravity = true;
		GetComponent<Rigidbody> ().isKinematic = false;
		transform.parent = null;
		transform.position = _tempParent.transform.position;

        SetTempParent();
		_isMoving = false;
    }
}
