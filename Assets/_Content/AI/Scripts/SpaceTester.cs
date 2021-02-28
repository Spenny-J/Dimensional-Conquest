using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTester : MonoBehaviour
{
	private enum ActionType { Moving, Turning, Waiting };
	private ActionType currentAction;

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			int _randAction = Random.Range(0, System.Enum.GetValues(typeof(ActionType)).Length);
			currentAction = (ActionType)_randAction;
			Debug.Log(currentAction);
		}
	}
}
