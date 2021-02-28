using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class AI_Controller : MonoBehaviour
{
	public GameObject SurviveTarget;
	public GameObject AttackTarget;
	public GameObject GoalTarget;

	private AI_Move myMove;

    // Start is called before the first frame update
    void Start()
    {
		if (myMove == null && GetComponent<AI_Move>() != null)
			myMove = GetComponent<AI_Move>();
    }

    // Update is called once per frame
    void Update()
    {
		if (SurviveTarget != null)
		{
			//Move to Survalist Target set by cover script or self preservation script
			//We are going to grab this object from the AI controller if not null
			return;
		}
		if (AttackTarget != null)
		{
			//Move to attack target set by the attack scripts
			//We are going to grab this object from the AI controller if not null
			return;
		}
		if (GoalTarget != null)
		{
			//Move to goal target
			//We are going to grab this object from the AI controller if not null
			return;
		}
		else
		{
			//Lets use the Roam script as placeholder for doing something when there aren't the other scripts there.
		}
	}
}
