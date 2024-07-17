using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviourPun
{
    public static EnemyHealth Instance;

    public enum ZombieType { Male, Female, Boss}
    public ZombieType zombieType;

    public LayerMask whatIsTarget; // 공격 대상 레이어

    public Coroutine UpdateTargetCorutine;

    public Animator anim;

    [HideInInspector]public Transform Target;
    public bool isChase;
    [HideInInspector]public NavMeshAgent nav;


    public Collider[] colliders;
    float closestDistance = Mathf.Infinity;


    public delegate void EnemyKilledHandler(EnemyHealth enemy);
    public event EnemyKilledHandler OnEnemyKilled;

    public BoxCollider attackRange;

    public float health = 20;
    [HideInInspector] public bool isDead;

    EnemyAttack enemyAttack;
    bool isAttack;

    // === BossParam === 
    public float chaseDistance = 5f;
    public float minJumpDistance = 4f;
    public float maxJumpDistance = 8f;
    public float jumpSpeed = 2f;
    public float jumpHeight = 2f;
    public float jumpCooldown = 3f; // 점프 쿨타임 (초 단위)

    private Vector3 jumpStartPosition;
    private Vector3 jumpTargetPosition;
    private float jumpStartTime;
    private float lastJumpTime; // 마지막 점프 시간
    private bool isJumping = false;
    private bool isJumpAnimating = false;
    // === BossParam === 

    private void Awake()
    {
        Instance = this;

        isDead = false;

        enemyAttack = GetComponentInChildren<EnemyAttack>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        isAttack = false;
        isChase = true;


        StartCoroutine(UpdateTarget());
    }

    

    IEnumerator UpdateTarget()
    {
        while (!isDead)
        {
            closestDistance = Mathf.Infinity;
            GameObject closestTarget = null;
            switch (zombieType)
            {
                case ZombieType.Female:
                    colliders = Physics.OverlapSphere(transform.position, 10, whatIsTarget);
                    break;
                case ZombieType.Male:
                    colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
                    break;
                case ZombieType.Boss:
                    colliders = Physics.OverlapSphere(transform.position, 25, whatIsTarget);
                    break;
            }

            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject targetObject = colliders[i].gameObject;

                //좀비 타입에 따라 다르게 타겟팅
                if (zombieType == ZombieType.Female && targetObject.layer == LayerMask.NameToLayer("Player"))
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = targetObject;
                    }
                }
                else if (zombieType == ZombieType.Male && (targetObject.layer == LayerMask.NameToLayer("Player") || targetObject.layer == LayerMask.NameToLayer("Wall")))
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = targetObject;
                    }
                }
                else if (zombieType == ZombieType.Boss && targetObject.layer == LayerMask.NameToLayer("Player"))
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetObject.transform.position);

                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = targetObject;
                    }
                }

                if (closestTarget != null)
                {
                    Target = closestTarget.transform;
                    photonView.RPC("SetTarget", RpcTarget.AllBuffered, closestTarget.GetComponent<PhotonView>().ViewID);
                }
            }

            if (colliders.Length <= 0)
            {
                isChase = false;
                Target = null;
            }
            else
            {
                isChase = true;
            }

            if (Target)
            {
                float speed = 0;
                float targetRadius = 0.5f;
                float targetRange = 0.5f;
                if(zombieType == ZombieType.Female)
                {
                    RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

                    if (rayHits.Length > 0 && !isAttack)
                    {
                        foreach (RaycastHit hit in rayHits)
                        {
                            GameObject hitObject = hit.collider.gameObject;
                            if (hitObject.TryGetComponent(out ActionStateManager player))
                            {
                                enemyAttack.target = player.gameObject;
                                break;
                            }
                        }
                        StartCoroutine(Attack());
                    }
                }
                else if(zombieType == ZombieType.Male)
                {
                    RaycastHit[] rayHits = Physics.SphereCastAll(
                        transform.position, 
                        targetRadius, 
                        transform.forward, 
                        targetRange, 
                        LayerMask.GetMask("Player") | LayerMask.GetMask("Wall"));

                    if (rayHits.Length > 0 && !isAttack)
                    {
                        foreach (RaycastHit hit in rayHits)
                        {
                            GameObject hitObject = hit.collider.gameObject;
                            if (hitObject.TryGetComponent(out ActionStateManager player))
                            {
                                enemyAttack.target = player.gameObject;
                                break;
                            }
                            else if(hitObject.TryGetComponent(out WallHP wall))
                            {
                                enemyAttack.target = wall.gameObject;
                                break;
                            }
                        }
                        StartCoroutine(Attack());
                    }
                }
                else if (zombieType == ZombieType.Boss)
                {
                    RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

                    if (rayHits.Length > 0 && !isAttack)
                    {
                        foreach (RaycastHit hit in rayHits)
                        {
                            GameObject hitObject = hit.collider.gameObject;
                            if (hitObject.TryGetComponent(out ActionStateManager player))
                            {
                                enemyAttack.target = player.gameObject;
                                break;
                            }
                        }
                        StartCoroutine(Attack());
                    }
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

                if (speed > 0.1f)
                {
                    anim.SetBool("isWalking", true);
                    anim.SetFloat("speed", speed);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    anim.SetFloat("speed", 0f);
                }

                if (isChase)
                {
                    nav.SetDestination(Target.transform.position);
                }
            }
            else
            {
                nav.isStopped = true;
                anim.SetBool("isWalking", false);
                anim.SetFloat("speed", 0f);
            }

            yield return new WaitForSeconds(0.25f);
        }
    }


    /*IEnumerator UpdateTarget()
    {
        while (!isDead)
        {
            // 매 루프 시작 시 closestDistance를 초기화
            closestDistance = Mathf.Infinity;
            // 20 유닛의 반지름을 가진 가상의 구를 그렸을때, 구와 겹치는 모든 콜라이더를 가져옴
            // 단, targetLayers에 해당하는 레이어를 가진 콜라이더만 가져오도록 필터링
            colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
            GameObject closestTarget = null;

            // 모든 콜라이더들을 순회하면서, 살아있는 플레이어를 찾기
            for (int i = 0; i < colliders.Length; i++)
            {
                // 콜라이더로부터 LivingEntity 컴포넌트 가져오기
                GameObject livingEntity = colliders[i].gameObject;

                // LivingEntity 컴포넌트가 존재
                if (livingEntity.layer == 7)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, livingEntity.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = livingEntity;
                    }

                    *//*
                    * 플레이어를 타겟으로 설정
                    * Photon의 RPC 호출을 통해 모든 클라이언트에서
                    * SetTarget함수를 호출
                    * ViewID로 타깃 동기화
                    *//*
                    if (closestTarget != null)
                    {
                        Target = closestTarget.transform;
                        photonView.RPC(
                            "SetTarget",
                            RpcTarget.AllBuffered,
                            closestTarget.GetComponent<PhotonView>().ViewID);
                    }
                }
            }

            //감지여부판별
            if (colliders.Length <= 0)
            {
                isChase = false;
                Target = null;
            }
            else
                isChase = true;

            if (Target)
            {
                float speed = 0;

                float targetRadius = 0.5f;
                float targetRange = 0.5f;
                RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                                                         targetRadius, transform.forward,
                                                         targetRange, LayerMask.GetMask("Player"));
                
                if (rayHits.Length > 0 && !isAttack)
                {
                    foreach (RaycastHit hit in rayHits)
                    {
                        GameObject hitObject = hit.collider.gameObject;
                        if(hitObject.TryGetComponent(out ActionStateManager player))
                        {
                            enemyAttack.targetPlayer = player.gameObject;
                            break;
                        }
                    }
                    StartCoroutine(Attack());
                }

                //공격중이면 잠시 nav잠시 멈춤
                if (!isAttack)
                {
                    nav.isStopped = false;
                    speed = nav.velocity.magnitude;
                }
                else
                {
                    nav.isStopped = true;
                }

                if (speed > 0.1f)
                {
                    anim.SetBool("isWalking", true);
                    anim.SetFloat("speed", speed);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    anim.SetFloat("speed", 0f);
                }

                if(isChase)
                    nav.SetDestination(Target.transform.position);
            }
            else
            {
                nav.isStopped = true;// 추적 대상 없음 : AI 이동 중지
                //SetRandomDestination();
            }
            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }*/

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

    public void StopUpdateTargetCorutine()
    {
        if(UpdateTargetCorutine != null)
        {
            StopCoroutine(UpdateTarget());
            UpdateTargetCorutine = null;
        }
    }

    public void StartUpdateTargetCorutine()
    {
        if(UpdateTargetCorutine == null)
        {
            UpdateTargetCorutine = StartCoroutine(UpdateTarget());
        }
    }



    private void SetRandomDestination()
    {
        anim.SetBool("isWalking", true);
        anim.SetFloat("speed", nav.velocity.magnitude);


        Vector3 randomDir = Random.insideUnitCircle * 2;
        randomDir += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDir, out navHit, 1, NavMesh.AllAreas);

        nav.SetDestination(navHit.position);
        nav.isStopped = false;
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (health > 0)
        {
            health -= damage;

            //죽으면 RPC로 EnemyDeath()호출후 EnemyPool에 집어넣기
            if (health <= 0)
            {
                if (PhotonNetwork.IsMasterClient || photonView.IsMine)
                {
                    photonView.RPC("EnemyDeath", RpcTarget.All);

                    // OnEnemyKilled 이벤트 호출
                    OnEnemyKilled?.Invoke(this);
                }
            }
            /*else
                Debug.Log("Hit!");*/
        }
    }

    [PunRPC]
    void EnemyDeath()
    {
        isDead = true;
        if (photonView.IsMine)
            photonView.RPC("StopAction", RpcTarget.All);
    }

    [PunRPC]
    public void StopAction()
    {
        isChase = false;
        nav.enabled = false;
        anim.enabled = false;
        
        UpdateTargetCorutine = null;
        if(isDead)
        {
            if (zombieType == EnemyHealth.ZombieType.Male)
            {
                EnemySpawnPool.Instance.maleZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.maleEnemiesToSpawn -= 1;
            }
            else if (zombieType == EnemyHealth.ZombieType.Female)
            {
                EnemySpawnPool.Instance.femaleZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.femaleEnemiesToSpawn -= 1;
            }
            else if (zombieType == EnemyHealth.ZombieType.Boss)
            {
                EnemySpawnPool.Instance.bossZombiePool.Enqueue(this);
                EnemySpawnPool.Instance.bossEnemiesToSpawn -= 1;
            }
        }
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void ReStartAction()
    {
        if(nav.enabled == false)
            nav.enabled = true;
        anim.enabled = true;
        if(isDead)
            if (zombieType == ZombieType.Boss)
                EnemyController.Instance.ReStartTarget();
        isDead = false;
        isChase = true;

        switch(zombieType)
        {
            case ZombieType.Female:
                health = 25;
                break;
            case ZombieType.Male:
                health = 80;
                break;
            case ZombieType.Boss:
                health = 300;
                break;
        }

        StartUpdateTargetCorutine();
        /*if (zombieType == EnemyHealth.ZombieType.Boss)
            enemyJump.RestartCorutine();*/
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
