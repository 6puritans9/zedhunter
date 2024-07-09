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
	private int wave = 1; // 현재 웨이브

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
		// 호스트만 적을 직접 생성할 수 있음
		// 다른 클라이언트들은 호스트가 생성한 적을 동기화를 통해 받아옴
		if (PhotonNetwork.IsMasterClient)
		{
			// 적을 모두 물리친 경우 다음 스폰 실행
			if (enemyList.Count <= 0)
			{
				SpawnWave();
			}
			UpdateRoomProperties();
		}
	}

	// 현재 웨이브에 맞춰 적을 생성
	private void SpawnWave()
	{
		// 웨이브 1 증가
		wave++;

		// 현재 웨이브 * 1.5에 반올림 한 개수 만큼 적을 생성
		int spawnCount = Mathf.RoundToInt(wave * 1.5f);

		// spawnCount 만큼 적을 생성
		for (int i = 0; i < spawnCount; i++)
		{
			// 적 프리팹으로부터 적을 생성, 네트워크 상의 모든 클라이언트들에게 생성됨
			GameObject createdEnemy = PhotonNetwork.Instantiate(enemyPrefab.gameObject.name,
				transform.position,
				Quaternion.identity);
			// 생성된 적을 리스트에 추가
			enemyList.Add(createdEnemy.GetComponent<EnemyHealth>());

			// 적 객체의 파괴 이벤트에 대한 리스너 추가
			createdEnemy.GetComponent<EnemyHealth>().OnEnemyKilled += OnEnemyKilled;
		}
	}

	// 적 객체가 파괴되었을 때 호출될 메서드
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
		properties[EnemyCountKey] = enemyList.Count; // 생성된 enemy 수를 방 속성에 저장
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
