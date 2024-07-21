// 기존 코드

// using Photon.Pun;
// using System.Collections;
// using System.Collections.Generic;
// using Hashtable = ExitGames.Client.Photon.Hashtable;
// using UnityEngine;

// public class EnemySpawner : MonoBehaviourPunCallbacks
//      {
//          public static EnemySpawner Instance;
     
//          public Transform[] spawnPoints;
     
//          public GameObject femaleZombiePrefab;
//          public GameObject maleZombiePrefab;
//          public GameObject bossZombiePrefab;
     
//          public Queue<EnemyHealth> femaleZombiePool = new Queue<EnemyHealth>();
//          public Queue<EnemyHealth> maleZombiePool = new Queue<EnemyHealth>();
//          public Queue<EnemyHealth> bossZombiePool = new Queue<EnemyHealth>();
     
//          private EnemyHealth enemyHealth;
     
//          public int femaleZombiePoolSize;
//          public int maleZombiePoolSize;
//          public int bossZombiePoolSize;
     
//          public Vector2 spawnAreaSize = new Vector2(30f, 30f); // ������ ������ ũ��
     
//          [HideInInspector]public int maleEnemiesToSpawn;
//          [HideInInspector] public int femaleEnemiesToSpawn;
//          [HideInInspector] public int bossEnemiesToSpawn;
//          private void Awake()
//          {
//              Instance = this;
     
//              PhotonView photonView = GetComponent<PhotonView>();
     
//              maleEnemiesToSpawn = 0;
//              femaleEnemiesToSpawn = 0;
//              bossEnemiesToSpawn = 0;
     
//              enemyHealth = GetComponent<EnemyHealth>();
//              spawnPoints[0] = transform.GetChild(0);
//              spawnPoints[1] = transform.GetChild(1);
//              spawnPoints[2] = transform.GetChild(2);
//          }
//          private void Start()
//          {
             
//              if (PhotonNetwork.IsMasterClient)
//                  {
//                      InitializeEnemyPool();
//                  }
//          }
     
//          private void Update()
//          {
//              // ȣ��Ʈ�� ���� ���� ������ �� ����
//              // �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
//              if (PhotonNetwork.IsMasterClient)
//              {
//                  // ���� ��� ����ģ ��� ���� ���� ����
//                  if (femaleEnemiesToSpawn <= 0)
//                  {
//                      //FemaleZombieSpawnWave();
//                      photonView.RPC("FemaleZombieSpawnWave", RpcTarget.All);
//                  }
//                  if (maleEnemiesToSpawn <= 0)
//                  {
                     
//                      //MaleZombieSpawnWave();
//                      photonView.RPC("MaleZombieSpawnWave", RpcTarget.All);
//                  }
//                  if (bossEnemiesToSpawn <= 0)
//                  {
//                      //MaleZombieSpawnWave();
//                      photonView.RPC("BossZombieSpawnWave", RpcTarget.All);
//                  }
     
//                  //UpdateRoomProperties();
//              }
//          }
     
//          // enemy �ʱ�ȭ
//          void InitializeEnemyPool()
//          {
//              //femaleZombie
//              for (int i = 0; i < femaleZombiePoolSize; i++)
//              {
//                  GameObject enemy = PhotonNetwork.Instantiate(femaleZombiePrefab.name, spawnPoints[0].position, Quaternion.identity);
//                  EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//                  femaleZombiePool.Enqueue(enemyHealth);
//                  EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
//                  enemy.SetActive(false);
//              }
//              //maleZombie
//              for (int i = 0; i < maleZombiePoolSize; i++)
//              {
//                  GameObject enemy = PhotonNetwork.Instantiate(maleZombiePrefab.name, spawnPoints[1].position, Quaternion.identity);
//                  EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//                  maleZombiePool.Enqueue(enemyHealth);
//                  EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
//                  enemy.SetActive(false);
//              }
//              //bossmaleZombie
//              for (int i = 0; i < bossZombiePoolSize; i++)
//              {
//                  GameObject enemy = PhotonNetwork.Instantiate(bossZombiePrefab.name, spawnPoints[2].position, Quaternion.identity);
//                  EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//                  bossZombiePool.Enqueue(enemyHealth);
//                  EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
//                  enemy.SetActive(false);
//              }
//          }
     
     
//          [PunRPC]
//          // ���� ���̺꿡 ���� ���� ����
//          private void FemaleZombieSpawnWave()
//          {
//              femaleEnemiesToSpawn = femaleZombiePoolSize;
//              //int enemiesToSpawn = femaleZombiePoolSize;
//              for (int i = 0; i < femaleZombiePoolSize; i++)
//              {
//                  //EnemyHealth enemy = femaleZombiePool.Dequeue();
//                  femaleZombiePool.TryDequeue(out EnemyHealth enemy);
//                  if (!enemy.gameObject.activeSelf)
//                  {
//                      enemy.transform.position = GetRandomSpawnPosition(spawnPoints[0].position);
//                      //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[0].position);
//                      //enemy.transform.position = randomSpawnPosition;
//                      enemy.gameObject.SetActive(true);
//                      //enemiesToSpawn--;
//                  }
//                  else
//                  {
//                      // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
//                      femaleZombiePool.Enqueue(enemy);
//                  }
//              }
//          }
     
