using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class EnemySpawnPool : MonoBehaviourPunCallbacks
{
    public static EnemySpawnPool Instance;

    public PhotonView photonView;

    public Transform[] spawnPoints;

    public GameObject femaleZombiePrefab;
    public GameObject maleZombiePrefab;

    public Queue<EnemyHealth> femaleZombiePool = new Queue<EnemyHealth>();
    public Queue<EnemyHealth> maleZombiePool = new Queue<EnemyHealth>();

    private EnemyHealth enemyHealth;

    public int femaleZombiePoolSize;
    public int maleZombiePoolSize;

    public Vector2 spawnAreaSize = new Vector2(30f, 30f); // 스폰할 영역의 크기

    public int maleEnemiesToSpawn;
    public int femaleEnemiesToSpawn;
    private void Awake()
    {
        Instance = this;

        photonView = GetComponent<PhotonView>();

        maleEnemiesToSpawn = 0;
        femaleEnemiesToSpawn = 0;

        enemyHealth = GetComponent<EnemyHealth>();
        spawnPoints[0] = transform.GetChild(0);
        spawnPoints[1] = transform.GetChild(1);
    }
    private void Start()
    {
        
        // 클라이언트가 방에 들어가면 InitializeEnemyPool이 호출될 것입니다.
    }

    public override void OnJoinedRoom()
    {
        // 클라이언트가 방에 들어갔을 때 호출됩니다.
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeEnemyPool();
        }
    }

    private void Update()
    {
        // 호스트만 적을 직접 생성할 수 있음
        // 다른 클라이언트들은 호스트가 생성한 적을 동기화를 통해 받아옴
        if (PhotonNetwork.IsMasterClient)
        {
            // 적을 모두 물리친 경우 다음 스폰 실행
            if (femaleEnemiesToSpawn <= 0)
            {
                //FemaleZombieSpawnWave();
                photonView.RPC("FemaleZombieSpawnWave", RpcTarget.All);
            }
            if (maleEnemiesToSpawn <= 0)
            {
                
                //MaleZombieSpawnWave();
                photonView.RPC("MaleZombieSpawnWave", RpcTarget.All);
            }
            //UpdateRoomProperties();
        }
    }

    // enemy 초기화
    void InitializeEnemyPool()
    {
        for (int i = 0; i < femaleZombiePoolSize; i++)
        {
            GameObject enemy = PhotonNetwork.Instantiate(femaleZombiePrefab.name, spawnPoints[0].position, Quaternion.identity);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            femaleZombiePool.Enqueue(enemyHealth);
            EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
            enemy.SetActive(false);
        }

        for (int i = 0; i < maleZombiePoolSize; i++)
        {
            GameObject enemy = PhotonNetwork.Instantiate(maleZombiePrefab.name, spawnPoints[1].position, Quaternion.identity);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            maleZombiePool.Enqueue(enemyHealth);
            EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
            enemy.SetActive(false);
        }
    }


    [PunRPC]
    // 현재 웨이브에 맞춰 적을 생성
    private void FemaleZombieSpawnWave()
    {
        femaleEnemiesToSpawn = femaleZombiePoolSize;
        //int enemiesToSpawn = femaleZombiePoolSize;
        for (int i = 0; i < femaleZombiePoolSize; i++)
        {
            //EnemyHealth enemy = femaleZombiePool.Dequeue();
            femaleZombiePool.TryDequeue(out EnemyHealth enemy);
            if (!enemy.gameObject.activeSelf)
            {
                enemy.transform.position = spawnPoints[0].position;
                //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[0].position);
                //enemy.transform.position = randomSpawnPosition;
                enemy.gameObject.SetActive(true);
                //enemiesToSpawn--;
            }
            else
            {
                // 다시 큐에 넣어 비활성화된 적을 찾을 때까지 반복
                femaleZombiePool.Enqueue(enemy);
            }
        }
    }

    [PunRPC]
    // 현재 웨이브에 맞춰 적을 생성
    private void MaleZombieSpawnWave()
    {
        maleEnemiesToSpawn = maleZombiePoolSize;
        //maleEnemiesToSpawn = maleZombiePoolSize;
        for (int i = 0; i < maleZombiePoolSize; i++)
        {
            //EnemyHealth enemy = maleZombiePool.Dequeue();
            maleZombiePool.TryDequeue(out EnemyHealth enemy);
            if (!enemy.gameObject.activeSelf)
            {
                enemy.transform.position = spawnPoints[1].position;
                //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[1].position);
                //enemy.transform.position = randomSpawnPosition;
                enemy.gameObject.SetActive(true);
                //enemiesToSpawn--;
            }
            else
            {
                // 다시 큐에 넣어 비활성화된 적을 찾을 때까지 반복
                maleZombiePool.Enqueue(enemy);
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
