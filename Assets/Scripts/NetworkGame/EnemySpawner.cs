using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
     {
         public static EnemySpawner Instance;
     
         public Transform[] spawnPoints;
     
         public GameObject femaleZombiePrefab;
         public GameObject maleZombiePrefab;
         public GameObject bossZombiePrefab;
     
         public Queue<EnemyHealth> femaleZombiePool = new Queue<EnemyHealth>();
         public Queue<EnemyHealth> maleZombiePool = new Queue<EnemyHealth>();
         public Queue<EnemyHealth> bossZombiePool = new Queue<EnemyHealth>();
     
         private EnemyHealth enemyHealth;
     
         public int femaleZombiePoolSize;
         public int maleZombiePoolSize;
         public int bossZombiePoolSize;
     
         public Vector2 spawnAreaSize = new Vector2(30f, 30f); // ������ ������ ũ��
     
         [HideInInspector]public int maleEnemiesToSpawn;
         [HideInInspector] public int femaleEnemiesToSpawn;
         [HideInInspector] public int bossEnemiesToSpawn;
         private void Awake()
         {
             Instance = this;
     
             PhotonView photonView = GetComponent<PhotonView>();
     
             maleEnemiesToSpawn = 0;
             femaleEnemiesToSpawn = 0;
             bossEnemiesToSpawn = 0;
     
             enemyHealth = GetComponent<EnemyHealth>();
             spawnPoints[0] = transform.GetChild(0);
             spawnPoints[1] = transform.GetChild(1);
             spawnPoints[2] = transform.GetChild(2);
         }
         private void Start()
         {
             
             if (PhotonNetwork.IsMasterClient)
                 {
                     InitializeEnemyPool();
                 }
         }
     
         private void Update()
         {
             // ȣ��Ʈ�� ���� ���� ������ �� ����
             // �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
             if (PhotonNetwork.IsMasterClient)
             {
                 // ���� ��� ����ģ ��� ���� ���� ����
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
                 if (bossEnemiesToSpawn <= 0)
                 {
                     //MaleZombieSpawnWave();
                     photonView.RPC("BossZombieSpawnWave", RpcTarget.All);
                 }
     
                 //UpdateRoomProperties();
             }
         }
     
         // enemy �ʱ�ȭ
         void InitializeEnemyPool()
         {
             //femaleZombie
             for (int i = 0; i < femaleZombiePoolSize; i++)
             {
                 GameObject enemy = PhotonNetwork.Instantiate(femaleZombiePrefab.name, spawnPoints[0].position, Quaternion.identity);
                 EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                 femaleZombiePool.Enqueue(enemyHealth);
                 EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
                 enemy.SetActive(false);
             }
             //maleZombie
             for (int i = 0; i < maleZombiePoolSize; i++)
             {
                 GameObject enemy = PhotonNetwork.Instantiate(maleZombiePrefab.name, spawnPoints[1].position, Quaternion.identity);
                 EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                 maleZombiePool.Enqueue(enemyHealth);
                 EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
                 enemy.SetActive(false);
             }
             //bossmaleZombie
             for (int i = 0; i < bossZombiePoolSize; i++)
             {
                 GameObject enemy = PhotonNetwork.Instantiate(bossZombiePrefab.name, spawnPoints[2].position, Quaternion.identity);
                 EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                 bossZombiePool.Enqueue(enemyHealth);
                 EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
                 enemy.SetActive(false);
             }
         }
     
     
         [PunRPC]
         // ���� ���̺꿡 ���� ���� ����
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
                     enemy.transform.position = GetRandomSpawnPosition(spawnPoints[0].position);
                     //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[0].position);
                     //enemy.transform.position = randomSpawnPosition;
                     enemy.gameObject.SetActive(true);
                     //enemiesToSpawn--;
                 }
                 else
                 {
                     // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
                     femaleZombiePool.Enqueue(enemy);
                 }
             }
         }
     
         [PunRPC]
         // ���� ���̺꿡 ���� ���� ����
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
                     enemy.transform.position = GetRandomSpawnPosition(spawnPoints[1].position);
                     //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[1].position);
                     //enemy.transform.position = randomSpawnPosition;
                     enemy.gameObject.SetActive(true);
                     //enemiesToSpawn--;
                 }
                 else
                 {
                     // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
                     maleZombiePool.Enqueue(enemy);
                 }
             }
         }
     
         [PunRPC]
         // ���� ���̺꿡 ���� ���� ����
         private void BossZombieSpawnWave()
         {
             bossEnemiesToSpawn = bossZombiePoolSize;
             //int enemiesToSpawn = femaleZombiePoolSize;
             for (int i = 0; i < bossZombiePoolSize; i++)
             {
                 //EnemyHealth enemy = femaleZombiePool.Dequeue();
                 bossZombiePool.TryDequeue(out EnemyHealth enemy);
                 if(enemy)
                 {
                     if (!enemy.gameObject.activeSelf)
                     {
                         enemy.transform.position = GetRandomSpawnPosition(spawnPoints[2].position);
                         //Vector3 randomSpawnPosition = GetRandomSpawnPosition(spawnPoints[0].position);
                         //enemy.transform.position = randomSpawnPosition;
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
