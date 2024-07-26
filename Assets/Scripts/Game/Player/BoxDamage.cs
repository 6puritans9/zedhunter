using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoxDamage : MonoBehaviour
{
	WallHP wallHp;
	NavMeshObstacle obstacle;

	public bool groundCheck;
	public bool downDamage = true;
	public float explosionForce = 1000f;
	public float explosionRadius = 5f;

	private void Awake()
	{
		wallHp = GetComponent<WallHP>();
		obstacle = GetComponent<NavMeshObstacle>();
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			groundCheck = true;
			obstacle.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!groundCheck && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			if (collision.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
			{
				enemyHealth.TakeDamage(500);
			}
		}
	}

	private void Update()
	{
		if (wallHp.canAttack)
			ApplyExplosionForce();
	}


	private void ApplyExplosionForce()
	{
		//폭발 데미지, 밀어내기
		Collider[] colliders = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Enemy"));
		foreach (Collider collider in colliders)
		{
			if (collider.TryGetComponent(out EnemyHealth enemyHealth))
			{
				enemyHealth.TakeDamage(300);
			}
			if (collider.TryGetComponent(out Rigidbody rb))
			{
				rb.AddExplosionForce(300, transform.position, 10);
				/*Vector3 explosionDirection = collider.transform.position - transform.position;
				Vector3 forceDirection = -explosionDirection.normalized;
				rb.AddForce(forceDirection * 1000f, ForceMode.Impulse);*/
			}
		}
	}
}
