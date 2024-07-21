// using Photon.Pun;
// using System.Collections;
// using System.Collections.Generic;
// using Hashtable = ExitGames.Client.Photon.Hashtable;
// using UnityEngine;

// public class EnemySpawnPool : MonoBehaviourPunCallbacks
//     {
//         public static EnemySpawnPool Instance;
//         public GameObject enemyPrefab;
//         public Queue<EnemyHealth> enemyPool = new Queue<EnemyHealth>();
//         private EnemyHealth enemyHealth;
//         private int wave = 3; // ���� ���̺�

//         public int poolSize;

//         private void Awake()
//             {
//                 Instance = this;
//                 enemyHealth = GetComponent<EnemyHealth>();
//             }

//         private void Start()
//             {
//                 // Ŭ���̾�Ʈ�� �濡 ���� InitializeEnemyPool�� ȣ��� ���Դϴ�.
//             }

//         public override void OnJoinedRoom()
//             {
//                 // Ŭ���̾�Ʈ�� �濡 ���� �� ȣ��˴ϴ�.
//                 if (PhotonNetwork.IsMasterClient)
//                     {
//                         InitializeEnemyPool();
//                     }
//             }

//         private void Update()
//             {
//                 // ȣ��Ʈ�� ���� ���� ������ �� ����
//                 // �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
//                 if (PhotonNetwork.IsMasterClient)
//                     {
//                         // ���� ��� ����ģ ��� ���� ���� ����
//                         if (enemyPool.Count <= poolSize)
//                             {
//                                 SpawnWave();
//                                 //photonView.RPC("SpawnWave", RpcTarget.All);
//                             }

//                         UpdateRoomProperties();
//                     }
//             }

//         // enemy �ʱ�ȭ
//         void InitializeEnemyPool()
//             {
//                 for (int i = 0; i < poolSize; i++)
//                     {
//                         GameObject enemy =
//                             PhotonNetwork.Instantiate(enemyPrefab.name, transform.position, Quaternion.identity);
//                         EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
//                         enemyPool.Enqueue(enemyHealth);
//                         EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
//                         enemy.SetActive(false);
//                     }
//             }


//         [PunRPC]
//         // ���� ���̺꿡 ���� ���� ����
//         private void SpawnWave()
//             {
//                 int enemiesToSpawn = poolSize;
//                 while (enemiesToSpawn > 0 && enemyPool.Count > 0)
//                     {
//                         EnemyHealth enemy = enemyPool.Dequeue();
//                         if (!enemy.gameObject.activeInHierarchy)
//                             {
//                                 enemy.transform.position = transform.position;
//                                 enemy.gameObject.SetActive(true);
//                                 enemiesToSpawn--;
//                             }
//                         else
//                             {
//                                 // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
//                                 enemyPool.Enqueue(enemy);
//                             }
//                     }
//             }

//         // �� ��ü�� �ı��Ǿ��� �� ȣ��� �޼���
//         private void OnEnemyKilled(EnemyHealth enemy)
//             {
//                 //Debug.Log("�� ȣ��ƾ�!");
//                 enemy.gameObject.SetActive(false);
//                 UpdateRoomProperties();
//             }

//         private void UpdateRoomProperties()
//             {
//                 /*Hashtable properties = new Hashtable();
//                 for (int i = enemyPool.Count - 1; i >= 0; i--)
//                 {
//                     if (enemyPool[i] == null || !enemyPool[i].gameObject.activeInHierarchy)
//                     {
//                         continue;
//                     }

//                     properties[$"enemyPosition_{i}"] = enemyPool[i].transform.position;
//                     properties[$"enemyRotation_{i}"] = enemyPool[i].transform.rotation;
//                 }
//                 properties[EnemyCountKey] = enemyPool.Count; // ������ enemy ���� �� �Ӽ��� ����
//                 PhotonNetwork.CurrentRoom.SetCustomProperties(properties);*/
//             }

