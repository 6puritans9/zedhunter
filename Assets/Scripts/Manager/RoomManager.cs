using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager Instance;

	public GameObject player;
	public GameObject zombie;
	[Space]
	public Transform playerSpawnPoint;
	public Transform[] enemySpanwPoint;
	public Vector2 spawnAreaSize = new Vector2(2f, 2f); // 스폰할 영역의 크기

	[SerializeField] public List<Player> players = new List<Player>();
	[SerializeField] public Dictionary<Player, GameObject> playerGameObjects = new Dictionary<Player, GameObject>();

    private void Awake()
    {
		Instance = this;

	}

    // Start is called before the first frame update
    void Start()
	{
		Debug.Log("Connecting...");
		PhotonNetwork.PhotonServerSettings.DevRegion = "kr";

		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		base.OnConnectedToMaster();

		Debug.Log("Connected to Server");

		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		base.OnJoinedLobby();

		PhotonNetwork.JoinOrCreateRoom("test", null, null);

		Debug.Log("We're in the lobby!!");
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		Debug.Log("We're connected and in a room!!!");

		// 플레이어 스폰 위치 설정
		Vector3 randomSpawnPosition = GetRandomSpawnPosition(playerSpawnPoint.position);

		GameObject _player = PhotonNetwork.Instantiate(player.name, randomSpawnPosition, Quaternion.identity);

		_player.GetComponentInChildren<PlayerSetup>().IsLocalPlayer();

		players.Clear();
		playerGameObjects.Clear();

        foreach (Player p in PhotonNetwork.PlayerList)
        {
			players.Add(p);

			GameObject playerObject = GetPlayerGameObject(p);
			if(playerGameObjects != null)
            {
				playerGameObjects.Add(p, playerObject);
            }
        }

		/*if (PhotonNetwork.IsMasterClient)
			enemySpanwPoint[0].GetComponent<EnemySpawnPool>().SyncStateWithNewPlayer();*/
	}

    private GameObject GetPlayerGameObject(Player player)
    {
        foreach (var go in GameObject.FindGameObjectsWithTag("Player"))
        {
			PhotonView photonView = go.GetComponent<PhotonView>();

			if (photonView != null && photonView.Owner == player)
				return go;
        }
		return null;
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
