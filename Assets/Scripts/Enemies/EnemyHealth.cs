

// using Photon.Pun;
// using Photon.Realtime;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.AI;

// public class EnemyHealth : MonoBehaviourPun
// {
//     public static EnemyHealth Instance;

//     public LayerMask whatIsTarget; // 타겟 레이어

//     public Coroutine UpdateTargetCorutine;

//     public Animator anim;

//     [HideInInspector] public Transform Target;
//     public bool isChase;
//     [HideInInspector] public NavMeshAgent nav;

//     public Collider[] colliders;
//     float closestDistance = Mathf.Infinity;

//     public delegate void EnemyKilledHandler(EnemyHealth enemy);
//     public event EnemyKilledHandler OnEnemyKilled;

//     public BoxCollider attackRange;

//     public float health = 20;
//     [HideInInspector] public bool isDead;

//     EnemyAttack enemyAttack;
//     bool isAttack;

//     private void Awake()
//     {
//         Instance = this;
//         enemyAttack = GetComponentInChildren<EnemyAttack>();
//         nav = GetComponent<NavMeshAgent>();
//         anim = GetComponent<Animator>();
//     }

//     private void Start()
//     {
//         isAttack = false;
//         isChase = true;
//         isDead = false;
//         StartCoroutine(UpdateTarget());
//     }

//     IEnumerator UpdateTarget()
//     {
//         while (!isDead)
//         {
//             // 매 업데이트마다 closestDistance를 초기화
//             closestDistance = Mathf.Infinity;
//             // 15 거리 내의 타겟 레이어를 가진 콜라이더들 찾기
//             colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
//             GameObject closestTarget = null;

//             // 모든 콜라이더를 확인하며, 가장 가까운 플레이어 찾기
//             for (int i = 0; i < colliders.Length; i++)
//             {
//                 GameObject livingEntity = colliders[i].gameObject;

//                 if (livingEntity.layer == 7)
//                 {
//                     float distanceToTarget = Vector3.Distance(transform.position, livingEntity.transform.position);
//                     if (distanceToTarget < closestDistance)
//                     {
//                         closestDistance = distanceToTarget;
//                         closestTarget = livingEntity;
//                     }

//                     if (closestTarget != null)
//                     {
//                         Target = closestTarget.transform;
//                         photonView.RPC(
//                             "SetTarget",
//                             RpcTarget.AllBuffered,
//                             closestTarget.GetComponent<PhotonView>().ViewID);
//                     }
//                 }
//             }

//             // 타겟이 없으면 추적 중지
//             if (colliders.Length <= 0)
//             {
//                 isChase = false;
//                 Target = null;
//             }
//             else
//                 isChase = true;

//             if (Target)
//             {
//                 float speed = 0;

//                 float targetRadius = 0.5f;
//                 float targetRange = 0.5f;
//                 RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
//                                                          targetRadius, transform.forward,
//                                                          targetRange, LayerMask.GetMask("Player"));

//                 if (rayHits.Length > 0 && !isAttack)
//                 {
//                     foreach (RaycastHit hit in rayHits)
//                     {
//                         GameObject hitObject = hit.collider.gameObject;
//                         if (hitObject.TryGetComponent(out ActionStateManager player))
//                         {
//                             enemyAttack.targetPlayer = player.gameObject;
//                             break;
//                         }
//                     }
//                     StartCoroutine(Attack());
//                 }

//                 // 추적 중이면 네비게이션 이동
//                 if (!isAttack)
//                 {
//                     nav.isStopped = false;
//                     speed = nav.velocity.magnitude;
//                 }
//                 else
//                     nav.isStopped = true;

//                 if (speed > 0.1f)
//                 {
//                     anim.SetBool("isWalking", true);
//                     anim.SetFloat("speed", speed);
//                 }
//                 else
//                 {
//                     anim.SetBool("isWalking", false);
//                     anim.SetFloat("speed", 0f);
//                 }

//                 nav.SetDestination(Target.transform.position);
//             }
//             else
//             {
//                 nav.isStopped = true; // 타겟이 없으면 AI 이동 중지
//                 SetRandomDestination();
//             }
//             yield return new WaitForSeconds(0.25f);
//         }
//     }

//     IEnumerator Attack()
//     {
//         isChase = false;
//         isAttack = true;
//         anim.SetBool("isAttack", true);

//         yield return new WaitForSeconds(1f);

//         isChase = true;
//         isAttack = false;
//         anim.SetBool("isAttack", false);
//     }

//     public void DoAttack()
//     {
//         if (enemyAttack != null)
//             enemyAttack.DoAttack();
//     }

//     public void StopUpdateTargetCorutine()
//     {
//         if (UpdateTargetCorutine != null)
//         {
//             StopCoroutine(UpdateTarget());
//             UpdateTargetCorutine = null;
//         }
//     }

//     public void StartUpdateTargetCorutine()
//     {
//         if (UpdateTargetCorutine == null)
//         {
//             UpdateTargetCorutine = StartCoroutine(UpdateTarget());
//         }
//     }