//         public void SyncStateWithNewPlayer()
//             {
//                 /*Hashtable properties = PhotonNetwork.CurrentRoom.CustomProperties;
//                 int i = 0;
//                 while (properties.ContainsKey($"enemyPosition_{i}"))
//                 {
//                     Vector3 enemyPos = (Vector3)properties[$"enemyPosition_{i}"];
//                     Quaternion enemyRot = (Quaternion)properties[$"enemyRotation_{i}"];
//                     GameObject _enemy = PhotonNetwork.Instantiate(enemyPrefab.name, enemyPos, enemyRot);
//                     enemyPool.Add(_enemy.GetComponent<EnemyHealth>());
//                     i++;
//                 }*/
//             }
//     }


using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class EnemySpawnPool : MonoBehaviourPunCallbacks
{
    public static EnemySpawnPool Instance;
    public GameObject maleEnemyPrefab;
    public GameObject femaleEnemyPrefab;
    public GameObject bossEnemyPrefab;

    public Queue<EnemyHealth> maleZombiePool = new Queue<EnemyHealth>();
    public Queue<EnemyHealth> femaleZombiePool = new Queue<EnemyHealth>();
    public Queue<EnemyHealth> bossZombiePool = new Queue<EnemyHealth>();

    public int maleEnemiesToSpawn;
    public int femaleEnemiesToSpawn;
    public int bossEnemiesToSpawn;

    private int wave = 3; // 현재 웨이브

    public int malePoolSize;
    public int femalePoolSize;
    public int bossPoolSize;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 클라이언트가 방에 참여할 때 InitializeEnemyPool이 호출될 것입니다.
    }

    public override void OnJoinedRoom()
    {
        // 클라이언트가 방에 참여할 때 호출됩니다.
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeEnemyPool();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnWaveIfNeeded();
            UpdateRoomProperties();
        }
    }

    void InitializeEnemyPool()
    {
        InitializePoolForType(maleEnemyPrefab, malePoolSize, maleZombiePool);
        InitializePoolForType(femaleEnemyPrefab, femalePoolSize, femaleZombiePool);
        InitializePoolForType(bossEnemyPrefab, bossPoolSize, bossZombiePool);
    }

    void InitializePoolForType(GameObject prefab, int poolSize, Queue<EnemyHealth> pool)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = PhotonNetwork.Instantiate(prefab.name, transform.position, Quaternion.identity);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            pool.Enqueue(enemyHealth);
            enemyHealth.OnEnemyKilled += OnEnemyKilled;
            enemy.SetActive(false);
        }
    }

    private void SpawnWaveIfNeeded()
    {
        if (maleZombiePool.Count <= malePoolSize)
        {
            SpawnWaveForType(maleZombiePool, ref maleEnemiesToSpawn);
        }
        if (femaleZombiePool.Count <= femalePoolSize)
        {
            SpawnWaveForType(femaleZombiePool, ref femaleEnemiesToSpawn);
        }
        if (bossZombiePool.Count <= bossPoolSize)
        {
            SpawnWaveForType(bossZombiePool, ref bossEnemiesToSpawn);
        }
    }

    private void SpawnWaveForType(Queue<EnemyHealth> pool, ref int enemiesToSpawn)
    {
        while (enemiesToSpawn > 0 && pool.Count > 0)
        {
            EnemyHealth enemy = pool.Dequeue();
            if (!enemy.gameObject.activeInHierarchy)
            {
                enemy.transform.position = GetRandomSpawnPosition();
                enemy.gameObject.SetActive(true);
                enemy.ReStartAction();
                enemiesToSpawn--;
            }
            else
            {
                pool.Enqueue(enemy);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // 랜덤한 스폰 위치를 반환하는 로직을 구현하세요
        return transform.position + Random.insideUnitSphere * 10f;
    }

    private void OnEnemyKilled(EnemyHealth enemy)
    {
        enemy.gameObject.SetActive(false);
        UpdateRoomProperties();
    }

    private void UpdateRoomProperties()
    {
        // 필요한 경우 방 속성을 업데이트하는 로직을 구현하세요
    }

    public void SyncStateWithNewPlayer()
    {
        // 필요한 경우 새 플레이어와 상태를 동기화하는 로직을 구현하세요
    }
}