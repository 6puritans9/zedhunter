using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDamage : MonoBehaviour
{
	public bool groundCheck;
	public bool downDamage;

	private void OnTriggerEnter(Collider other)
	{
		if(downDamage && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			if(TryGetComponent(out EnemyHealth enemyHealth)) 
			{
				enemyHealth.TakeDamage(500);
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
			groundCheck = true;
	}

	private void Update()
	{
		if (!groundCheck)
			downDamage = true;
		else downDamage = false;
	}
}