//          [PunRPC]
//          // ���� ���̺꿡 ���� ���� ����
//          private void MaleZombieSpawnWave()
//          {
//              maleEnemiesToSpawn = maleZombiePoolSize;
//              //maleEnemiesToSpawn = maleZombiePoolSize;
//              for (int i = 0; i < maleZombiePoolSize; i++)
//              {
//                  //EnemyHealth enemy = maleZombiePool.Dequeue();
//                  maleZombiePool.TryDequeue(out EnemyHealth enemy);
//                  if (!enemy.gameObject.activeSelf)
//                  {
//                      enemy.transform.position = GetRandomSpawnPosition(spawnPoints[1].position);
//                      //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[1].position);
//                      //enemy.transform.position = randomSpawnPosition;
//                      enemy.gameObject.SetActive(true);
//                      //enemiesToSpawn--;
//                  }
//                  else
//                  {
//                      // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
//                      maleZombiePool.Enqueue(enemy);
//                  }
//              }
//          }
     
//          [PunRPC]
//          // ���� ���̺꿡 ���� ���� ����
//          private void BossZombieSpawnWave()
//          {
//              bossEnemiesToSpawn = bossZombiePoolSize;
//              //int enemiesToSpawn = femaleZombiePoolSize;
//              for (int i = 0; i < bossZombiePoolSize; i++)
//              {
//                  //EnemyHealth enemy = femaleZombiePool.Dequeue();
//                  bossZombiePool.TryDequeue(out EnemyHealth enemy);
//                  if(enemy)
//                  {
//                      if (!enemy.gameObject.activeSelf)
//                      {
//                          enemy.transform.position = GetRandomSpawnPosition(spawnPoints[2].position);
//                          //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[0].position);
//                          //enemy.transform.position = randomSpawnPosition;
//                          enemy.gameObject.SetActive(true);
//                          //enemiesToSpawn--;
//                      }
//                      else
//                      {
//                          // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
//                          bossZombiePool.Enqueue(enemy);
//                      }
//                  }
//              }
//          }
     
     
//          // �� ��ü�� �ı��Ǿ��� �� ȣ��� �޼���
//          private void OnEnemyKilled(EnemyHealth enemy)
//          {
//              //enemy.gameObject.SetActive(false);
//          }
     
//          private void UpdateRoomProperties()
//          {
//              /*Hashtable properties = new Hashtable();
//              for (int i = enemyPool.Count - 1; i >= 0; i--)
//              {
//                  if (enemyPool[i] == null || !enemyPool[i].gameObject.activeInHierarchy)
//                  {
//                      continue;
//                  }
     
//                  properties[$"enemyPosition_{i}"] = enemyPool[i].transform.position;
//                  properties[$"enemyRotation_{i}"] = enemyPool[i].transform.rotation;
//              }
//              properties[EnemyCountKey] = enemyPool.Count; // ������ enemy ���� �� �Ӽ��� ����
//              PhotonNetwork.CurrentRoom.SetCustomProperties(properties);*/
//          }
     
//          public void SyncStateWithNewPlayer()
//          {
//              /*Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
//              int i = 0;
//              while (properties.ContainsKey($"enemyPosition_{i}"))
//              {
//                  Vector3 enemyPos = (Vector3)properties[$"enemyPosition_{i}"];
//                  Quaternion enemyRot = (Quaternion)properties[$"enemyRotation_{i}"];
//                  GameObject _enemy = PhotonNetwork.Instantiate(enemyPrefab.name, enemyPos, enemyRot);
//                  enemyPool.Add(_enemy.GetComponent<EnemyHealth>());
//                  i++;
//              }*/
//          }
     
//          Vector3 GetRandomSpawnPosition(Vector3 center)
//          {
//              // ���� ������ ������ ���
//              float halfWidth = spawnAreaSize.x / 2f;
//              float halfHeight = spawnAreaSize.y / 2f;
     
