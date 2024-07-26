using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadShotDamage : MonoBehaviour
{
    [HideInInspector]public EnemyHealth enemyHealth;

	private void Awake()
	{
		enemyHealth = GetComponentInParent<EnemyHealth>();
	}

	public void TakeHeadShotDamage(float Damage)
	{
		enemyHealth.TakeDamage(Damage);
	}
}
