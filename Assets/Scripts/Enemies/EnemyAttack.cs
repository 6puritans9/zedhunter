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

    public GameObject targetPlayer;

    private void Awake()
    {
        Instance = this;
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    private void OnTriggerStay(Collider other)
    {
        //공격범위에 있는 플레이어 바라보기
        if (other.CompareTag("Player") && enemyHealth.Target != null)
        {
            targetPlayer = other.gameObject;
            canAttack = true;
            Vector3 dir = (enemyHealth.Target.transform.position - enemyHealth.transform.position).normalized;
            dir.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            enemyHealth.transform.rotation = Quaternion.Slerp(enemyHealth.transform.rotation, lookRotation, 1);
        }
    }

    public void DoAttack()
    {
        if (targetPlayer == null) return;
        if(targetPlayer.TryGetComponent(out ActionStateManager playerAction) && canAttack)
        {
            playerAction.TakeDamage(enemyAD);

            if (playerAction.playerHealth <= 0 && !playerAction.isDead)
                playerAction.isDead = true;
        }
    }
}



