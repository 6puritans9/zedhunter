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
    private int wave = 200; // ���� ���̺�

    public int poolSize = 200;

    private void Start()
    {
        // Ŭ���̾�Ʈ�� �濡 ���� InitializeEnemyPool�� ȣ��� ���Դϴ�.
    }

    public override void OnJoinedRoom()
    {
        // Ŭ���̾�Ʈ�� �濡 ���� �� ȣ��˴ϴ�.
        InitializeEnemyPool();
    }

    private void Update()
    {
        // ȣ��Ʈ�� ���� ���� ������ �� ����
        // �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
        if (PhotonNetwork.IsMasterClient)
        {
            // ���� ��� ����ģ ��� ���� ���� ����
            if (enemyPool.Count <= 0)
            {
                SpawnWave();
            }
            UpdateRoomProperties();
        }
    }

    // enemy �ʱ�ȭ
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

    // ���� ���̺꿡 ���� ���� ����
    private void SpawnWave()
    {
        /*// ���̺� 1 ����
        wave++;

        // ���� ���̺� * 1.5�� �ݿø� �� ���� ��ŭ ���� Ȱ��ȭ
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

    // �� ��ü�� �ı��Ǿ��� �� ȣ��� �޼���
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
        properties[EnemyCountKey] = enemyPool.Count; // ������ enemy ���� �� �Ӽ��� ����
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
