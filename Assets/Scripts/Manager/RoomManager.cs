using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public GameObject player;
	public GameObject zombie;
	[Space]
	public Transform playerSpawnPoint;
	public Transform[] enemySpanwPoint;


	[SerializeField] public List<Player> players = new List<Player>();
	[SerializeField] public Dictionary<Player, GameObject> playerGameObjects = new Dictionary<Player, GameObject>();

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
}
