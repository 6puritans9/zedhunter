using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviourPun
{
    public static EnemyAttack Instance;
    private EnemyHealth enemyHealth;
    private Coroutine attackCoroutine;

    public int enemyAD;
    public bool canAttack;

    private void Awake()
    {
        Instance = this;
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enemyHealth.Target != null)
        {
            canAttack = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && enemyHealth.Target != null)
        {
            canAttack = true;
            Vector3 dir = (enemyHealth.Target.transform.position - enemyHealth.transform.position).normalized;
            dir.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            enemyHealth.transform.rotation = Quaternion.Slerp(enemyHealth.transform.rotation, lookRotation, 1);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        canAttack = false;
    }

    public void DoAttack(GameObject target)
    {
        if(target.TryGetComponent(out ActionStateManager playerAction))
        {
            PhotonView playerPhotonView = playerAction.GetComponent<PhotonView>();
            playerPhotonView.RPC("TakeDamage", RpcTarget.All, enemyAD);

            if (playerAction.playerHealth <= 0 && !playerAction.isDead)
                playerAction.isDead = true;
        }
    }
}



