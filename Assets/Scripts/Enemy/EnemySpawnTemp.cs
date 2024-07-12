using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class EnemySpawnTemp : MonoBehaviourPunCallbacks
{
    public GameObject enemyPrefab;
    public List<EnemyHealth> enemyPool = new List<EnemyHealth>();
    private const string EnemyCountKey = "EnemyCount";
    private int wave = 200; // 현재 웨이브

    public int poolSize = 200;

    private void Start()
    {
        // 클라이언트가 방에 들어가면 InitializeEnemyPool이 호출될 것입니다.
    }

    public override void OnJoinedRoom()
    {
        // 클라이언트가 방에 들어갔을 때 호출됩니다.
        InitializeEnemyPool();
    }

    private void Update()
    {
        // 호스트만 적을 직접 생성할 수 있음
        // 다른 클라이언트들은 호스트가 생성한 적을 동기화를 통해 받아옴
        if (PhotonNetwork.IsMasterClient)
        {
            // 적을 모두 물리친 경우 다음 스폰 실행
            if (enemyPool.Count <= 0)
            {
                SpawnWave();
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
            enemyPool.Add(enemyHealth);
            enemy.SetActive(false);
        }
    }

    // 현재 웨이브에 맞춰 적을 생성
    private void SpawnWave()
    {
        /*// 웨이브 1 증가
        wave++;

        // 현재 웨이브 * 1.5에 반올림 한 개수 만큼 적을 활성화
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);*/

        for (int i = 0; i < poolSize; i++)
        {
            foreach (var enemy in enemyPool)
            {
                if (!enemy.gameObject.activeInHierarchy)
                {
                    enemy.transform.position = transform.position;
                    enemy.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    // 적 객체가 파괴되었을 때 호출될 메서드
    private void OnEnemyKilled(EnemyHealth enemy)
    {
        enemy.gameObject.SetActive(false);
        UpdateRoomProperties();
    }

    private void UpdateRoomProperties()
    {
        Hashtable properties = new Hashtable();
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
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
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
