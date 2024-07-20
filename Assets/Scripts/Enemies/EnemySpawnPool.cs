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
        private int wave = 3; // ���� ���̺�

        public int poolSize;

        private void Awake()
            {
                Instance = this;
                enemyHealth = GetComponent<EnemyHealth>();
            }

        private void Start()
            {
                // Ŭ���̾�Ʈ�� �濡 ���� InitializeEnemyPool�� ȣ��� ���Դϴ�.
            }

        public override void OnJoinedRoom()
            {
                // Ŭ���̾�Ʈ�� �濡 ���� �� ȣ��˴ϴ�.
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
                        if (enemyPool.Count <= poolSize)
                            {
                                SpawnWave();
                                //photonView.RPC("SpawnWave", RpcTarget.All);
                            }

                        UpdateRoomProperties();
                    }
            }

        // enemy �ʱ�ȭ
        void InitializeEnemyPool()
            {
                for (int i = 0; i < poolSize; i++)
                    {
                        GameObject enemy =
                            PhotonNetwork.Instantiate(enemyPrefab.name, transform.position, Quaternion.identity);
                        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                        enemyPool.Enqueue(enemyHealth);
                        EnemyHealth.Instance.OnEnemyKilled += OnEnemyKilled;
                        enemy.SetActive(false);
                    }
            }


        [PunRPC]
        // ���� ���̺꿡 ���� ���� ����
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
                                // �ٽ� ť�� �־� ��Ȱ��ȭ�� ���� ã�� ������ �ݺ�
                                enemyPool.Enqueue(enemy);
                            }
                    }
            }

        // �� ��ü�� �ı��Ǿ��� �� ȣ��� �޼���
        private void OnEnemyKilled(EnemyHealth enemy)
            {
                //Debug.Log("�� ȣ��ƾ�!");
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
    }