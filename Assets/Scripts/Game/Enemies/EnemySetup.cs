using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySetup : MonoBehaviour
{
	EnemyHealth enemyHealth;
	//public EnemySpawnPool enemySpawnPool;
	private NavMeshAgent agent;
	private Vector3 saveDestination;

	private void Awake()
	{
		enemyHealth = GetComponent<EnemyHealth>();
		agent = enemyHealth.nav;
	}

	public void OnEnable()
	{
		
		enemyHealth.ReStartAction();
	}

	public void OnDisable()
	{
	}
}