//     private void SetRandomDestination()
//     {
//         anim.SetBool("isWalking", true);
//         anim.SetFloat("speed", nav.velocity.magnitude);

//         Vector3 randomDir = Random.insideUnitCircle * 2;
//         randomDir += transform.position;
//         NavMeshHit navHit;
//         NavMesh.SamplePosition(randomDir, out navHit, 1, NavMesh.AllAreas);

//         nav.SetDestination(navHit.position);
//         nav.isStopped = false;
//     }

//     [PunRPC]
//     public void TakeDamage(float damage)
//     {
//         if (health > 0)
//         {
//             health -= damage;
//             anim.SetTrigger("damage");
//             if (PhotonNetwork.IsMasterClient)
//             {
//                 // 마스터 클라이언트에서만 죽음 처리
//                 if (health <= 0)
//                 {
//                     photonView.RPC("EnemyDeath", RpcTarget.All);
//                     StartCoroutine(RemoveFromSceneAfterDelay(2f));
//                 }
//             }
//         }
//     }

//     IEnumerator RemoveFromSceneAfterDelay(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         if (PhotonNetwork.IsMasterClient)
//         {
//             PhotonNetwork.Destroy(gameObject);
//         }
//     }

//     [PunRPC]
//     void EnemyDeath()
//     {
//         if (isDead) return;

//         if (photonView.IsMine)
//         {
//             photonView.RPC("StopAction", RpcTarget.All);

//             // OnEnemyKilled 이벤트 호출
//             OnEnemyKilled?.Invoke(this);
//         }
//         anim.SetTrigger("death");
//     }

//     [PunRPC]
//     public void StopAction()
//     {
//         isDead = true;
//         isChase = false;

//         nav.enabled = false;
//         anim.enabled = false;

//         UpdateTargetCorutine = null;
//     }

//     [PunRPC]
//     public void ReStartAction()
//     {
//         if (nav.enabled == false)
//             nav.enabled = true;
//         anim.enabled = true;

//         isDead = false;
//         isChase = true;
//         health = 25;

//         StartUpdateTargetCorutine();
//     }

//     [PunRPC]
//     void SetTarget(int targetViewID)
//     {
//         PhotonView targetPhotonView = PhotonView.Find(targetViewID);
//         if (targetPhotonView != null)
//         {
//             Target = targetPhotonView.transform;
//         }
//     }
// }


