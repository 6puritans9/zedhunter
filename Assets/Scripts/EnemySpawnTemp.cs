using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;

public class EnemySpawnTemp : MonoBehaviourPun
{
	public static EnemySpawnTemp Instance;

	public GameObject enemyPrefab;
	public List<EnemyHealth> enemyList = new List<EnemyHealth>();
	private const string EnemyCountKey = "EnemyCount";
	private int wave = 1; // ���� ���̺�

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
		// ȣ��Ʈ�� ���� ���� ������ �� ����
		// �ٸ� Ŭ���̾�Ʈ���� ȣ��Ʈ�� ������ ���� ����ȭ�� ���� �޾ƿ�
		if (PhotonNetwork.IsMasterClient)
		{
			// ���� ��� ����ģ ��� ���� ���� ����
			if (enemyList.Count <= 0)
			{
				SpawnWave();
			}
			UpdateRoomProperties();
		}
	}

	// ���� ���̺꿡 ���� ���� ����
	private void SpawnWave()
	{
		// ���̺� 1 ����
		wave++;

		// ���� ���̺� * 1.5�� �ݿø� �� ���� ��ŭ ���� ����
		int spawnCount = Mathf.RoundToInt(wave * 1.5f);

		// spawnCount ��ŭ ���� ����
		for (int i = 0; i < spawnCount; i++)
		{
			// �� ���������κ��� ���� ����, ��Ʈ��ũ ���� ��� Ŭ���̾�Ʈ�鿡�� ������
			GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name,
				transform.position,
				Quaternion.identity);
			// ������ ���� ����Ʈ�� �߰�
			enemyList.Add(createdEnemy.GetComponent<EnemyHealth>());

			// �� ��ü�� �ı� �̺�Ʈ�� ���� ������ �߰�
			createdEnemy.GetComponent<EnemyHealth>().OnEnemyKilled += OnEnemyKilled;
		}
	}

	// �� ��ü�� �ı��Ǿ��� �� ȣ��� �޼���
	private void OnEnemyKilled(EnemyHealth enemy)
	{
		enemyList.Remove(enemy);
		UpdateRoomProperties();
	}

	private void UpdateRoomProperties()
	{
		Hashtable properties = new Hashtable();
		for (int i = enemyList.Count - 1; i >= 0; i--)
		{
			if (enemyList[i] == null)
			{
				enemyList.RemoveAt(i);
				continue;
			}

			properties[$"enemyPosition_{i}"] = enemyList[i].transform.position;
			properties[$"enemyRotation_{i}"] = enemyList[i].transform.rotation;
		}
		properties[EnemyCountKey] = enemyList.Count; // ������ enemy ���� �� �Ӽ��� ����
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
            enemyList.Add(_enemy.GetComponent<EnemyHealth>());
            i++;
        }*/
	}
}
