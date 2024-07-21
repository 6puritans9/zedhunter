// using Photon.Pun;
// using Photon.Realtime;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.AI;

// public class EnemyHealth : MonoBehaviourPun
// {
//     public static EnemyHealth Instance;

//     public LayerMask whatIsTarget; // ���� ��� ���̾�

//     public Coroutine UpdateTargetCorutine;

//     public Animator anim;

//     [HideInInspector]public Transform Target;
//     public bool isChase;
//     [HideInInspector]public NavMeshAgent nav;

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
//             // �� ���� ���� �� closestDistance�� �ʱ�ȭ
//             closestDistance = Mathf.Infinity;
//             // 20 ������ �������� ���� ������ ���� �׷�����, ���� ��ġ�� ��� �ݶ��̴��� ������
//             // ��, targetLayers�� �ش��ϴ� ���̾ ���� �ݶ��̴��� ���������� ���͸�
//             colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
//             GameObject closestTarget = null;

//             // ��� �ݶ��̴����� ��ȸ�ϸ鼭, ����ִ� �÷��̾ ã��
//             for (int i = 0; i < colliders.Length; i++)
//             {
//                 // �ݶ��̴��κ��� LivingEntity ������Ʈ ��������
//                 GameObject livingEntity = colliders[i].gameObject;

//                 // LivingEntity ������Ʈ�� ����
//                 if (livingEntity.layer == 7)
//                 {
//                     float distanceToTarget = Vector3.Distance(transform.position, livingEntity.transform.position);
//                     if (distanceToTarget < closestDistance)
//                     {
//                         closestDistance = distanceToTarget;
//                         closestTarget = livingEntity;
//                     }

//                     /*
//                     * �÷��̾ Ÿ������ ����
//                     * Photon�� RPC ȣ���� ���� ��� Ŭ���̾�Ʈ����
//                     * SetTarget�Լ��� ȣ��
//                     * ViewID�� Ÿ�� ����ȭ
//                     */
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

//             //���������Ǻ�
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
//                         if(hitObject.TryGetComponent(out ActionStateManager player))
//                         {
//                             enemyAttack.targetPlayer = player.gameObject;
//                             break;
//                         }
//                     }
//                     StartCoroutine(Attack());
//                 }

//                 //�������̸� ��� nav��� ����
//                 if(!isAttack)
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
//                 nav.isStopped = true;// ���� ��� ���� : AI �̵� ����
//                 SetRandomDestination();
//             }
//             // 0.25�� �ֱ�� ó�� �ݺ�
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
//         if(UpdateTargetCorutine != null)
//         {
//             StopCoroutine(UpdateTarget());
//             UpdateTargetCorutine = null;
//         }
//     }

//     public void StartUpdateTargetCorutine()
//     {
//         if(UpdateTargetCorutine == null)
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
//             if (PhotonNetwork.IsMasterClient)
//             {
//                 //������ RPC�� EnemyDeath()ȣ���� EnemyPool�� ����ֱ�
//                 if (health <= 0)
//                 {
//                     photonView.RPC("EnemyDeath", RpcTarget.All);
//                     EnemySpawnPool.Instance.enemyPool.Enqueue(this);
//                 }
//             }
//             /*else
//                 Debug.Log("Hit!");*/
//         }
//     }

//     [PunRPC]
//     void EnemyDeath()
//     {
//         if (isDead) return;

//         if (photonView.IsMine)
//         {
//             photonView.RPC("StopAction", RpcTarget.All);

//             // OnEnemyKilled �̺�Ʈ ȣ��
//             OnEnemyKilled?.Invoke(this);
//         }
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
//         if(nav.enabled == false)
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

    public LayerMask whatIsTarget; // 타겟 레이어

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

    public float health = 20;
    [HideInInspector] public bool isDead;

    EnemyAttack enemyAttack;
    bool isAttack;

    private void Awake()
    {
        Instance = this;
        enemyAttack = GetComponentInChildren<EnemyAttack>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        isAttack = false;
        isChase = true;
        isDead = false;
        StartCoroutine(UpdateTarget());
    }

    IEnumerator UpdateTarget()
    {
        while (!isDead)
        {
            // 매 업데이트마다 closestDistance를 초기화
            closestDistance = Mathf.Infinity;
            // 15 거리 내의 타겟 레이어를 가진 콜라이더들 찾기
            colliders = Physics.OverlapSphere(transform.position, 15, whatIsTarget);
            GameObject closestTarget = null;

            // 모든 콜라이더를 확인하며, 가장 가까운 플레이어 찾기
            for (int i = 0; i < colliders.Length; i++)
            {
                GameObject livingEntity = colliders[i].gameObject;

                if (livingEntity.layer == 7)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, livingEntity.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = livingEntity;
                    }

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

            // 타겟이 없으면 추적 중지
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
                        if (hitObject.TryGetComponent(out ActionStateManager player))
                        {
                            enemyAttack.targetPlayer = player.gameObject;
                            break;
                        }
                    }
                    StartCoroutine(Attack());
                }

                // 추적 중이면 네비게이션 이동
                if (!isAttack)
                {
                    nav.isStopped = false;
                    speed = nav.velocity.magnitude;
                }
                else
                    nav.isStopped = true;

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

                nav.SetDestination(Target.transform.position);
            }
            else
            {
                nav.isStopped = true; // 타겟이 없으면 AI 이동 중지
                SetRandomDestination();
            }
            yield return new WaitForSeconds(0.25f);
        }
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
            anim.SetTrigger("damage");
            if (PhotonNetwork.IsMasterClient)
            {
                // 마스터 클라이언트에서만 죽음 처리
                if (health <= 0)
                {
                    photonView.RPC("EnemyDeath", RpcTarget.All);
                    StartCoroutine(RemoveFromSceneAfterDelay(2f));
                }
            }
        }
    }

    IEnumerator RemoveFromSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void EnemyDeath()
    {
        if (isDead) return;

        if (photonView.IsMine)
        {
            photonView.RPC("StopAction", RpcTarget.All);

            // OnEnemyKilled 이벤트 호출
            OnEnemyKilled?.Invoke(this);
        }
        anim.SetTrigger("death");
    }

    [PunRPC]
    public void StopAction()
    {
        isDead = true;
        isChase = false;

        nav.enabled = false;
        anim.enabled = false;

        UpdateTargetCorutine = null;
    }

    [PunRPC]
    public void ReStartAction()
    {
        if (nav.enabled == false)
            nav.enabled = true;
        anim.enabled = true;

        isDead = false;
        isChase = true;
        health = 25;

        StartUpdateTargetCorutine();
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


