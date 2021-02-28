using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

	public int StartingHealth = 10;
	private int CurrentHealth;

	private void OnStart()
	{
		Invoke("OnEnable", 0f);
	}

	private void OnEnable()
	{
		CurrentHealth = StartingHealth;
	}

	public void AddDamage(int _damage)
	{
		CurrentHealth -= _damage;
		CheckLife();
	}

	public void AddHealth(int _health)
	{
		CurrentHealth += _health;
		CheckLife();
	}

	public int CheckLife()
	{
		if (CurrentHealth <= 0)
			gameObject.SetActive(false);
		return CurrentHealth;
	}
}
