using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameManager gameManager;
    public GameObject[] playerPrefabs;
    public Transform spawnPoint;

    private void Start()
    {
        if (!PhotonNetwork.IsMessageQueueRunning)
        {
            PhotonNetwork.IsMessageQueueRunning = true;
        }

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
            return;
        }

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("playerAvatar", out object avatarIndexObj))
        {
            int avatarIndex = (int)avatarIndexObj;
            Debug.Log($"Avatar index: {avatarIndex}");

            if (avatarIndex < 0 || avatarIndex >= playerPrefabs.Length)
            {
                Debug.LogError("Avatar index is out of range of playerPrefabs array.");
                return;
            }

            GameObject playerToSpawn = playerPrefabs[avatarIndex];
            if (playerToSpawn == null)
            {
                Debug.LogError("Player prefab to spawn is null.");
                return;
            }

            // Debug.Log($"Instantiating player: {playerToSpawn.name} at position {spawnPoint.position}");
            GameObject player = PhotonNetwork.Instantiate(playerToSpawn.name, spawnPoint.position, Quaternion.Euler(0, 180, 0));
            if (player == null)
            {
                Debug.LogError("Failed to instantiate player.");
                return;
            }
            
            //debug
            PhotonView playerPhotonView = player.GetComponentInChildren<PhotonView>();
            if (playerPhotonView != null)
                {
                    Debug.Log($"Player instantiated successfully with PhotonView ID: {playerPhotonView.ViewID}");
                }
            else
                {
                    Debug.LogError("Player instantiated but PhotonView component is missing.");
                }

            // Debug.Log("Player instantiated successfully.");
            gameManager.InitializePlayer(player);
        }
        else
        {
            Debug.LogError("Player avatar not found in CustomProperties.");
        }
    }
}