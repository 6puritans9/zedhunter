using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieController_man : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent navAgent;
    private Animator animator;
    public float attackDistance = 3.3f;  // 공격 거리 설정
    private bool isAttacking = false;
    private bool canAttack = true;

    void Start()
    {
        // "Player" 태그를 가진 오브젝트를 찾습니다.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found. Make sure the player object has the 'Player' tag.");
        }

        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!navAgent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh.");
        }
    }

    void Update()
    {
        if (player != null && navAgent.isOnNavMesh)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackDistance)
            {
                if (!isAttacking && canAttack)
                {
                    StartCoroutine(AttackPlayer());
                }
            }
            else
            {
                if (isAttacking)
                {
                    isAttacking = false;
                    navAgent.isStopped = false;
                }

                navAgent.SetDestination(player.position);
                float speed = navAgent.velocity.magnitude;
                
                if (speed > 0.1f)
                {
                    animator.SetBool("isWalking", true);
                    animator.SetFloat("speed", speed);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    animator.SetFloat("speed", 0f);
                }
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        navAgent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");

        // 애니메이션의 길이를 기다립니다.
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;
        navAgent.isStopped = false;
        canAttack = true;
    }

    // 공격 애니메이션 끝나면 호출되는 이벤트
    public void OnAttackAnimationComplete()
    {
        Debug.Log("Attack animation complete");
        isAttacking = false;
        navAgent.isStopped = false;

        // 공격 애니메이션이 끝난 후 다시 공격할 수 있도록 설정
        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            StartCoroutine(AttackPlayer());
        }
    }
}
