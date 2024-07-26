using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPool : MonoBehaviour
{
	public static EnemySpawnPool Instance;

	public Transform[] spawnPoints;

	public GameObject pizzaZombiePrefab;
	public GameObject playerZombiePrefab;
	public GameObject bossZombiePrefab;

	public Queue<EnemyHealth> pizzaZombiePool = new Queue<EnemyHealth>();
	public Queue<EnemyHealth> playerZombiePool = new Queue<EnemyHealth>();
	public Queue<EnemyHealth> bossZombiePool = new Queue<EnemyHealth>();

	private EnemyHealth enemyHealth;

	public int pizzaZombiePoolSize;
	public int playerZombiePoolSize;
	public int bossZombiePoolSize;

	public Vector2 spawnAreaSize = new Vector2(30f, 30f); // 스폰할 영역의 크기

	[HideInInspector] public int pizzaEnemiesToSpawn;
	[HideInInspector] public int playerEnemiesToSpawn;
	[HideInInspector] public int bossEnemiesToSpawn;

	private void Awake()
	{
		Instance = this;

		pizzaEnemiesToSpawn = 0;
		playerEnemiesToSpawn = 0;
		bossEnemiesToSpawn = 0;

		enemyHealth = GetComponent<EnemyHealth>();

		// Ensure that spawnPoints array is properly initialized
		spawnPoints = new Transform[3];
		spawnPoints[0] = transform.GetChild(0);
		spawnPoints[1] = transform.GetChild(1);
		spawnPoints[2] = transform.GetChild(2);
	}

	private void Start()
	{
		InitializeEnemyPool();
	}

	private void Update()
	{
		if (playerEnemiesToSpawn <= 0)
		{
			Invoke("FemaleZombieSpawnWave", 2f);
		}
		if (pizzaEnemiesToSpawn <= 0)
		{
			Invoke("MaleZombieSpawnWave", 2f);
		}
		if (bossEnemiesToSpawn <= 0)
		{
			Invoke("BossZombieSpawnWave", 2f);
		}
	}

	// enemy 초기화
	void InitializeEnemyPool()
	{
		//femaleZombie
		for (int i = 0; i < pizzaZombiePoolSize; i++)
		{
			GameObject enemy = Instantiate(pizzaZombiePrefab, spawnPoints[0].position, Quaternion.identity);
			EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
			pizzaZombiePool.Enqueue(enemyHealth);
			EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
			enemy.SetActive(false);
		}
		//maleZombie
		for (int i = 0; i < playerZombiePoolSize; i++)
		{
			GameObject enemy = Instantiate(playerZombiePrefab, spawnPoints[1].position, Quaternion.identity);
			EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
			playerZombiePool.Enqueue(enemyHealth);
			EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
			enemy.SetActive(false);
		}
		//bossmaleZombie
		for (int i = 0; i < bossZombiePoolSize; i++)
		{
			GameObject enemy = Instantiate(bossZombiePrefab, spawnPoints[2].position, Quaternion.identity);
			EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
			bossZombiePool.Enqueue(enemyHealth);
			EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
			enemy.SetActive(false);
		}
	}

	private void FemaleZombieSpawnWave()
	{
		playerEnemiesToSpawn = pizzaZombiePoolSize;
		for (int i = 0; i < Random.Range(0, pizzaZombiePoolSize); i++)
		{
			if (pizzaZombiePool.TryDequeue(out EnemyHealth enemy))
			{
				if (!enemy.gameObject.activeSelf)
				{
					Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
					enemy.transform.position = randomSpawnPosition;
					enemy.gameObject.SetActive(true);

					// 랜덤 애니메이션 설정
					Animator animator = enemy.GetComponent<Animator>();
					animator.speed = Random.Range(0.8f, 1.2f);
					animator.Play("1-walk_chase", 0, Random.Range(0f, 1f));
				}
				else
				{
					pizzaZombiePool.Enqueue(enemy);
				}
			}
		}
	}

	private void MaleZombieSpawnWave()
	{
		pizzaEnemiesToSpawn = playerZombiePoolSize;
		for (int i = 0; i < Random.Range(0, playerZombiePoolSize); i++)
		{
			if (playerZombiePool.TryDequeue(out EnemyHealth enemy))
			{
				if (!enemy.gameObject.activeSelf)
				{
					Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
					enemy.transform.position = randomSpawnPosition;
					enemy.gameObject.SetActive(true);

					// 랜덤 애니메이션 설정
					Animator animator = enemy.GetComponent<Animator>();
					animator.speed = Random.Range(0.8f, 1.2f);
					animator.Play("1-walk_chase", 0, Random.Range(0f, 1f));
				}
				else
				{
					playerZombiePool.Enqueue(enemy);
				}
			}
		}
	}

	private void BossZombieSpawnWave()
	{
		bossEnemiesToSpawn = bossZombiePoolSize;
		for (int i = 0; i < Random.Range(1, bossZombiePoolSize); i++)
		{
			if (bossZombiePool.TryDequeue(out EnemyHealth enemy))
			{
				if (!enemy.gameObject.activeSelf)
				{
					Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
					enemy.transform.position = randomSpawnPosition;
					enemy.gameObject.SetActive(true);
				}
				else
				{
					bossZombiePool.Enqueue(enemy);
				}
			}
		}
	}

	private void OnEnemyKilled(EnemyHealth enemy)
	{
	}

	private void UpdateRoomProperties()
	{
	}

	public void SyncStateWithNewPlayer()
	{
	}

	Vector3 GetRandomSpawnPosition(Vector3 center)
	{
		float halfWidth = spawnAreaSize.x / 2f;
		float halfHeight = spawnAreaSize.y / 2f;

		float randomX = Random.Range(-halfWidth, halfWidth);
		float randomZ = Random.Range(-halfHeight, halfHeight);

		Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ) + center;

		return randomSpawnPosition;
	}
}
