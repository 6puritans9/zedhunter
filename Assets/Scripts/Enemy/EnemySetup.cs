using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySetup : MonoBehaviourPunCallbacks
{
    EnemyHealth enemyHealth;
    //public EnemySpawnPool enemySpawnPool;
    private NavMeshAgent agent;
    private Vector3 saveDestination;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        agent = enemyHealth.nav;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Photon network is not connected!");
            return;
        }
            photonView.RPC("ReStartAction", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (!PhotonNetwork.IsConnected)
        {
            Debug.LogError("Photon network is not connected!");
            return;
        }
            photonView.RPC("StopAction", RpcTarget.All);
        if (PhotonNetwork.IsMasterClient)
        {
        }
    }
}
