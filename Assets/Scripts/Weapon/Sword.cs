using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
	[HideInInspector] public Vector3 dir;
	public float damageCooldown = 0.5f;  // 피해를 다시 입기까지의 최소 시간 간격
	private float lastDamageTime = -1;   // 마지막으로 피해를 입은 시간
	private void OnTriggerEnter(Collider other)
	{
		if (Time.time - lastDamageTime < damageCooldown) return;  // 쿨다운 동안은 피해를 입지 않음
		if (other.gameObject.GetComponentInParent<EnemyHealth>())
		{
			EnemyHealth enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
			enemyHealth.TakeDamage(25);
			lastDamageTime = Time.time;  // 피해를 입은 시간 업데이트
			Debug.Log("맞음!!");
			if (enemyHealth.health <= 0 && !enemyHealth.isDead)
			{
				Rigidbody rb = other.gameObject.GetComponentInChildren<Rigidbody>();
				Vector3 forceDirection = (other.transform.position - transform.position).normalized;
				rb.AddForce(-forceDirection * 1000, ForceMode.Impulse); // 힘의 크기를 조절할 수 있음

				enemyHealth.isDead = true;
			}

		}
	}
	
}
