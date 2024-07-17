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
            Debug.Log("¿¬°á²÷±è");
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("ReStartAction", RpcTarget.All);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("¿¬°á²÷±è");
            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            /*if (enemyHealth.isDead)
                photonView.RPC("StopAction", RpcTarget.All);*/
        }
    }
}