//              // ������ ��ġ ���
//              float randomX = Random.Range(-halfWidth, halfWidth);
//              float randomZ = Random.Range(-halfHeight, halfHeight);
     
//              // ������ ��ġ ���� ��ȯ
//              Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ) + center;
     
//              return randomSpawnPosition;
//          }
// }





// 수정한 코드

// using Photon.Pun;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class EnemySpawner : MonoBehaviourPunCallbacks
// {
//     public static EnemySpawner Instance;

//     public Transform[] spawnPoints = new Transform[10]; // 10개의 스폰 포인트
//     public GameObject femaleZombiePrefab;
//     public GameObject maleZombiePrefab;
//     public GameObject bossZombiePrefab;

//     public int femaleZombiePoolSize;
//     public int maleZombiePoolSize;
//     public int bossZombiePoolSize;

//     private Queue<EnemyHealth> femaleZombiePool = new Queue<EnemyHealth>();
//     private Queue<EnemyHealth> maleZombiePool = new Queue<EnemyHealth>();
//     private Queue<EnemyHealth> bossZombiePool = new Queue<EnemyHealth>();

//     public Vector2 spawnAreaSize = new Vector2(1f, 1f); // 스폰 영역 크기

//     private void Awake()
//     {
//         Instance = this;
//         InitializeSpawnPoints();
//     }

//     private void Start()
//     {
//         if (PhotonNetwork.IsMasterClient)
//         {
//             InitializeEnemyPool();
//             StartCoroutine(SpawnEnemies());
//         }
//     }

//     private void InitializeSpawnPoints()
//     {
//         for (int i = 0; i < spawnPoints.Length; i++)
//         {
//             spawnPoints[i] = transform.GetChild(i);
//         }
//     }

//     private void InitializeEnemyPool()
//     {
//         // femaleZombie
//         for (int i = 0; i < femaleZombiePoolSize; i++)
//         {
//             GameObject enemy = PhotonNetwork.Instantiate(femaleZombiePrefab.name, Vector3.zero, Quaternion.identity);
//             EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//             femaleZombiePool.Enqueue(enemyHealth);
//             enemy.SetActive(false);
//         }

//         // maleZombie
//         for (int i = 0; i < maleZombiePoolSize; i++)
//         {
//             GameObject enemy = PhotonNetwork.Instantiate(maleZombiePrefab.name, Vector3.zero, Quaternion.identity);
//             EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//             maleZombiePool.Enqueue(enemyHealth);
//             enemy.SetActive(false);
//         }

//         // bossZombie
//         for (int i = 0; i < bossZombiePoolSize; i++)
//         {
//             GameObject enemy = PhotonNetwork.Instantiate(bossZombiePrefab.name, Vector3.zero, Quaternion.identity);
//             EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//             bossZombiePool.Enqueue(enemyHealth);
//             enemy.SetActive(false);
//         }
//     }

//     private IEnumerator SpawnEnemies()
//     {
//         while (true)
//         {
//             foreach (var spawnPoint in spawnPoints)
//             {
//                 float rand = Random.Range(0f, 1f);
//                 if (rand <= 0.60f)
//                 {
//                     SpawnZombieFromPool(femaleZombiePool, spawnPoint.position);
//                 }
//                 else if (rand <= 0.95f)
//                 {
//                     SpawnZombieFromPool(maleZombiePool, spawnPoint.position);
//                 }
//                 else
//                 {
//                     SpawnZombieFromPool(bossZombiePool, spawnPoint.position);
//                 }
//             }
//             yield return new WaitForSeconds(5f); // 2초 대기
//         }
//     }

//     private void SpawnZombieFromPool(Queue<EnemyHealth> pool, Vector3 spawnPosition)
//     {
//         if (pool.TryDequeue(out EnemyHealth enemy))
//         {
//             if (!enemy.gameObject.activeSelf)
//             {
//                 enemy.transform.position = GetRandomSpawnPosition(spawnPosition);
//                 enemy.gameObject.SetActive(true);
//             }
//             else
//             {
//                 pool.Enqueue(enemy);
//             }
//         }
//     }

//     private Vector3 GetRandomSpawnPosition(Vector3 center)
//     {
//         float halfWidth = spawnAreaSize.x / 2f;
//         float halfHeight = spawnAreaSize.y / 2f;

//         float randomX = Random.Range(-halfWidth, halfWidth);
//         float randomZ = Random.Range(-halfHeight, halfHeight);

//         Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ) + center;

