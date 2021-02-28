
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
/*
* - Attacking an enemy (melee)
*		Attacking preference between (WeakestEnemy, ClosestEnemy, StrongestEnemy)
*		Attacking style between (Engage till dead, Engage on hurt, Engage closest)
*/
[RequireComponent(typeof(DetectEnemies))]
public class AttackEnemy : MonoBehaviour
{

	[Tooltip("This is for preference on who the entity should engage")]
	public AI_LIB.AttackPreference myAttackPreference = AI_LIB.AttackPreference.Closest;
	[Tooltip("This is for how an entity will engage an enemy")]
	public AI_LIB.AttackStyle myAttackStyle = AI_LIB.AttackStyle.UntilDead;

	private DetectEnemies myDetectEnemies;
	private CharacterController myChar;
	[SerializeField] float distanceToATK = 1.8f;
	[SerializeField] float turningSpeed = 1f;
	[SerializeField] float movementSpeed = 1f;
	[SerializeField] float attackCoolDown = .25f;
	private float timeWaiting = 0;
	private bool attacking = false;

	private GameObject Target; //What we currently are engaging

	private void Start()
	{
		if (gameObject.GetComponent<DetectEnemies>() != null)
			myDetectEnemies = gameObject.GetComponent<DetectEnemies>();
		else
			Debug.LogError(gameObject.name + " IS NOT CONIFGURED PROPERLY. Needs a DetectEnemies script.");
		if (gameObject.GetComponent<CharacterController>() != null)
			myChar = gameObject.GetComponent<CharacterController>();
		else
			Debug.LogError(gameObject.name + " IS NOT CONFIGURED PROPERLY. If you don't have a character controller on here what the fuck are you doing?");

		//Invoke("OnEnable", 0);
	}

	private void OnDisable()
	{
		timeWaiting = 0f;
	}

	private void Update()
	{
		if (Target != null)
		{
			if (attacking == false)
			{
				ChooseTarget();
			}
			MeleeAttack(Target);
		}
		else
		{
			ChooseTarget();
		}
	}

	private void Move(GameObject _target) //Move towards target
	{
		Vector3 _direction = _target.transform.position - transform.position;
		float _speed = movementSpeed * Time.deltaTime;
		myChar.Move(_direction * _speed);
		//Vector3.RotateTowards(transform.position, _direction, 180, 180);
		float singleStep = turningSpeed * Time.deltaTime;
		Vector3 newDirection = Vector3.RotateTowards(transform.forward, _direction, singleStep, 0.0f);
		transform.rotation = Quaternion.LookRotation(newDirection);
	}

	private void ChooseTarget() //This for determing (using preferences above) which target should be chosen
	{
		switch (myAttackPreference)
		{
			case AI_LIB.AttackPreference.Closest:
				Target = FindClosest();
				break;
			case AI_LIB.AttackPreference.Weakest:
				Target = FindWeakest();
				break;
			case AI_LIB.AttackPreference.Strongest:
				Target = FindStrongest();
				break;
			default:
				Debug.LogError("You dun fucked up?" + gameObject.name);
				break;
		}
	}

	private void MeleeAttack(GameObject _target) //Attempt to deal damage to this target passed in
	{
		if (_target != null && !_target.activeSelf)
		{
			Target = null;
			attacking = false;
			timeWaiting = 0f;
			return;
		}
		timeWaiting -= Time.deltaTime;
		if (_target != null)
		{
			if (Vector3.Distance(_target.transform.position, transform.position) > distanceToATK && timeWaiting <= 0.0f)
			{
				Move(_target); //Move to target
			}
			else
			{

				if (timeWaiting <= 0.0f)
				{
					_target.GetComponent<Health>().AddDamage(1);
					timeWaiting = attackCoolDown;
					attacking = true;
					if (Target.GetComponent<AttackEnemy>() != null && Target.GetComponent<AttackEnemy>().myAttackStyle == AI_LIB.AttackStyle.OnDamaged) //If I damage an enemy set their target to me
						Target.GetComponent<AttackEnemy>().ChangeTarget(gameObject);
					if (Target.GetComponent<AttackEnemy>() != null && myAttackStyle == AI_LIB.AttackStyle.Proximity) //If I see someone closer when attacking, set them as my target
						ChangeTarget(null);
				}
				else
				{
					Vector3 _direction = _target.transform.position - transform.position;
					float _speed = movementSpeed * Time.deltaTime;
					myChar.Move(-_direction * _speed); //Simulate the character moving back so we can see whats going on
				}
			}
		}	
	}

	private void ChangeTarget(GameObject _target) //Change active target which can be called based on what the attack style is (If user is attacked by someone else)
	{
		switch (myAttackStyle)
		{
			case AI_LIB.AttackStyle.OnDamaged: //When the person is damaged (make ugly call) to AttackEnemy to attack me
				Target = _target;
				break;
			case AI_LIB.AttackStyle.Proximity:
				Target = FindClosest();
				break;
			default:
				break;
		}
	}

	private GameObject FindWeakest()
	{
		if (myDetectEnemies.EntitiesSeen.Count > 0)
		{
			int maxHealth = 10000;
			GameObject _enemy = null; //Set to null just because target will not like it
			foreach (var enemy in myDetectEnemies.EntitiesSeen)
			{
				int enemyHealth = enemy.GetComponent<Health>().CheckLife();
				if (enemyHealth < maxHealth)
				{
					maxHealth = enemyHealth;
					_enemy = enemy;
				}
			}
			return _enemy;
		}
		else
		{
			return null;
		}
	}

	private GameObject FindClosest()
	{
		if (myDetectEnemies.EntitiesSeen.Count > 0)
		{
			float closestDist = 1000f;
			GameObject _enemy = null; //Set to null just because target will not like it
			foreach (var enemy in myDetectEnemies.EntitiesSeen)
			{
				float enemyDist = Vector3.Distance(enemy.transform.position, transform.position);
				if (enemyDist < closestDist)
				{
					closestDist = enemyDist;
					_enemy = enemy;
				}
			}
			return _enemy;
		}
		else
		{
			return null;
		}
	}

	private GameObject FindStrongest()
	{
		if (myDetectEnemies.EntitiesSeen.Count > 0)
		{
			int maxHealth = 0;
			GameObject _enemy = null; //Set to null just because target will not like it
			foreach (var enemy in myDetectEnemies.EntitiesSeen)
			{
				int enemyHealth = enemy.GetComponent<Health>().CheckLife();
				if (enemyHealth > maxHealth)
				{
					maxHealth = enemyHealth;
					_enemy = enemy;
					print(_enemy);
				}
			}
			return _enemy;
		}
		else
		{
			return null;
		}
	}


	
}
