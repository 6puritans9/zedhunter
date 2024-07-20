// GameManager.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance;

        [Space] public Transform playerSpawnPoint;
        public Transform[] enemySpawnPoints;
        public Vector2 spawnAreaSize = new Vector2(2f, 2f);

        [SerializeField] public List<Player> players = new List<Player>();
        [SerializeField] public Dictionary<Player, PhotonView> playerGameObjects = new Dictionary<Player, PhotonView>();

        private void Awake()
            {
                Instance = this;
            }

        // public void InitializePlayer(GameObject player)
        //     {
        //         Debug.Log("GameManager InitializePlayer method called.");
        //
        //         if (player == null)
        //             {
        //                 Debug.LogError("Player object passed to InitializePlayer is null.");
        //                 return;
        //             }
        //
        //         Debug.Log($"Initializing player: {player.name}");
        //
        //         PhotonView photonView = player.GetComponent<PhotonView>();
        //         if (photonView == null)
        //             {
        //                 Debug.LogError("PhotonView component not found on player.");
        //                 return;
        //             }
        //
        //         if (photonView.IsMine)
        //             {
        //                 PlayerSetup playerSetup = player.GetComponentInChildren<PlayerSetup>();
        //                 if (playerSetup == null)
        //                     {
        //                         Debug.LogError("PlayerSetup component not found on player.");
        //                         return;
        //                     }
        //
        //                 playerSetup.IsLocalPlayer();
        //                 Debug.Log("PlayerSetup IsLocalPlayer method called.");
        //             }
        //
        //         players.Clear();
        //         playerGameObjects.Clear();
        //
        //         foreach (Player p in PhotonNetwork.PlayerList)
        //             {
        //                 players.Add(p);
        //                 Debug.Log($"Added player: {p.NickName}");
        //
        //                 GameObject playerObject = GetPlayerGameObject(p);
        //                 if (playerObject != null)
        //                     {
        //                         playerGameObjects.Add(p, playerObject);
        //                         Debug.Log($"Added player object for: {p.NickName}");
        //                     }
        //                 else
        //                     {
        //                         Debug.LogError($"Player object for {p.NickName} is null.");
        //                     }
        //             }
        //
        //         if (PhotonNetwork.IsMasterClient)
        //             {
        //                 // Optional: Sync state with new player if master client
        //                 // enemySpawnPoints[0].GetComponent<EnemySpawnPool>().SyncStateWithNewPlayer();
        //             }
        //     }

        public void InitializePlayer(GameObject player)
            {
                Debug.Log($"InitializePlayer called for: {player.name}");

                PhotonView photonView = player.GetComponent<PhotonView>();
                if (photonView == null)
                    {
                        Debug.LogError("PhotonView component not found on player.");
                        return; // Return here instead of yield return null, as this is not a coroutine
                    }

                if (photonView.IsMine)
                    {
                        PlayerSetup playerSetup = player.GetComponentInChildren<PlayerSetup>();
                        if (playerSetup == null)
                            {
                                Debug.LogError("PlayerSetup component not found on player.");
                                return;
                            }

                        playerSetup.IsLocalPlayer();
                        Debug.Log("PlayerSetup IsLocalPlayer method called.");
                    }

                StartCoroutine(DelayedInitializePlayer(player));
            }

        private IEnumerator DelayedInitializePlayer(GameObject player)
            {
                yield return new WaitForSeconds(0.5f); // Adjust this delay as needed

                Debug.Log($"Delayed initialization starting for player: {player.name}");

                // Wait until the local player's GameObject is available
                yield return new WaitUntil(() => GetPlayerGameObject(player.GetComponent<PhotonView>().Owner) != null);

                players.Clear();
                playerGameObjects.Clear();

                foreach (Player p in PhotonNetwork.PlayerList)
                    {
                        players.Add(p);
                        Debug.Log($"Added player to list: {p.NickName}");

                        PhotonView playerObject = GetPlayerGameObject(p);
                        if (playerObject != null)
                            {
                                playerGameObjects.Add(p, playerObject);
                                Debug.Log($"Added player object for: {p.NickName}");
                            }
                        else
                            {
                                Debug.LogWarning(
                                    $"Player object for {p.NickName} is still null after delay. Retrying...");
                                yield return StartCoroutine(RetryGetPlayerObject(p));
                            }
                    }

                Debug.Log("Delayed initialization completed for all players.");
            }

        private IEnumerator RetryGetPlayerObject(Player player, int maxRetries = 5)
            {
                for (int i = 0; i < maxRetries; i++)
                    {
                        yield return new WaitForSeconds(0.5f);
                        PhotonView playerObject = GetPlayerGameObject(player);
                        if (playerObject != null)
                            {
                                playerGameObjects.Add(player, playerObject);
                                Debug.Log($"Successfully added player object for {player.NickName} after retry");
                                yield break;
                            }
                    }

                Debug.LogError($"Failed to get player object for {player.NickName} after {maxRetries} retries");
            }

        private PhotonView GetPlayerGameObject(Player player)
            {
                foreach (var playerObject in GameObject.FindGameObjectsWithTag("Player"))
                    {
                        PhotonView photonView = playerObject.GetComponentInChildren<PhotonView>();
                        // print($"photon view: {photonView}");
                        // print($"playerObject: {playerObject}");

                        if (photonView != null && photonView.Owner == player)
                            {
                                return photonView;
                            }
                    }

                return null;
            }

        Vector3 GetRandomSpawnPosition(Vector3 center)
            {
                float halfWidth = spawnAreaSize.x / 2f;
                float halfHeight = spawnAreaSize.y / 2f;

                float randomX = Random.Range(-halfWidth, halfWidth);
                float randomZ = Random.Range(-halfHeight, halfHeight);

                Vector3 randomSpawnPosition = new Vector3(randomX, 0f, randomZ) + center;

                return randomSpawnPosition;
            }
    }
