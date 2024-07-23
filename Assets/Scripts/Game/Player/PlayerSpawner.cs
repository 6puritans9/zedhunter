using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
	private GameManager _gameManager;
	private PlayerManager _playerManager;

	[Header("Spawn")]
	public GameObject[] playerPrefabs;
	public Transform playerSpawnPoint;

	[Header("Respawn")]
	private GameObject _player;

	private void Start()
	{
		_gameManager = FindObjectOfType<GameManager>();
		_playerManager = FindObjectOfType<PlayerManager>();

		SpawnPlayer();
	}

	// For Debug
	private void SpawnPlayer()
	{
		GameObject playerToSpawn = playerPrefabs[0];
		if (playerToSpawn == null)
		{
			Debug.LogError("Player prefab to spawn is null.");
			return;
		}

		GameObject player = Instantiate(playerToSpawn, playerSpawnPoint.position, Quaternion.Euler(0, 180, 0));
	}

	// TODO: Restore Here
	// private void SpawnPlayer(int index)
	//     {
	//         int avatarIndex = index;
	//         
	//         GameObject playerToSpawn = playerPrefabs[avatarIndex];
	//         if (playerToSpawn == null)
	//             {
	//                 Debug.LogError("Player prefab to spawn is null.");
	//                 return;
	//             }
	//
	//         GameObject player = Instantiate(playerToSpawn, playerSpawnPoint.position, Quaternion.Euler(0, 180, 0));
	//     }

	public void RespawnPlayer()
	{
		Debug.Log("ActionStateManager: RespawnPlayer called");

		// if (GameManager.Instance == null)
		// {
		//     Debug.LogError("ActionStateManager: GameManager.Instance is null!");
		//     return;
		// }

		// if (GameManager.Instance.playerSpawnPoint == null)
		// {
		//     Debug.LogError("ActionStateManager: GameManager.Instance.playerSpawnPoint is null!");
		//     return;
		// }


		transform.position = playerSpawnPoint.position;
		_playerManager.isDead = false;
		_playerManager.currentPlayerHealth = 100;

		// photonView.RPC("RPC_RespawnPlayer", RpcTarget.All, photonView.ViewID);

		_player.SetActive(true);
	}

	// public void RPC_RespawnPlayer(int viewID)
	//     {
	//         Debug.Log($"ActionStateManager: RPC_RespawnPlayer called with viewID: {viewID}");
	//         if (targetView != null)
	//             {
	//                 targetView.gameObject.SetActive(true);
	//             }
	//         else
	//             {
	//                 Debug.LogError(
	//                     $"ActionStateManager: PhotonView with ID {viewID} not found in RPC_RespawnPlayer!");
	//             }
	//     }

}