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
		spawnPoints[0] = transform.GetChild(0);
		spawnPoints[1] = transform.GetChild(1);
		spawnPoints[2] = transform.GetChild(2);
	}
	private void Start()
	{

		// 클라이언트가 방에 들어가면 InitializeEnemyPool이 호출될 것입니다.
		InitializeEnemyPool();
	}

	private void Update()
	{
		// 호스트만 적을 직접 생성할 수 있음
		// 다른 클라이언트들은 호스트가 생성한 적을 동기화를 통해 받아옴
		// 적을 모두 물리친 경우 다음 스폰 실행
		if (playerEnemiesToSpawn <= 0)
		{
			//FemaleZombieSpawnWave();
			FemaleZombieSpawnWave();
		}
		if (pizzaEnemiesToSpawn <= 0)
		{

			//MaleZombieSpawnWave();
			MaleZombieSpawnWave();
		}
		if (bossEnemiesToSpawn <= 0)
		{
			//MaleZombieSpawnWave();
			BossZombieSpawnWave();
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


	// 현재 웨이브에 맞춰 적을 생성
	private void FemaleZombieSpawnWave()
	{
		playerEnemiesToSpawn = pizzaZombiePoolSize;
		//int enemiesToSpawn = femaleZombiePoolSize;
		for (int i = 0; i < pizzaZombiePoolSize; i++)
		{
			//EnemyHealth enemy = femaleZombiePool.Dequeue();
			pizzaZombiePool.TryDequeue(out EnemyHealth enemy);
			if (!enemy.gameObject.activeSelf)
			{
				//enemy.transform.position = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
				Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
				enemy.transform.position = randomSpawnPosition;
				enemy.gameObject.SetActive(true);
				//enemiesToSpawn--;
			}
			else
			{
				// 다시 큐에 넣어 비활성화된 적을 찾을 때까지 반복
				pizzaZombiePool.Enqueue(enemy);
			}
		}
	}

	// 현재 웨이브에 맞춰 적을 생성
	private void MaleZombieSpawnWave()
	{
		pizzaEnemiesToSpawn = playerZombiePoolSize;
		//maleEnemiesToSpawn = maleZombiePoolSize;
		for (int i = 0; i < playerZombiePoolSize; i++)
		{
			//EnemyHealth enemy = maleZombiePool.Dequeue();
			playerZombiePool.TryDequeue(out EnemyHealth enemy);
			if (!enemy.gameObject.activeSelf)
			{
				//enemy.transform.position = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
				Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
				enemy.transform.position = randomSpawnPosition;
				enemy.gameObject.SetActive(true);
				//enemiesToSpawn--;
			}
			else
			{
				// 다시 큐에 넣어 비활성화된 적을 찾을 때까지 반복
				playerZombiePool.Enqueue(enemy);
			}
		}
	}

	// 현재 웨이브에 맞춰 적을 생성
	private void BossZombieSpawnWave()
	{
		bossEnemiesToSpawn = bossZombiePoolSize;
		//int enemiesToSpawn = femaleZombiePoolSize;
		for (int i = 0; i < bossZombiePoolSize; i++)
		{
			//EnemyHealth enemy = femaleZombiePool.Dequeue();
			bossZombiePool.TryDequeue(out EnemyHealth enemy);
			if (enemy)
			{
				if (!enemy.gameObject.activeSelf)
				{
					//enemy.transform.position = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
					Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
					enemy.transform.position = randomSpawnPosition;
					enemy.gameObject.SetActive(true);
					//enemiesToSpawn--;
				}
				else
				{
					// 다시 큐에 넣어 비활성화된 적을 찾을 때까지 반복
					bossZombiePool.Enqueue(enemy);
				}
			}
		}
	}


	// 적 객체가 파괴되었을 때 호출될 메서드
	private void OnEnemyKilled(EnemyHealth enemy)
	{
		//enemy.gameObject.SetActive(false);
	}

	private void UpdateRoomProperties()
	{
		/*Hashtable properties = new Hashtable();
        for (int i = enemyPool.Count - 1; i >= 0; i--)
        {
            if (enemyPool[i] == null || !enemyPool[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            properties[$"enemyPosition_{i}"] = enemyPool[i].transform.position;
            properties[$"enemyRotation_{i}"] = enemyPool[i].transform.rotation;
        }
        properties[EnemyCountKey] = enemyPool.Count; // 생성된 enemy 수를 방 속성에 저장
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);*/
	}

	public void SyncStateWithNewPlayer()
	{
		/*Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
        int i = 0;
        while (properties.ContainsKey($"enemyPosition_{i}"))
        {
            Vector3 enemyPos = (Vector3)properties[$"enemyPosition_{i}"];
            Quaternion enemyRot = (Quaternion)properties[$"enemyRotation_{i}"];
            GameObject _enemy = PhotonNetwork.Instantiate(enemyPrefab.name, enemyPos, enemyRot);
            enemyPool.Add(_enemy.GetComponent<EnemyHealth>());
            i++;
        }*/
	}

	Vector3 GetRandomSpawnPosition(Vector3 center)
	{
		// 스폰 영역의 반지름 계산
		float halfWidth = spawnAreaSize.x / 2f;
		float halfHeight = spawnAreaSize.y / 2f;

		// 랜덤한 위치 계산
		float randomX = Random.Range(-halfWidth, halfWidth);
		float randomZ = Random.Range(-halfHeight, halfHeight);

		// 랜덤한 위치 벡터 반환
		Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ) + center;

		return randomSpawnPosition;
	}
}
