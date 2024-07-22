using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
    {
        private GameManager _gameManager;
        private PlayerManager _playerManager;
        public GameObject[] playerPrefabs;
        public Transform playerSpawnPoint;
    
        private void Start()
            {
                _gameManager = FindObjectOfType<GameManager>();
                _playerManager = FindObjectOfType<PlayerManager>();
                
                SpawnPlayer(_gameManager.GetPlayerAvatar());
            }

        private void SpawnPlayer(int index)
            {
                int avatarIndex = index;
                
                GameObject playerToSpawn = playerPrefabs[avatarIndex];
                if (playerToSpawn == null)
                    {
                        Debug.LogError("Player prefab to spawn is null.");
                        return;
                    }

                GameObject player = Instantiate(playerToSpawn, playerSpawnPoint.position, Quaternion.Euler(0, 180, 0));
            }
    }