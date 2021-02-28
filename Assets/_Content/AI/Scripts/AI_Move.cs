using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

[RequireComponent(typeof(AI_Controller))]
public class AI_Move : MonoBehaviour
{
	private AI_Controller myController;
	private CharacterController myCharCntrlr;
	private GameObject me;
	[SerializeField] private const float WalkSpeed = 1f;


    // Start is called before the first frame update
    void Start()
    {
		myController = gameObject.GetComponent<AI_Controller>();
		me = gameObject;
    }

	private void OnEnable()
	{
		if(myController == null)
			myController = gameObject.GetComponent<AI_Controller>();
	}

	public void MoveTo(GameObject _target, GameObject _thisObject, float _movementSpeed = WalkSpeed)
	{
		if (_thisObject == null)
			_thisObject = gameObject;
		Vector3 _direction = _target.transform.position - transform.position;
		_thisObject.GetComponent<CharacterController>().Move(_direction * _movementSpeed);

	}
}
