using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Photon.Pun;

public class EnemyController : MonoBehaviourPunCallbacks
{
    public float jumpHeight = 2f;
    public float jumpSpeed = 2f;
    public float chaseDistance = 5f;
    public float attackDistance = 2.5f;
    public float minJumpDistance = 4f;
    public float maxJumpDistance = 8f;
    public float rotationSpeed = 5f;

    public NavMeshAgent agent;
    public Animator animator;
    public Vector3 jumpStartPosition;
    public Vector3 jumpTargetPosition;
    public float jumpStartTime;
    public bool isJumping = false;
    public bool isJumpAnimating = false;

    EnemyHealth enemyHealth;
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponentInParent<PhotonView>();
    }

    void Start()
    {
        enemyHealth = GetComponentInParent<EnemyHealth>();
        agent = GetComponentInParent<NavMeshAgent>();
        animator = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (enemyHealth.Target)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, enemyHealth.Target.position);

            if (distanceToPlayer >= minJumpDistance && distanceToPlayer <= maxJumpDistance && !isJumping)
            {
                // 플레이어가 점프 거리 내에 있으면 점프
                photonView.RPC("StartJump", RpcTarget.All);
            }
        }

        if (isJumping)
        {
            PerformJump();
        }

        Debug.Log(isJumpAnimating);
    }
    

    [PunRPC]
    void StartJump()
    {
        isJumping = true;
        isJumpAnimating = true;
        agent.isStopped = true;
        jumpStartPosition = transform.position;
        jumpTargetPosition = enemyHealth.Target.position;
        jumpStartTime = Time.time;

        // 점프 높이를 고려한 포물선 궤적 계산
        Vector3 jumpDirection = (jumpTargetPosition - jumpStartPosition).normalized;
        float jumpDistance = Mathf.Min(Vector3.Distance(jumpStartPosition, jumpTargetPosition), maxJumpDistance);
        jumpTargetPosition = jumpStartPosition + jumpDirection * jumpDistance;
        jumpTargetPosition.y = jumpStartPosition.y;
    }

    void PerformJump()
    {
        float elapsedTime = Time.time - jumpStartTime;
        float jumpProgress = elapsedTime / jumpSpeed;
        animator.SetBool("isJumping", isJumpAnimating);
        if (jumpProgress < 1f)
        {
            // 포물선 운동 계산
            Vector3 currentPosition = Vector3.Lerp(jumpStartPosition, jumpTargetPosition, jumpProgress);
            currentPosition.y = jumpStartPosition.y + Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight;

            transform.position = currentPosition;
        }
        else
        {
            LandJump();
        }
    }

    void LandJump()
    {
        isJumping = false;
        agent.isStopped = false;

        isJumpAnimating = false;
        animator.SetBool("isJumping", isJumpAnimating);
    }
}