using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
    public static EnemyHealth Instance;

    public enum ZombieType { Male, Female, Boss }
    public ZombieType zombieType;

    public LayerMask whatIsTarget;

    public Coroutine UpdateTargetCorutine;

    public Animator anim;

    [HideInInspector] public Transform Target;
    public bool isChase;
    [HideInInspector] public NavMeshAgent nav;

    public Collider[] colliders;
    float closestDistance = Mathf.Infinity;

    public delegate void EnemyKilledHandler(EnemyHealth enemy);
    public event EnemyKilledHandler OnEnemyKilled;

    public BoxCollider attackRange;

    public float health;
    [HideInInspector] public bool isDead;

    EnemyAttack enemyAttack;
    bool isAttack;

    // Boss specific parameters
    public float chaseDistance = 5f;
    public float minJumpDistance = 4f;
    public float maxJumpDistance = 8f;
    public float jumpSpeed = 2f;
    public float jumpHeight = 2f;
    public float jumpCooldown = 3f;


    // Visual effects
    public GameObject bloodSplashPrefab;

    private void Awake()
    {
        Instance = this;
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        InitializeZombie();
        StartCoroutine(UpdateTarget());
    }

    private void InitializeZombie()
    {
        isAttack = false;
        isChase = true;
        isDead = false;

        switch (zombieType)
        {
            case ZombieType.Female:
                health = 5;
                break;
            case ZombieType.Male:
                health = 5;
                break;
            case ZombieType.Boss:
                health = 10;
                break;
        }
    }

    IEnumerator UpdateTarget()
    {
        while (!isDead)
        {
            closestDistance = Mathf.Infinity;
            GameObject closestTarget = null;

            float detectionRange = GetDetectionRange();
            colliders = Physics.OverlapSphere(transform.position, detectionRange, whatIsTarget);

            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject targetObject = colliders[i].gameObject;
                if (IsValidTarget(targetObject))
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = targetObject;
                    }
                }
            }

            if (closestTarget != null)
            {
                Target = closestTarget.transform;
                photonView.RPC("SetTarget", RpcTarget.AllBuffered, closestTarget.GetComponent<PhotonView>().ViewID);
            }

            HandleMovementAndAttack();

            yield return new WaitForSeconds(0.25f);
        }
    }

    private float GetDetectionRange()
    {
        switch (zombieType)
        {
            case ZombieType.Female: return 300f;
            case ZombieType.Male: return 300f;
            case ZombieType.Boss: return 300f;
            default: return 300f;
        }
    }

    private bool IsValidTarget(GameObject targetObject)
    {
        switch (zombieType)
        {
            case ZombieType.Female:
                return targetObject.layer == LayerMask.NameToLayer("Player");
            case ZombieType.Male:
                return targetObject.layer == LayerMask.NameToLayer("Player") || targetObject.layer == LayerMask.NameToLayer("Wall");
            case ZombieType.Boss:
                return targetObject.layer == LayerMask.NameToLayer("Player");
            default:
                return false;
        }
    }

    private void HandleMovementAndAttack()
    {
        if (Target)
        {
            float speed = 0;
            float targetRadius = 0.5f;
            float targetRange = 0.5f;

            if (IsInAttackRange())
            {
                StartCoroutine(Attack());
            }

            if (!isAttack)
            {
                nav.isStopped = false;
                speed = nav.velocity.magnitude;
            }
            else
            {
                nav.isStopped = true;
            }

            UpdateAnimation(speed);

            if (isChase)
            {
                nav.SetDestination(Target.position);
            }

            if (zombieType == ZombieType.Boss)
            {
                HandleBossJump();
            }
        }
        else
        {
            nav.isStopped = true;
            UpdateAnimation(0);
        }
    }

    private bool IsInAttackRange()
    {
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 0.5f, transform.forward, 0.5f, LayerMask.GetMask("Player", "Wall"));
        return rayHits.Length > 0 && !isAttack;
    }

    private void UpdateAnimation(float speed)
    {
        anim.SetBool("isWalking", speed > 0.1f);
        anim.SetFloat("speed", speed);
    }

    private void HandleBossJump()
    {
        // Implement boss jump logic here
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        yield return new WaitForSeconds(1f);

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    public void DoAttack()
    {
        if (enemyAttack != null)
            enemyAttack.DoAttack();
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            anim.SetTrigger("damage");
            SpawnBloodEffect();

            if (health <= 0 && (PhotonNetwork.IsMasterClient || photonView.IsMine))
            {
                photonView.RPC("EnemyDeath", RpcTarget.All);
                OnEnemyKilled?.Invoke(this);
            }
        }
    }

    private void SpawnBloodEffect()
    {
            // 피 효과 생성
            if (bloodSplashPrefab != null)
            {
                // 피격 지점 계산 (예: 적의 중심점에서 약간 랜덤하게)
                Vector3 hitPoint = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f));  
                
                // 피 효과 인스턴스화 및 방향 설정
                GameObject bloodEffect = Instantiate(bloodSplashPrefab, hitPoint, Quaternion.identity);
                bloodEffect.transform.LookAt(hitPoint + Random.insideUnitSphere); // 랜덤한 방향으로 설정
    
                // 파티클 시스템 재생 및 삭제
                ParticleSystem particleSystem = bloodEffect.GetComponent<ParticleSystem>();
                if (particleSystem != null)
                {
                    particleSystem.Play();
                    Destroy(bloodEffect, particleSystem.main.duration);
                }
            }
    }

    [PunRPC]
    void EnemyDeath()
    {
        isDead = true;
        nav.enabled = false;
        anim.SetTrigger("death");

        if (photonView.IsMine)
        {
            StartCoroutine(WaitForDeathAnimationAndDisable());
        }
    }

    IEnumerator WaitForDeathAnimationAndDisable()
    {
        // yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + 2f);
        // photonView.RPC("StopAction", RpcTarget.All);

        // 사망 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        // 추가 2초 대기
        yield return new WaitForSeconds(2f);

        // 애니메이션이 끝난 후 StopAction RPC 호출
        photonView.RPC("StopAction", RpcTarget.All);
    }

    [PunRPC]
    public void StopAction()
    {
        isChase = false;
        nav.enabled = false;
        anim.enabled = false;

        UpdateTargetCorutine = null;
        if (isDead)
        {
            ReturnToPool();
        }
        gameObject.SetActive(false);
    }

    private void ReturnToPool()
    {
        switch (zombieType)
        {
            case ZombieType.Male:
                EnemySpawnPool.Instance.maleZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.maleEnemiesToSpawn -= 1;
                break;
            case ZombieType.Female:
                EnemySpawnPool.Instance.femaleZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.femaleEnemiesToSpawn -= 1;
                break;
            case ZombieType.Boss:
                EnemySpawnPool.Instance.bossZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.bossEnemiesToSpawn -= 1;
                break;
        }
    }

    [PunRPC]
    public void ReStartAction()
    {
        if (nav.enabled == false)
            nav.enabled = true;
        anim.enabled = true;
        if (isDead && zombieType == ZombieType.Boss)
            EnemyController.Instance.ReStartTarget();
        
        InitializeZombie();
        StartUpdateTargetCorutine();
    }

    public void StopUpdateTargetCorutine()
    {
        if (UpdateTargetCorutine != null)
        {
            StopCoroutine(UpdateTarget());
            UpdateTargetCorutine = null;
        }
    }

    public void StartUpdateTargetCorutine()
    {
        if (UpdateTargetCorutine == null)
        {
            UpdateTargetCorutine = StartCoroutine(UpdateTarget());
        }
    }

    [PunRPC]
    void SetTarget(int targetViewID)
    {
        PhotonView targetPhotonView = PhotonView.Find(targetViewID);
        if (targetPhotonView != null)
        {
            Target = targetPhotonView.transform;
        }
    }

}



