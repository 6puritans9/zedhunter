using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPool : MonoBehaviourPunCallbacks
{
    public static PlayerSpawnPool Instance;

    public GameObject playerToSpawn;
    public List<GameObject> playerPool = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void OnPlayerKilled(ActionStateManager player)
    {
        player.gameObject.SetActive(false);
    }
}
