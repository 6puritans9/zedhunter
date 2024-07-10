using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieController_Yaku : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent navAgent;
    private Animator animator;
    private Rigidbody rb;
    public float attackDistance = 3.3f;
    public float jumpForce = 5f;
    public float jumpCooldown = 5f;
    public float obstacleDetectionDistance = 1f;
    private bool isAttacking = false;
    private bool canAttack = true;
    private bool isJumping = false;
    private bool canJump = true;
    public float jumpHeight = 4f;
    public float jumpDistance = 4f;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (navAgent == null || animator == null || rb == null)
        {
            Debug.LogError("One or more required components are missing on the zombie!");
            enabled = false;
            return;
        }

        FindNearestPlayer();

        if (!navAgent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on a NavMesh.");
            enabled = false;
        }
    }

    void Update()
    {
        if (player == null)
        {
            FindNearestPlayer();
            if (player == null) return;
        }

        if (navAgent == null || !navAgent.isOnNavMesh) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance)
        {
            if (!isAttacking && canAttack)
            {
                StartAttack();
            }
        }
        else
        {
            if (isAttacking)
            {
                StopAttack();
            }

            if (!isJumping)
            {
                navAgent.SetDestination(player.position);
                float currentSpeed = navAgent.velocity.magnitude;
                bool isCurrentlyWalking = currentSpeed > 0.1f;

                UpdateAnimation(isCurrentlyWalking, currentSpeed);

                if (canJump && isCurrentlyWalking && DetectObstacle())
                {
                    Debug.Log("Obstacle detected, attempting to jump");
                    Jump();
                }
            }
        }
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;

        foreach (GameObject playerObj in players)
        {
            float distance = Vector3.Distance(transform.position, playerObj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestPlayer = playerObj;
            }
        }

        if (nearestPlayer != null)
        {
            player = nearestPlayer.transform;
        }
        else
        {
            Debug.LogError("No players found. Make sure player objects have the 'Player' tag.");
        }
    }

    bool DetectObstacle()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.5f; // 좀비의 "발" 위치에서 시작
        if (Physics.Raycast(rayStart, transform.forward, out hit, obstacleDetectionDistance))
        {
            if (!hit.collider.isTrigger && hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                Debug.DrawRay(rayStart, transform.forward * obstacleDetectionDistance, Color.red, 0.5f);
                return true;
            }
        }
        Debug.DrawRay(rayStart, transform.forward * obstacleDetectionDistance, Color.green, 0.5f);
        return false;
    }

    void Jump()
    {
        Debug.Log("Jump initiated");
        StartCoroutine(PerformJump());
    }

    IEnumerator PerformJump()
    {
        isJumping = true;
        canJump = false;
        navAgent.enabled = false;
        
        animator.SetTrigger("jump");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f); // 점프 지속 시간

        while (!IsGrounded())
        {
            yield return null;
        }

        navAgent.enabled = true;
        isJumping = false;

        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    private void StartAttack()
    {
        StartCoroutine(AttackPlayer());
    }

    private void StopAttack()
    {
        isAttacking = false;
        navAgent.isStopped = false;
    }

    private void UpdateAnimation(bool isWalking, float speed)
    {
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("speed", speed);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        navAgent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("attack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        isAttacking = false;
        navAgent.isStopped = false;
        canAttack = true;
    }

    public void OnAttackAnimationComplete()
    {
        Debug.Log("Attack animation complete");
        isAttacking = false;
        navAgent.isStopped = false;

        if (Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            StartAttack();
        }
    }
}