using UnityEngine;

public class ControllerMovement : MonoBehaviour {
	
	public float MovementSpeed;	 
	public GameObject player;

	private GameControls _controls;
	private bool _isMoving;

	void Awake()
	{
		_controls = new GameControls();
		_controls.Player.Interact.performed += _ => _isMoving = true;
		_controls.Player.Interact.canceled += _ => _isMoving = false;
	}

	void OnEnable() => _controls.Player.Enable();
	void OnDisable() => _controls.Player.Disable();
		
	void Update () {

		if(_isMoving)
		{
			player.transform.position += transform.forward * Time.deltaTime * MovementSpeed;
		}
	}
}
