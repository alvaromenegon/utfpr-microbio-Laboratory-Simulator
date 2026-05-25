using Assets.Scripts;
using UnityEngine;

public class MoveObjects : MonoBehaviour, IMovable {
	[Header("References")]
	[SerializeField] private Transform guide;
	[SerializeField] private LaboratoryObject laboratoryObject;

	private LayerMask originalLayer;

    GameObject _tempParent;
	Rigidbody _rigidbody;
    private bool _isMoving;
	private bool _canMove = true;

	public void SetCanMove(bool canMove) {
		_canMove = canMove;
    }

	private void Awake() {
		originalLayer = gameObject.layer;
	}

    public bool IsMoving => _isMoving;

	public LaboratoryObject GetLaboratoryObject() {
		if (laboratoryObject == null) {
			Debug.LogWarning("LaboratoryObject reference is not assigned, trying to find it in the game object...");
			laboratoryObject = gameObject.GetComponent<LaboratoryObject>();
			if (laboratoryObject == null) {
				Debug.LogError("LaboratoryObject is null!");
			}
        }
        return laboratoryObject;
    }

    void Start () {		
		_rigidbody = gameObject.GetComponent<Rigidbody>();
		_rigidbody.useGravity = true;
		_isMoving = false;
	}

	private void SetTempParent() {
		if (guide == null) {
			Debug.LogWarning("Guide is not assigned, trying to find it in the scene...");
			Debug.LogWarning("This is for compatibility and will be removed in the future. Please assign the guide reference in the inspector.");
			_tempParent = GameObject.Find("guide");
			if (_tempParent == null) {
				Debug.LogError("temp parent is null!");
			}
		}
		else {
			_tempParent = guide.gameObject;
		}
	}

	private void ResetTempParent() {
		_tempParent = null;
    }

	public void PickUp()
	{
        if (!_canMove) {
            return;
        }
        SetTempParent();
        _rigidbody.useGravity = false;
		_rigidbody.isKinematic = true;
		transform.position = _tempParent.transform.position;
        transform.parent = _tempParent.transform;
		_isMoving = true;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    public void Drop(){
        if (!_isMoving) {
			return;
		}
        _rigidbody.useGravity = true;
		_rigidbody.isKinematic = false;
		transform.parent = null;
		transform.position = _tempParent.transform.position;

        ResetTempParent();
		_isMoving = false;
		gameObject.layer = originalLayer;

    }

}
