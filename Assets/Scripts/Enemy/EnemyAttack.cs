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

    public GameObject target;

    private void Awake()
    {
        Instance = this;
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (enemyHealth.zombieType == EnemyHealth.ZombieType.Female)
        {
            //공격범위에 있는 플레이어 바라보기
            if (other.CompareTag("Player") && enemyHealth.Target != null)
            {
                target = other.gameObject;
                canAttack = true;
                Vector3 dir = (enemyHealth.Target.transform.position - enemyHealth.transform.position).normalized;
                dir.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                enemyHealth.transform.rotation = Quaternion.Slerp(enemyHealth.transform.rotation, lookRotation, 1);
            }
        }
        else if(enemyHealth.zombieType == EnemyHealth.ZombieType.Male)
        {
            if ((other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Wall")) && enemyHealth.Target != null)
            {
                target = other.gameObject;
                canAttack = true;
                Vector3 dir = (enemyHealth.Target.transform.position - enemyHealth.transform.position).normalized;
                dir.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                enemyHealth.transform.rotation = Quaternion.Slerp(enemyHealth.transform.rotation, lookRotation, 1);
            }
        }
        else if (enemyHealth.zombieType == EnemyHealth.ZombieType.Boss)
        {
            if (other.CompareTag("Player") && enemyHealth.Target != null)
            {
                target = other.gameObject;
                canAttack = true;
                Vector3 dir = (enemyHealth.Target.transform.position - enemyHealth.transform.position).normalized;
                dir.y = 0;
                Quaternion lookRotation = Quaternion.LookRotation(dir);
                enemyHealth.transform.rotation = Quaternion.Slerp(enemyHealth.transform.rotation, lookRotation, 1);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || other.gameObject.layer == LayerMask.NameToLayer("Wall")) && enemyHealth.Target != null)
        {
            canAttack = false;
        }
    }

    public void DoAttack()
    {
        if (target == null || !canAttack)
        {
            return;
        }

        if(target.TryGetComponent(out ActionStateManager playerAction) && canAttack)
        {
            playerAction.TakeDamage(enemyAD);

            if (playerAction.playerHealth <= 0 && !playerAction.isDead)
                playerAction.isDead = true;
        }
        else if(target.TryGetComponent(out WallHP wall))
        {
            wall.TakeDamage(enemyAD);
        }
    }
}



