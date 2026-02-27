using UnityEngine;

public class MoveObjects : MonoBehaviour {

	public GameObject items;
	public GameObject tempParent;
	public Transform guide;

	private GameControls _controls;

	// Use this for initialization
	void Start () {
		items.GetComponent<Rigidbody> ().useGravity = true;
	}

	void Awake()
	{
		_controls = new GameControls();
		_controls.Player.Interact.performed += _ => clickObject();
		_controls.Player.Interact.canceled += _ => letGoObject();
	}

	void OnEnable() => _controls.Player.Enable();
	void OnDisable() => _controls.Player.Disable();

	public void clickObject()
	{
		items.GetComponent<Rigidbody> ().useGravity = false;
		items.GetComponent<Rigidbody> ().isKinematic = true;
		items.transform.position = guide.transform.position;
		//items.transform.rotation = guide.transform.rotation;
		items.transform.parent = tempParent.transform;
	}

	public void letGoObject()
	{
		items.GetComponent<Rigidbody> ().useGravity = true;
		items.GetComponent<Rigidbody> ().isKinematic = false;
		items.transform.parent = null;
		items.transform.position = guide.transform.position;
	}
}