//         return randomSpawnPosition;
//     }
// }
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner Instance;

    public Transform[] spawnPoints;
    public GameObject[] enemyPrefabs; // 0: female, 1: male, 2: boss

    [System.Serializable]
    public class EnemyPoolInfo
    {
        public int poolSize;
        public float spawnChance;
    }

    public EnemyPoolInfo[] enemyPoolInfos; // 0: female, 1: male, 2: boss

    private ObjectPool<EnemyHealth>[] enemyPools;

    public Vector2 spawnAreaSize = new Vector2(1f, 1f);

    private float spawnInterval = 5f;
    private float nextSpawnTime;

    private void Awake()
    {
        Instance = this;
        InitializeSpawnPoints();
        InitializeEnemyPools();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && Time.time >= nextSpawnTime)
        {
            SpawnEnemyWave();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void InitializeSpawnPoints()
    {
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }
    }

    private void InitializeEnemyPools()
    {
        enemyPools = new ObjectPool<EnemyHealth>[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int poolSize = enemyPoolInfos[i].poolSize;
            enemyPools[i] = new ObjectPool<EnemyHealth>(
                createFunc: () => PhotonNetwork.Instantiate(enemyPrefabs[i].name, Vector3.zero, Quaternion.identity).GetComponent<EnemyHealth>(),
                actionOnGet: (enemy) => enemy.gameObject.SetActive(true),
                actionOnRelease: (enemy) => enemy.gameObject.SetActive(false),
                actionOnDestroy: (enemy) => PhotonNetwork.Destroy(enemy.gameObject),
                defaultCapacity: poolSize,
                maxSize: poolSize
            );
        }
    }

    [PunRPC]
    private void SpawnEnemy(int enemyType, Vector3 position, int viewID)
    {
        if (enemyType < 0 || enemyType >= enemyPools.Length) return;

        EnemyHealth enemy = enemyPools[enemyType].Get();
        enemy.transform.position = position;
        enemy.GetComponent<PhotonView>().ViewID = viewID;
        enemy.OnEnemyKilled += (killedEnemy) => ReturnEnemyToPool(killedEnemy, enemyType);
    }

    private void SpawnEnemyWave()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            float rand = Random.Range(0f, 1f);
            float cumulativeChance = 0f;

            for (int i = 0; i < enemyPoolInfos.Length; i++)
            {
                cumulativeChance += enemyPoolInfos[i].spawnChance;
                if (rand <= cumulativeChance)
                {
                    Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint.position);
                    // int viewID = PhotonNetwork.AllocateViewID();
                    PhotonView pv = PhotonNetwork.Instantiate(enemyPrefabs[i].name, spawnPosition, Quaternion.identity).GetComponent<PhotonView>();
                    int viewID = pv.ViewID;
                    photonView.RPC("SpawnEnemy", RpcTarget.All, i, spawnPosition, viewID);
                    break;
                }
            }
        }
    }

    private void ReturnEnemyToPool(EnemyHealth enemy, int enemyType)
    {
        enemyPools[enemyType].Release(enemy);
    }

    private Vector3 GetRandomSpawnPosition(Vector3 center)
    {
        float halfWidth = spawnAreaSize.x / 2f;
        float halfHeight = spawnAreaSize.y / 2f;

        float randomX = Random.Range(-halfWidth, halfWidth);
        float randomZ = Random.Range(-halfHeight, halfHeight);

        return new Vector3(randomX, 0f, randomZ) + center;
    }
}

// 간단한 ObjectPool 구현
public class ObjectPool<T> where T : Component
{
    private readonly Queue<T> objects = new Queue<T>();
    private readonly System.Func<T> createFunc;
    private readonly System.Action<T> actionOnGet;
    private readonly System.Action<T> actionOnRelease;
    private readonly System.Action<T> actionOnDestroy;
    private readonly int maxSize;

    public ObjectPool(System.Func<T> createFunc, System.Action<T> actionOnGet, System.Action<T> actionOnRelease, System.Action<T> actionOnDestroy, int defaultCapacity, int maxSize)
    {
        this.createFunc = createFunc;
        this.actionOnGet = actionOnGet;
        this.actionOnRelease = actionOnRelease;
        this.actionOnDestroy = actionOnDestroy;
        this.maxSize = maxSize;

        for (int i = 0; i < defaultCapacity; i++)
        {
            objects.Enqueue(createFunc());
        }
    }

    public T Get()
    {
        T obj = objects.Count > 0 ? objects.Dequeue() : createFunc();
        actionOnGet?.Invoke(obj);
        return obj;
    }

    public void Release(T obj)
    {
        if (objects.Count < maxSize)
        {
            actionOnRelease?.Invoke(obj);
            objects.Enqueue(obj);
        }
        else
        {
            actionOnDestroy?.Invoke(obj);
        }
    }
}