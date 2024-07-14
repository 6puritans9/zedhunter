using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class EnemySpawnPool : MonoBehaviourPunCallbacks
{
    public static EnemySpawnPool Instance;
    public GameObject enemyPrefab;
    public Queue<EnemyHealth> enemyPool = new Queue<EnemyHealth>();
    private EnemyHealth enemyHealth;
    private int wave = 3; // 현재 웨이브

    public int poolSize;

    PhotonView photonView;

    private void Awake()
    {
        Instance = this;
        photonView = GetComponent<PhotonView>();
        enemyHealth = GetComponent<EnemyHealth>();
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
            if (enemyPool.Count <= poolSize)
            {
                SpawnWave();
                //photonView.RPC("SpawnWave", RpcTarget.All);
            }
            UpdateRoomProperties();
        }
    }

    // enemy 초기화
    void InitializeEnemyPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = PhotonNetwork.Instantiate(enemyPrefab.name, transform.position, Quaternion.identity);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            enemyPool.Enqueue(enemyHealth);
            EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
            enemy.SetActive(false);
        }
    }


    [PunRPC]
    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave()
    {

        int enemiesToSpawn = poolSize;
        while (enemiesToSpawn > 0 && enemyPool.Count > 0)
        {
            EnemyHealth enemy = enemyPool.Dequeue();
            if (!enemy.gameObject.activeInHierarchy)
            {
                enemy.transform.position = transform.position;
                enemy.gameObject.SetActive(true);
                enemiesToSpawn--;
            }
            else
            {
                // 다시 큐에 넣어 비활성화된 적을 찾을 때까지 반복
                enemyPool.Enqueue(enemy);
            }
        }
    }

    // 적 객체가 파괴되었을 때 호출될 메서드
    private void OnEnemyKilled(EnemyHealth enemy)
    {
        //Debug.Log("나 호출됐어!");
        enemy.gameObject.SetActive(false);
        UpdateRoomProperties();
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
}
