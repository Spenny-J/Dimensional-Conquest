using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

/* TODO
 * Move foward for a random amount of time based on a min and max
 * Stop for a random amount of time
 * Turn a random direction
 */ 

public class Roam : MonoBehaviour
{
	
	private AI_LIB.ActionType currentAction;
	private bool DoingAction;
	private CharacterController myChar;
	//[SerializeField] private float speed = .5f;

	//Move Variables
	[SerializeField] private float minDistance = 5f;
	[SerializeField] private float maxDistance = 20f;
	private float timeMoving;
	private bool resetMoving = true;
	private Vector3 moveDirection = Vector3.zero;

	//Turning Variables
	[SerializeField] private float turnAngle;
	[SerializeField] private float turnSpeed = 1;
	private bool resetTurning = true;


	//Waiting variables
	[SerializeField] private float maxWait = 10f;
	private float timeWaiting;
	private bool resetWaiting = true;


	private void Start()
	{
		if(gameObject.GetComponent<CharacterController>() != null)
			myChar = gameObject.GetComponent<CharacterController>();
	}

	private void FixedUpdate()
	{
		Movement();
	}

	public void Movement()
	{
		if(!DoingAction)
		{
			DoingAction = true;
			int _randAction = Random.Range(0, System.Enum.GetValues(typeof(AI_LIB.ActionType)).Length);
			currentAction = (AI_LIB.ActionType)_randAction;
			Debug.Log("Action is: " + currentAction);
		}
		//Loop movement, turn, and wait
		switch (currentAction)
		{
			case AI_LIB.ActionType.Moving:
				Move();
				break;
			case AI_LIB.ActionType.Turning:
				Turn();
				break;
			case AI_LIB.ActionType.Waiting:
				Wait();
				break;
			default:
				Debug.LogError(gameObject.name + "Has somehow hit the default in the Roaming Movement switch");
				break;
		}
	}

	void Wait()
	{
		if (!DoingAction)
			return;

		if(resetWaiting)
		{
			resetWaiting = false;
			timeWaiting = Random.Range(0f, maxWait);
		}

		timeWaiting -= Time.deltaTime;
		if(timeWaiting <= 0f)
		{
			//Stop waiting move on
			DoingAction = false;
			resetWaiting = true;

		}
	}

	void Move()
	{
		if (!DoingAction)
			return;

		if (resetMoving)
		{
			resetMoving = false;
			timeMoving = Random.Range(minDistance, maxDistance);
		}
		timeMoving -= Time.deltaTime;
		myChar.Move(transform.TransformDirection(Vector3.forward) * Time.deltaTime);
		if(timeMoving <= 0f)
		{
			DoingAction = false;
			resetMoving = true;
		}
	}

	void Turn()
	{
		if (!DoingAction)
			return;
		if (resetTurning)
		{
			resetTurning = false;
			turnAngle = Random.Range(0f, 360f);
			turnAngle = turnAngle * 100;
			turnAngle = Mathf.RoundToInt(turnAngle);
			turnAngle /= 100;
		}
		gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, Quaternion.Euler(gameObject.transform.eulerAngles.x, turnAngle, gameObject.transform.eulerAngles.z), turnSpeed * Time.deltaTime);
		float _currentY = gameObject.transform.eulerAngles.y;
		_currentY = _currentY * 100;
		_currentY = Mathf.Round(_currentY);
		_currentY /= 100;
		if(_currentY == turnAngle)
		{
			DoingAction = false;
			resetTurning = true;
		}

	}


}
