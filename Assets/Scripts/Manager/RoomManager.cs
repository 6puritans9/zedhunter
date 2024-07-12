using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public GameObject player;
	public GameObject zombie;
	[Space]
	public Transform playerSpawnPoint;
	public Transform[] enemySpanwPoint;


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

		GameObject _player = PhotonNetwork.Instantiate(player.name, playerSpawnPoint.position, Quaternion.identity);
		_player.GetComponentInChildren<PlayerSetup>().IsLocalPlayer();
		if (PhotonNetwork.IsMasterClient)
			enemySpanwPoint[0].GetComponent<EnemySpawnTemp>().SyncStateWithNewPlayer();
	}
}
