using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

/*
 * TODO 
 * Only attack enemies of specified type in the AI_LIB.
 */
 [RequireComponent(typeof(Character_ID))]
public class DetectEnemies : MonoBehaviour
{
	//Determines at what angle that enemies will be seen.
	public float AngleOfDetection = 25f;

	//Since we are using a detection trigger as a child object I will start with that.
	private GameObject triggerObj;

	[SerializeField] private List<GameObject> EntitiesInTrigger = new List<GameObject>();
	public List<GameObject> EntitiesSeen = new List<GameObject>();

	[Tooltip("This is used to determine what to attack")]
	public AI_LIB.Faction EnemyFaction; 

	void OnStart()
	{
		Invoke("OnEnable", 0f);
		triggerObj = gameObject.transform.GetChild(0).gameObject;
	}

	private void FixedUpdate()
	{
		if (EntitiesInTrigger.Count > 0)
		{
			foreach (var entity in EntitiesInTrigger)
			{
				if(!entity.activeSelf)
				{
					EntitiesInTrigger.Remove(entity);
					if (EntitiesSeen.Contains(entity))
						EntitiesSeen.Remove(entity);
					return;
				}
				//Entity is our target in each scenario
				Vector3 targetDir = entity.transform.position - transform.position;
				float angle = Vector3.Angle(targetDir, transform.forward);
				if (angle <= AngleOfDetection)
				{
					//Raycast to see if entitiy is behind cover/wall/or another object and can actually be seen
					RaycastHit hit;
					Vector3 fromPosition = transform.position;
					Vector3 toPosition = entity.transform.position;
					Vector3 rayDirection = toPosition - fromPosition;
					Ray sightRay = new Ray(transform.position, rayDirection);
					if(Physics.Raycast(sightRay, out hit, 50f))
					{
						//Debug.DrawRay(transform.position, rayDirection, Color.blue, 1.5f);
						if (hit.transform.gameObject == entity && !EntitiesSeen.Contains(entity))
						{
							EntitiesSeen.Add(entity);
						}
						if(hit.transform.gameObject != entity && EntitiesSeen.Contains(entity))
						{
							EntitiesSeen.Remove(entity);
						}
					}

				}
				if (angle > AngleOfDetection && EntitiesSeen.Contains(entity))
				{
					EntitiesSeen.Remove(entity);
				}

			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.GetComponent<Character_ID>() != null && other.GetComponent<Character_ID>().Affiliation == EnemyFaction)
			EntitiesInTrigger.Add(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
		EntitiesInTrigger.Remove(other.gameObject);
	}
}
