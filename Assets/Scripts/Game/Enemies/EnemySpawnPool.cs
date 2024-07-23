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

	public Vector2 spawnAreaSize = new Vector2(30f, 30f); // ������ ������ ũ��

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

		// Ŭ���̾�Ʈ�� �濡 ���� InitializeEnemyPool�� ȣ��� ���Դϴ�.
		InitializeEnemyPool();
	}

	private void Update()
	{
		// ȣ��Ʈ�� ���� ���� ������ �� ����
		// �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
		// ���� ��� ����ģ ��� ���� ���� ����
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

	// enemy �ʱ�ȭ
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


	// ���� ���̺꿡 ���� ���� ����
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
				// �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
				pizzaZombiePool.Enqueue(enemy);
			}
		}
	}

	// ���� ���̺꿡 ���� ���� ����
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
				// �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
				playerZombiePool.Enqueue(enemy);
			}
		}
	}

	// ���� ���̺꿡 ���� ���� ����
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
					// �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
					bossZombiePool.Enqueue(enemy);
				}
			}
		}
	}


	// �� ��ü�� �ı��Ǿ��� �� ȣ��� �޼���
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
        properties[EnemyCountKey] = enemyPool.Count; // ������ enemy ���� �� �Ӽ��� ����
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
		// ���� ������ ������ ���
		float halfWidth = spawnAreaSize.x / 2f;
		float halfHeight = spawnAreaSize.y / 2f;

		// ������ ��ġ ���
		float randomX = Random.Range(-halfWidth, halfWidth);
		float randomZ = Random.Range(-halfHeight, halfHeight);

		// ������ ��ġ ���� ��ȯ
		Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ) + center;

		return randomSpawnPosition;
	}
}
