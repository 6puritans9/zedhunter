using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
	[HideInInspector] public Vector3 dir;
	public float damageCooldown = 0.5f;  // ���ظ� �ٽ� �Ա������ �ּ� �ð� ����
	private float lastDamageTime = -1;   // ���������� ���ظ� ���� �ð�
	private void OnTriggerEnter(Collider other)
	{
		if (Time.time - lastDamageTime < damageCooldown) return;  // ��ٿ� ������ ���ظ� ���� ����
		if (other.gameObject.GetComponentInParent<EnemyHealth>())
		{
			EnemyHealth enemyHealth = other.gameObject.GetComponentInParent<EnemyHealth>();
			enemyHealth.TakeDamage(25);
			lastDamageTime = Time.time;  // ���ظ� ���� �ð� ������Ʈ
			Debug.Log("����!!");
			if (enemyHealth.health <= 0 && !enemyHealth.isDead)
			{
				Rigidbody rb = other.gameObject.GetComponentInChildren<Rigidbody>();
				Vector3 forceDirection = (other.transform.position - transform.position).normalized;
				rb.AddForce(-forceDirection * 1000, ForceMode.Impulse); // ���� ũ�⸦ ������ �� ����

				enemyHealth.isDead = true;
			}

		}
	}
	
}
